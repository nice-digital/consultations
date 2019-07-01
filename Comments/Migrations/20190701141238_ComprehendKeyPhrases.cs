using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class ComprehendKeyPhrases : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.CreateTable(
		        name: "KeyPhrase",
		        columns: table => new
		        {
			        KeyPhraseID = table.Column<int>(nullable: false)
				        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
			        Text = table.Column<string>(nullable: false)
		        },
		        constraints: table =>
		        {
			        table.PrimaryKey("PK_KeyPhrase", x => x.KeyPhraseID);
		        });


	        migrationBuilder.CreateTable(
		        name: "CommentKeyPhrase",
		        columns: table => new
		        {
			        CommentKeyPhraseID = table.Column<int>(nullable: false)
				        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					CommentID = table.Column<int>(nullable: false),
			        KeyPhraseID = table.Column<int>(nullable: false),
			        Score = table.Column<float>(nullable: false)
				},
		        constraints: table =>
		        {
			        table.PrimaryKey("PK_CommentKeyPhrase", x => x.KeyPhraseID);
			        table.ForeignKey(
				        name: "FK_CommentKeyPhrase_CommentID",
				        column: x => x.CommentID,
				        principalTable: "Comment",
				        principalColumn: "CommentID",
				        onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
				        name: "FK_CommentKeyPhrase_KeyPhraseID",
				        column: x => x.KeyPhraseID,
				        principalTable: "KeyPhrase",
				        principalColumn: "KeyPhraseID",
				        onDelete: ReferentialAction.Restrict);
				});

	        migrationBuilder.CreateTable(
		        name: "AnswerKeyPhrase",
		        columns: table => new
		        {
			        AnswerKeyPhraseID = table.Column<int>(nullable: false)
				        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
			        AnswerID = table.Column<int>(nullable: false),
					KeyPhraseID = table.Column<int>(nullable: false),
			        Score = table.Column<float>(nullable: false)
		        },
		        constraints: table =>
		        {
			        table.PrimaryKey("PK_AnswerKeyPhrase", x => x.KeyPhraseID);
			        table.ForeignKey(
				        name: "FK_AnswerKeyPhrase_CommentID",
				        column: x => x.AnswerID,
				        principalTable: "Answer",
				        principalColumn: "AnswerID",
				        onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
				        name: "FK_AnswerKeyPhrase_KeyPhraseID",
				        column: x => x.KeyPhraseID,
				        principalTable: "KeyPhrase",
				        principalColumn: "KeyPhraseID",
				        onDelete: ReferentialAction.Restrict);
		        });
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.DropTable("CommentKeyPhrase");
	        migrationBuilder.DropTable("AnswerKeyPhrase");
	        migrationBuilder.DropTable("KeyPhrase");
		}
    }
}
