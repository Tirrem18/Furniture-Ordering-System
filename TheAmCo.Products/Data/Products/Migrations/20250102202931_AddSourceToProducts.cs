using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheAmCo.Products.Data.Products.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "Products");
        }
    }
}
