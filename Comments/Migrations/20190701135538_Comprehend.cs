using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class Comprehend : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sentiment",
                table: "Comment",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "SentimentScoreMixed",
                table: "Comment",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SentimentScoreNegative",
                table: "Comment",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SentimentScoreNeutral",
                table: "Comment",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SentimentScorePositive",
                table: "Comment",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Sentiment",
                table: "Answer",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "SentimentScoreMixed",
                table: "Answer",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SentimentScoreNegative",
                table: "Answer",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SentimentScoreNeutral",
                table: "Answer",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SentimentScorePositive",
                table: "Answer",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sentiment",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "SentimentScoreMixed",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "SentimentScoreNegative",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "SentimentScoreNeutral",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "SentimentScorePositive",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Sentiment",
                table: "Answer");

            migrationBuilder.DropColumn(
                name: "SentimentScoreMixed",
                table: "Answer");

            migrationBuilder.DropColumn(
                name: "SentimentScoreNegative",
                table: "Answer");

            migrationBuilder.DropColumn(
                name: "SentimentScoreNeutral",
                table: "Answer");

            migrationBuilder.DropColumn(
                name: "SentimentScorePositive",
                table: "Answer");
        }
    }
}
