using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RepositoryLayer.Migrations
{
    public partial class AddedRegisterDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: -1);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationTime",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "sysdatetime()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationTime",
                table: "Users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "sysdatetime()");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "Email", "FirstName", "LastName", "MobileNumber", "Password", "ZipCode" },
                values: new object[] { -1, "dfgh", "uncle.bob@gmail.com", "Uncle", "Bob", "999-888-7777", "ABC", 234567 });
        }
    }
}
