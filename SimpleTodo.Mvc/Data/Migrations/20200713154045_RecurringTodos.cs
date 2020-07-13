using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleTodo.Mvc.Data.Migrations
{
    public partial class RecurringTodos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedFrom",
                table: "Todos",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "Todos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Todos",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedFrom",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Todos");
        }
    }
}
