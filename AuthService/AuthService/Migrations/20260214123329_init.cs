using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "credentials",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "text", nullable: true),
                    email_verified = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    google_o_auth_id = table.Column<string>(type: "text", nullable: true),
                    apple_o_auth_id = table.Column<string>(type: "text", nullable: true),
                    role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_credentials", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_credentials_apple_o_auth_id",
                table: "credentials",
                column: "apple_o_auth_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_credentials_email",
                table: "credentials",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_credentials_google_o_auth_id",
                table: "credentials",
                column: "google_o_auth_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "credentials");
        }
    }
}
