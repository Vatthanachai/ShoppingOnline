using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using Serilog;

using ShoppingOnline.Component.Abstractions.DataTypes.Guids;
using ShoppingOnline.Component.Abstractions.Emails;
using ShoppingOnline.Component.Abstractions.Emails.Templates;
using ShoppingOnline.Component.Abstractions.Securities;
using ShoppingOnline.Component.Abstractions.ServiceResponses;
using ShoppingOnline.Component.Abstractions.Services;
using ShoppingOnline.Database.Context;
using ShoppingOnline.Database.UnitOfWork;
using ShoppingOnline.Model.Dto.Auth;
using ShoppingOnline.Model.Entities;

namespace ShoppingOnline.Service.Auth;

public class AuthService(
    IShoppingDbContext context,
    IShoppingUnitOfWork unitOfWork,
    ILogger logger,
    IHttpContextAccessor httpContextAccessor,
    IEncryptionService encryptionService,
    IPasetoTokenService pasetoTokenService,
    IEmailService emailService)
    : BaseService<User, IShoppingDbContext, IShoppingUnitOfWork>(context, unitOfWork, logger, httpContextAccessor), IAuthService
{
    public async Task<ServiceResponse> SignUpAsync(SignUpRequest request)
    {
        var emailExists = await DbSet.AnyAsync(u => u.Email == request.Email);
        if (emailExists)
        {
            return new Service409Response("Email is already registered.");
        }

        var plainPassword = encryptionService.PasswordGenerate();
        var hash = encryptionService.HashPassword(plainPassword, out var salt);
        var passwordHash = encryptionService.CombinePasswordComponents(hash, Convert.ToHexString(salt));

        // The generated password is otherwise only ever delivered by email - unlike the
        // seeded admin user (see DataMockupService), which logs its password so local dev
        // works without a running SMTP catcher. Debug level keeps this out of default log
        // output while still being retrievable when actually needed.
        Logger.Debug("Generated password for {Email}: {Password}", request.Email, plainPassword);

        try
        {
            var body = AccountCredentialsEmailTemplate.Build(
                "Your ShoppingOnline account has been created. Use the credentials below to sign in.",
                request.Email,
                plainPassword);
            await emailService.SendAsync(request.Email, "Your ShoppingOnline account", body);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to send sign-up email to {Email}", request.Email);
            return new Service500Response(ex);
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = passwordHash,
            MustChangePassword = true,
            SecurityStamp = GuidGenerator.NewGuid(8, 4).ToString(),
            Role = UserRole.Customer,
            CreatedBy = "self-register",
            CreatedOn = DateTime.UtcNow,
            IsActive = true,
        };

        DbSet.Add(user);

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to create the account."));
        }

        return new Service200Response<SignUpResult>(new SignUpResult { UserId = user.UserId, Email = user.Email });
    }

    public async Task<ServiceResponse> SignInAsync(SignInRequest request)
    {
        var user = await DbSet.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null)
        {
            return new Service401Response("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            return new Service403Response("This account is inactive.");
        }

        var (hash, salt, _) = encryptionService.Extract(user.PasswordHash);
        if (!encryptionService.VerifyPassword(request.Password, hash, salt))
        {
            return new Service401Response("Invalid email or password.");
        }

        var token = pasetoTokenService.GenerateToken(new PasetoTokenClaims
        {
            UserId = user.UserId,
            Email = user.Email,
            SecurityStamp = user.SecurityStamp,
            MustChangePassword = user.MustChangePassword,
            Role = user.Role.ToString(),
        });

        return new Service200Response<SignInResult>(new SignInResult
        {
            Token = token.Value,
            ExpiresAt = token.ExpiresAt,
            MustChangePassword = user.MustChangePassword,
        });
    }

    public async Task<ServiceResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var user = await DbSet.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user is null)
        {
            return new Service404Response("User not found.");
        }

        var (hash, salt, _) = encryptionService.Extract(user.PasswordHash);
        if (!encryptionService.VerifyPassword(request.CurrentPassword, hash, salt))
        {
            return new Service401Response("Current password is incorrect.");
        }

        var newHash = encryptionService.HashPassword(request.NewPassword, out var newSalt);
        user.PasswordHash = encryptionService.CombinePasswordComponents(newHash, Convert.ToHexString(newSalt));
        user.MustChangePassword = false;
        user.SecurityStamp = GuidGenerator.NewGuid(8, 4).ToString();
        user.ModifiedBy = "change-password";
        user.ModifiedDate = DateTime.UtcNow;

        var committed = await UnitOfWork.CommitAsync();
        if (!committed)
        {
            return new Service500Response(new Exception("Failed to update the password."));
        }

        return new Service200Response("Password changed successfully. Please sign in again.");
    }

    public async Task<ServiceResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await DbSet.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is not null)
        {
            var plainPassword = encryptionService.PasswordGenerate();
            var hash = encryptionService.HashPassword(plainPassword, out var salt);
            var passwordHash = encryptionService.CombinePasswordComponents(hash, Convert.ToHexString(salt));

            Logger.Debug("Generated password for {Email}: {Password}", user.Email, plainPassword);

            try
            {
                var body = AccountCredentialsEmailTemplate.Build(
                    "You requested a password reset. Use the new credentials below to sign in.",
                    user.Email,
                    plainPassword);
                await emailService.SendAsync(user.Email, "Your ShoppingOnline password has been reset", body);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to send forgot-password email to {Email}", user.Email);
                return new Service500Response(ex);
            }

            user.PasswordHash = passwordHash;
            user.MustChangePassword = true;
            user.SecurityStamp = GuidGenerator.NewGuid(8, 4).ToString();
            user.ModifiedBy = "forgot-password";
            user.ModifiedDate = DateTime.UtcNow;

            var committed = await UnitOfWork.CommitAsync();
            if (!committed)
            {
                return new Service500Response(new Exception("Failed to reset the password."));
            }
        }

        return new Service200Response("If the email exists in our system, a new password has been sent.");
    }
}
