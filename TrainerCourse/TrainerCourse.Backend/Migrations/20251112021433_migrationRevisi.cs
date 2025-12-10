using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainerCourse.Backend.Migrations
{
    /// <inheritdoc />
    public partial class migrationRevisi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Courses");
        }
    }
}
