using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCompositeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservations_FacilityId_UserId",
                table: "Reservations");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_FacilityId",
                table: "Reservations",
                column: "FacilityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservations_FacilityId",
                table: "Reservations");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_FacilityId_UserId",
                table: "Reservations",
                columns: new[] { "FacilityId", "UserId" });
        }
    }
}
