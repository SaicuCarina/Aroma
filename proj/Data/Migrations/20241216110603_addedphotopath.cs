using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace proj.Data.Migrations
{
    public partial class addedphotopath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Video",
                table: "Recipes",
                newName: "VideoPath");

            migrationBuilder.RenameColumn(
                name: "Photo",
                table: "Recipes",
                newName: "PhotoPath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VideoPath",
                table: "Recipes",
                newName: "Video");

            migrationBuilder.RenameColumn(
                name: "PhotoPath",
                table: "Recipes",
                newName: "Photo");
        }
    }
}
