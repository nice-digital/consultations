using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
	/// <summary>
	/// This migration adds the Status table and inserts data "Draft" and "Submitted". Then it adds columns to the Comment and Answer tables and adds the foreign keys and indexes.
	/// </summary>
	public partial class Submit : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{

			migrationBuilder.CreateTable(
				name: MigrationConstants.Tables.Status.TableName,
				columns: table => new
				{
					StatusID = table.Column<int>(nullable: false),
						//.Annotation(MigrationConstants.Annotations.Identity.Key, MigrationConstants.Annotations.Identity.Value),
					Name = table.Column<string>(maxLength: 100, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey($"PK_{MigrationConstants.Tables.Status.TableName}", x => x.StatusID);
				});

			var draftId = 1;
			migrationBuilder.Sql(
				$@"
					INSERT INTO {MigrationConstants.Tables.Status.TableName}({MigrationConstants.Tables.Status.StatusID},{MigrationConstants.Tables.Status.Name})
					VALUES({draftId}, 'Draft');

					INSERT INTO {MigrationConstants.Tables.Status.TableName}({MigrationConstants.Tables.Status.StatusID},{MigrationConstants.Tables.Status.Name})
					VALUES(2, 'Submitted');
				");

			migrationBuilder.AddColumn<int>(
				name: MigrationConstants.Tables.Comment.StatusID,
				table: MigrationConstants.Tables.Comment.TableName,
				nullable: false,
				defaultValue: draftId);

			migrationBuilder.CreateIndex(
				name: $"IX_{MigrationConstants.Tables.Comment.TableName}_{MigrationConstants.Tables.Comment.StatusID}",
				table: MigrationConstants.Tables.Comment.TableName,
				column: MigrationConstants.Tables.Comment.StatusID);

			migrationBuilder.AddForeignKey(
				name: $"FK_{MigrationConstants.Tables.Comment.TableName}_{MigrationConstants.Tables.Status.TableName}",
				table: MigrationConstants.Tables.Comment.TableName,
				column: MigrationConstants.Tables.Comment.StatusID,
				principalTable: MigrationConstants.Tables.Status.TableName,
				principalColumn: MigrationConstants.Tables.Status.StatusID,
				onDelete: ReferentialAction.Restrict);


			migrationBuilder.AddColumn<int>(
				name: MigrationConstants.Tables.Answer.StatusID,
				table: MigrationConstants.Tables.Answer.TableName,
				nullable: false,
				defaultValue: draftId);

			migrationBuilder.CreateIndex(
				name: $"IX_{MigrationConstants.Tables.Answer.TableName}_{MigrationConstants.Tables.Answer.StatusID}",
				table: MigrationConstants.Tables.Answer.TableName,
				column: MigrationConstants.Tables.Answer.StatusID);

			migrationBuilder.AddForeignKey(
				name: $"FK_{MigrationConstants.Tables.Answer.TableName}_{MigrationConstants.Tables.Status.TableName}",
				table: MigrationConstants.Tables.Answer.TableName,
				column: MigrationConstants.Tables.Answer.StatusID,
				principalTable: MigrationConstants.Tables.Status.TableName,
			  principalColumn: MigrationConstants.Tables.Status.StatusID,
				onDelete: ReferentialAction.Restrict);


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
