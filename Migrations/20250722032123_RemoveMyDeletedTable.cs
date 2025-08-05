using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UPINS.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMyDeletedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "BillProduct");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillProduct",
                columns: table => new
                {
                    BillsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillProduct", x => new { x.BillsId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_BillProduct_Bill_BillsId",
                        column: x => x.BillsId,
                        principalTable: "Bill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillProduct_Product_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillProduct_ProductsId",
                table: "BillProduct",
                column: "ProductsId");
        }
    }
}
