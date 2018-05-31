//using Microsoft.EntityFrameworkCore.Migrations;
//using System;
//using System.Collections.Generic;

//namespace Comments.Migrations
//{
//    public partial class TestRemoveColumn : Migration
//    {
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropColumn(
//                name: "TestNewColumn",
//                table: "QuestionType");
//        }

//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.AddColumn<bool>(
//                name: "TestNewColumn",
//                table: "QuestionType",
//                nullable: false,
//                defaultValue: false);
//        }
//    }
//}
