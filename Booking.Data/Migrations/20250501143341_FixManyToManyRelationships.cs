using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixManyToManyRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FacilitySchedules",
                table: "FacilitySchedules");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "FacilitySchedules",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FacilitySchedules",
                table: "FacilitySchedules",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_FacilityId_UserId",
                table: "Reservations",
                columns: new[] { "FacilityId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_FacilitySchedules_FacilityId_ScheduleId",
                table: "FacilitySchedules",
                columns: new[] { "FacilityId", "ScheduleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_FacilityId_UserId",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FacilitySchedules",
                table: "FacilitySchedules");

            migrationBuilder.DropIndex(
                name: "IX_FacilitySchedules_FacilityId_ScheduleId",
                table: "FacilitySchedules");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FacilitySchedules");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations",
                columns: new[] { "FacilityId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_FacilitySchedules",
                table: "FacilitySchedules",
                columns: new[] { "FacilityId", "ScheduleId" });
        }
    }
}
