using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingOnline.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPasswordPolicyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "tb_user",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "tb_user",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "tb_user");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "tb_user");
        }
    }
}
