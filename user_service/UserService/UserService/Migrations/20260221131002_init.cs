using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UserService.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cars",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    driver_id = table.Column<int>(type: "integer", nullable: false),
                    brand = table.Column<string>(type: "text", nullable: false),
                    model = table.Column<string>(type: "text", nullable: false),
                    color = table.Column<string>(type: "text", nullable: false),
                    number = table.Column<string>(type: "text", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    @class = table.Column<int>(name: "class", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cars", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "driver_profiles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    avatar_url = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    is_banned = table.Column<bool>(type: "boolean", nullable: false),
                    total_rides = table.Column<int>(type: "integer", nullable: false),
                    total_reviews = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<float>(type: "real", nullable: false),
                    license_number = table.Column<string>(type: "text", nullable: false),
                    license_expiry_date = table.Column<DateOnly>(type: "date", nullable: false),
                    current_car_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "passenger_profiles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    avatar_url = table.Column<string>(type: "text", nullable: false),
                    is_banned = table.Column<bool>(type: "boolean", nullable: false),
                    total_rides = table.Column<int>(type: "integer", nullable: false),
                    total_reviews = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_passenger_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    recipient_id = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reviews", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cars_driver_id",
                table: "cars",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_cars_number",
                table: "cars",
                column: "number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_driver_profiles_license_number",
                table: "driver_profiles",
                column: "license_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reviews_author_id",
                table: "reviews",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_reviews_recipient_id",
                table: "reviews",
                column: "recipient_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cars");

            migrationBuilder.DropTable(
                name: "driver_profiles");

            migrationBuilder.DropTable(
                name: "passenger_profiles");

            migrationBuilder.DropTable(
                name: "reviews");
        }
    }
}
