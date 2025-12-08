using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gentech_services.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicianIdToServiceOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TechnicianID",
                table: "ServiceOrders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_TechnicianID",
                table: "ServiceOrders",
                column: "TechnicianID");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Users_TechnicianID",
                table: "ServiceOrders",
                column: "TechnicianID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Users_TechnicianID",
                table: "ServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_TechnicianID",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "TechnicianID",
                table: "ServiceOrders");
        }
    }
}
