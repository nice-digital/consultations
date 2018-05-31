using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Comments.Migrations
{
    public partial class Submit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

	        migrationBuilder.CreateTable(
		        name: "Status",
		        columns: table => new
		        {
			        StatusID = table.Column<int>(nullable: false)
				        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
			        Name = table.Column<string>(maxLength: 100, nullable: false)
		        },
		        constraints: table =>
		        {
			        table.PrimaryKey("PK_Status", x => x.StatusID);
		        });

	        migrationBuilder.Sql(
				@"
					INSERT INTO Status(Name)
					VALUES('Draft');

					INSERT INTO Status(Name)
					VALUES('Submitted');
				");

	        migrationBuilder.AddColumn<int>(
		        name: "StatusID",
		        table: "Comment",
		        nullable: false,
		        defaultValue: 1);

	        migrationBuilder.AddForeignKey(
				name: "FK_Comment_Status",
				table: "Comment",
				column: "StatusID"

				);

	        //migrationBuilder.AlterTable(
	        //	name: "Status",
	        //	)

	        //migrationBuilder.AlterColumn<DateTime>(
	        //    name: "CreatedDate",
	        //    table: "Question",
	        //    nullable: false,
	        //    defaultValueSql: "date('now')",
	        //    oldClrType: typeof(DateTime),
	        //    oldDefaultValueSql: "(getdate())");

	        //migrationBuilder.AlterColumn<DateTime>(
	        //    name: "LastModifiedDate",
	        //    table: "Comment",
	        //    nullable: false,
	        //    defaultValueSql: "date('now')",
	        //    oldClrType: typeof(DateTime),
	        //    oldDefaultValueSql: "(getdate())");

	        //migrationBuilder.AlterColumn<DateTime>(
	        //    name: "CreatedDate",
	        //    table: "Comment",
	        //    nullable: false,
	        //    defaultValueSql: "date('now')",
	        //    oldClrType: typeof(DateTime),
	        //    oldDefaultValueSql: "(getdate())");

	        //migrationBuilder.AlterColumn<DateTime>(
	        //    name: "LastModifiedDate",
	        //    table: "Answer",
	        //    nullable: false,
	        //    defaultValueSql: "date('now')",
	        //    oldClrType: typeof(DateTime),
	        //    oldDefaultValueSql: "(getdate())");

	        //migrationBuilder.AlterColumn<DateTime>(
	        //    name: "CreatedDate",
	        //    table: "Answer",
	        //    nullable: false,
	        //    defaultValueSql: "date('now')",
	        //    oldClrType: typeof(DateTime),
	        //    oldDefaultValueSql: "(getdate())");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.DropTable(
		        name: "Status");

			//migrationBuilder.AlterColumn<DateTime>(
			//    name: "CreatedDate",
			//    table: "Question",
			//    nullable: false,
			//    defaultValueSql: "(getdate())",
			//    oldClrType: typeof(DateTime),
			//    oldDefaultValueSql: "date('now')");

			//migrationBuilder.AlterColumn<DateTime>(
			//    name: "LastModifiedDate",
			//    table: "Comment",
			//    nullable: false,
			//    defaultValueSql: "(getdate())",
			//    oldClrType: typeof(DateTime),
			//    oldDefaultValueSql: "date('now')");

			//migrationBuilder.AlterColumn<DateTime>(
			//    name: "CreatedDate",
			//    table: "Comment",
			//    nullable: false,
			//    defaultValueSql: "(getdate())",
			//    oldClrType: typeof(DateTime),
			//    oldDefaultValueSql: "date('now')");

			//migrationBuilder.AlterColumn<DateTime>(
			//    name: "LastModifiedDate",
			//    table: "Answer",
			//    nullable: false,
			//    defaultValueSql: "(getdate())",
			//    oldClrType: typeof(DateTime),
			//    oldDefaultValueSql: "date('now')");

			//migrationBuilder.AlterColumn<DateTime>(
			//    name: "CreatedDate",
			//    table: "Answer",
			//    nullable: false,
			//    defaultValueSql: "(getdate())",
			//    oldClrType: typeof(DateTime),
			//    oldDefaultValueSql: "date('now')");
		}
    }
}
