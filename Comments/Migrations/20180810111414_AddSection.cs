using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class AddSection : Migration
    {
		protected override void Up(MigrationBuilder migrationBuilder)
	    {
		    migrationBuilder.AddColumn<string>(
			    name: "Section",
			    table: "Location",
			    nullable: true);
	    }

	    protected override void Down(MigrationBuilder migrationBuilder)
	    {
		    migrationBuilder.DropColumn(
			    name: "Section",
			    table: "Location");
	    }
	}
}
