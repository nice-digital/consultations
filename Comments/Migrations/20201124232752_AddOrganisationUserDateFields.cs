using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class AddOrganisationUserDateFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "OrganisationUser",
                nullable: false,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "OrganisationUser",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "OrganisationUser");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "OrganisationUser");
        }
    }
}
