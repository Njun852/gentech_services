using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gentech_services.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicianToServiceOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ServiceOrderItems",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ReturnedQuantity",
                table: "ProductOrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ServiceOrderItems");

            migrationBuilder.DropColumn(
                name: "ReturnedQuantity",
                table: "ProductOrderItems");
        }
    }
}
