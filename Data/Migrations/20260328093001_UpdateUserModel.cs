using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _5sAudit.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dept",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "Plant",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "fsa_users");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "fsa_users",
                newName: "EmailId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "fsa_users",
                newName: "ModifiedDt");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "fsa_users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "EmailId",
                table: "fsa_users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "fsa_users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "fsa_users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDt",
                table: "fsa_users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "fsa_users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeptId",
                table: "fsa_users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Firstname",
                table: "fsa_users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Lastname",
                table: "fsa_users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "fsa_users",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "fsa_users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlantId",
                table: "fsa_users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "fsa_users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "fsa_users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "fsa_users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dob",
                table: "fsa_users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "doj",
                table: "fsa_users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "experience",
                table: "fsa_users",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "CreatedDt",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "DeptId",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "Firstname",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "Lastname",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "PlantId",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "dob",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "doj",
                table: "fsa_users");

            migrationBuilder.DropColumn(
                name: "experience",
                table: "fsa_users");

            migrationBuilder.RenameColumn(
                name: "EmailId",
                table: "fsa_users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "ModifiedDt",
                table: "fsa_users",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "fsa_users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "fsa_users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dept",
                table: "fsa_users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "fsa_users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Plant",
                table: "fsa_users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "fsa_users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
