using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class ChangeOrganisationUserLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_OrganisationAuthorisation",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Answer_OrganisationUser",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_OrganisationAuthorisation",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_OrganisationUser",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_OrganisationAuthorisationID",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Answer_OrganisationAuthorisationID",
                table: "Answer");

            migrationBuilder.DropColumn(
                name: "OrganisationAuthorisationID",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "OrganisationAuthorisationID",
                table: "Answer");

            migrationBuilder.AddColumn<int>(
                name: "OrganisationAuthorisationID",
                table: "OrganisationUser",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "CollationCode",
                table: "OrganisationAuthorisation",
                nullable: true,
                oldClrType: typeof(int),
                oldMaxLength: 12);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationUser_OrganisationAuthorisationID",
                table: "OrganisationUser",
                column: "OrganisationAuthorisationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_OrganisationUser_OrganisationUserID",
                table: "Answer",
                column: "OrganisationUserID",
                principalTable: "OrganisationUser",
                principalColumn: "OrganisationUserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_OrganisationUser_OrganisationUserID",
                table: "Comment",
                column: "OrganisationUserID",
                principalTable: "OrganisationUser",
                principalColumn: "OrganisationUserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganisationUser_OrganisationAuthorisation",
                table: "OrganisationUser",
                column: "OrganisationAuthorisationID",
                principalTable: "OrganisationAuthorisation",
                principalColumn: "OrganisationAuthorisationID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_OrganisationUser_OrganisationUserID",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_OrganisationUser_OrganisationUserID",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganisationUser_OrganisationAuthorisation",
                table: "OrganisationUser");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationUser_OrganisationAuthorisationID",
                table: "OrganisationUser");

            migrationBuilder.DropColumn(
                name: "OrganisationAuthorisationID",
                table: "OrganisationUser");

            migrationBuilder.AlterColumn<int>(
                name: "CollationCode",
                table: "OrganisationAuthorisation",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganisationAuthorisationID",
                table: "Comment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganisationAuthorisationID",
                table: "Answer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_OrganisationAuthorisationID",
                table: "Comment",
                column: "OrganisationAuthorisationID");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_OrganisationAuthorisationID",
                table: "Answer",
                column: "OrganisationAuthorisationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_OrganisationAuthorisation",
                table: "Answer",
                column: "OrganisationAuthorisationID",
                principalTable: "OrganisationAuthorisation",
                principalColumn: "OrganisationAuthorisationID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_OrganisationUser",
                table: "Answer",
                column: "OrganisationUserID",
                principalTable: "OrganisationUser",
                principalColumn: "OrganisationUserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_OrganisationAuthorisation",
                table: "Comment",
                column: "OrganisationAuthorisationID",
                principalTable: "OrganisationAuthorisation",
                principalColumn: "OrganisationAuthorisationID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_OrganisationUser",
                table: "Comment",
                column: "OrganisationUserID",
                principalTable: "OrganisationUser",
                principalColumn: "OrganisationUserID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
