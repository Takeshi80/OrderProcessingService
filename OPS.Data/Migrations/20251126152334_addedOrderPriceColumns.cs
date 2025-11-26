using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OPS.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedOrderPriceColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceWithDiscount",
                table: "Orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceWithoutDiscount",
                table: "Orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceWithDiscount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PriceWithoutDiscount",
                table: "Orders");
        }
    }
}
