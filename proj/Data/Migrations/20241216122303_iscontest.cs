using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace proj.Data.Migrations
{
    public partial class iscontest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsContest",
                table: "Recipes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsContest",
                table: "Recipes");
        }
    }
}
