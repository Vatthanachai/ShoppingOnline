using ShoppingOnline.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddHostSetting();
//Add services to the container.
builder.Host.AddAutofacRegister();
//Add default services for the aspire framework.
builder.AddServiceDefaults();

var configuration = builder.Configuration as IConfiguration;
//add services for the application.
builder.Services.AddServiceRegister(configuration, builder.Environment);

var app = builder.Build();

//use the application setting register
app.UseApplicationSetting();

await app.RunAsync();