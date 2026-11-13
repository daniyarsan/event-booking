using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventBooking.Infrastructure.Migrations;

public partial class ProtectVenueSeats : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_reservation_seats_seats_seat_id",
            table: "reservation_seats");

        migrationBuilder.AddForeignKey(
            name: "FK_reservation_seats_seats_seat_id",
            table: "reservation_seats",
            column: "seat_id",
            principalTable: "seats",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        // PostgreSQL checks the final layout at commit, allowing seats to exchange positions.
        migrationBuilder.Sql("""
            ALTER TABLE seats
            ADD CONSTRAINT uq_seats_venue_position
            UNIQUE (venue_id, "RowNumber", "SeatNumber")
            DEFERRABLE INITIALLY DEFERRED;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("ALTER TABLE seats DROP CONSTRAINT uq_seats_venue_position;");

        migrationBuilder.DropForeignKey(
            name: "FK_reservation_seats_seats_seat_id",
            table: "reservation_seats");

        migrationBuilder.AddForeignKey(
            name: "FK_reservation_seats_seats_seat_id",
            table: "reservation_seats",
            column: "seat_id",
            principalTable: "seats",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
