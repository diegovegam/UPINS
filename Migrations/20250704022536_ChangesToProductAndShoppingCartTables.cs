using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UPINS.Migrations
{
    /// <inheritdoc />
    public partial class ChangesToProductAndShoppingCartTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantityInShoppingCart",
                table: "Product",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityInShoppingCart",
                table: "Product");
        }
    }
}
