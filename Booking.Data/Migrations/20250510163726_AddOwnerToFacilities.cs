using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerToFacilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Facilities",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_OwnerId",
                table: "Facilities",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_AspNetUsers_OwnerId",
                table: "Facilities",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_AspNetUsers_OwnerId",
                table: "Facilities");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_OwnerId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Facilities");
        }
    }
}
