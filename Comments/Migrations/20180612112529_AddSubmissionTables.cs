using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Comments.Migrations
{
    public partial class AddSubmissionTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusID",
                table: "Comment",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.AddColumn<int>(
                name: "StatusID",
                table: "Answer",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    StatusID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.StatusID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_StatusID",
                table: "Comment",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_StatusID",
                table: "Answer",
                column: "StatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_Status",
                table: "Answer",
                column: "StatusID",
                principalTable: "Status",
                principalColumn: "StatusID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Status",
                table: "Comment",
                column: "StatusID",
                principalTable: "Status",
                principalColumn: "StatusID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_Status",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Status",
                table: "Comment");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropIndex(
                name: "IX_Comment_StatusID",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Answer_StatusID",
                table: "Answer");

            migrationBuilder.DropColumn(
                name: "StatusID",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "StatusID",
                table: "Answer");
        }
    }
}
