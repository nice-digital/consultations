using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class OrganisationalCommenting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganisationAuthorisationID",
                table: "Comment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganisationUserID",
                table: "Comment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentCommentID",
                table: "Comment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganisationAuthorisationID",
                table: "Answer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganisationUserID",
                table: "Answer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentAnswerID",
                table: "Answer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrganisationAuthorisation",
                columns: table => new
                {
                    OrganisationAuthorisationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedByUserID = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    OrganisationID = table.Column<int>(nullable: false),
                    LocationID = table.Column<int>(nullable: false),
                    CollationCode = table.Column<string>(maxLength: 12, nullable: false)
				},
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationAuthorisation", x => x.OrganisationAuthorisationID);
                    table.ForeignKey(
                        name: "FK_OrganisationAuthorisation_Location",
                        column: x => x.LocationID,
                        principalTable: "Location",
                        principalColumn: "LocationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationUser",
                columns: table => new
                {
                    OrganisationUserID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuthorisationSession = table.Column<Guid>(nullable: false),
                    EmailAddress = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationUser", x => x.OrganisationUserID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_OrganisationAuthorisationID",
                table: "Comment",
                column: "OrganisationAuthorisationID");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_OrganisationUserID",
                table: "Comment",
                column: "OrganisationUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ParentCommentID",
                table: "Comment",
                column: "ParentCommentID");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_OrganisationAuthorisationID",
                table: "Answer",
                column: "OrganisationAuthorisationID");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_OrganisationUserID",
                table: "Answer",
                column: "OrganisationUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_ParentAnswerID",
                table: "Answer",
                column: "ParentAnswerID");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationAuthorisation_LocationID",
                table: "OrganisationAuthorisation",
                column: "LocationID");

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
                name: "FK_Answer_Answer",
                table: "Answer",
                column: "ParentAnswerID",
                principalTable: "Answer",
                principalColumn: "AnswerID",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Comment",
                table: "Comment",
                column: "ParentCommentID",
                principalTable: "Comment",
                principalColumn: "CommentID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_OrganisationAuthorisation",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Answer_OrganisationUser",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Answer_Answer",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_OrganisationAuthorisation",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_OrganisationUser",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Comment",
                table: "Comment");

            migrationBuilder.DropTable(
                name: "OrganisationAuthorisation");

            migrationBuilder.DropTable(
                name: "OrganisationUser");

            migrationBuilder.DropIndex(
                name: "IX_Comment_OrganisationAuthorisationID",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_OrganisationUserID",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ParentCommentID",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Answer_OrganisationAuthorisationID",
                table: "Answer");

            migrationBuilder.DropIndex(
                name: "IX_Answer_OrganisationUserID",
                table: "Answer");

            migrationBuilder.DropIndex(
                name: "IX_Answer_ParentAnswerID",
                table: "Answer");

            migrationBuilder.DropColumn(
                name: "OrganisationAuthorisationID",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "OrganisationUserID",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ParentCommentID",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "OrganisationAuthorisationID",
                table: "Answer");

            migrationBuilder.DropColumn(
                name: "OrganisationUserID",
                table: "Answer");

            migrationBuilder.DropColumn(
                name: "ParentAnswerID",
                table: "Answer");
        }
    }
}
