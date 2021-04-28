using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Innemost.LogLife.API.Migrations.IntegrationEventRecord
{
    public partial class InitIntegrationEventRecordDbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntegrationEventRecord",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TransactionId = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    EventTypeName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    State = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EventContent = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    TimesSend = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEventRecord", x => x.EventId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationEventRecord");
        }
    }
}
