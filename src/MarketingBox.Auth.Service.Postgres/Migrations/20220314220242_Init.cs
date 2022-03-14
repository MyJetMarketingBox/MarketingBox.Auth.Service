using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketingBox.Auth.Service.Postgres.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auth-service");

            migrationBuilder.CreateTable(
                name: "user",
                schema: "auth-service",
                columns: table => new
                {
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    ExternalUserId = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmailEncrypted = table.Column<string>(type: "text", nullable: true),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Salt = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => new { x.TenantId, x.ExternalUserId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_TenantId_EmailEncrypted",
                schema: "auth-service",
                table: "user",
                columns: new[] { "TenantId", "EmailEncrypted" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_TenantId_Username",
                schema: "auth-service",
                table: "user",
                columns: new[] { "TenantId", "Username" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user",
                schema: "auth-service");
        }
    }
}
