using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Innemost.LogLife.API.Migrations.LifeRecord
{
    public partial class InitLifeRecordDbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Province = table.Column<string>(type: "varchar(50) CHARACTER SET utf8mb4", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "varchar(50) CHARACTER SET utf8mb4", maxLength: 50, nullable: false),
                    County = table.Column<string>(type: "varchar(50) CHARACTER SET utf8mb4", maxLength: 50, nullable: false),
                    Town = table.Column<string>(type: "varchar(50) CHARACTER SET utf8mb4", maxLength: 50, nullable: false),
                    Place = table.Column<string>(type: "varchar(50) CHARACTER SET utf8mb4", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                    table.UniqueConstraint("AK_Location_Province_City_County_Town_Place", x => new { x.Province, x.City, x.County, x.Town, x.Place });
                });

            migrationBuilder.CreateTable(
                name: "MusicRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MusicName = table.Column<string>(type: "varchar(50) CHARACTER SET utf8mb4", maxLength: 50, nullable: false),
                    Singer = table.Column<string>(type: "varchar(50) CHARACTER SET utf8mb4", maxLength: 50, nullable: false),
                    Album = table.Column<string>(type: "varchar(50) CHARACTER SET utf8mb4", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TextType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    TextTypeName = table.Column<string>(type: "varchar(15) CHARACTER SET utf8mb4", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LifeRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: true),
                    Text = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", maxLength: 3000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TextTypeId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    PublishTime = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    MusicRecordId = table.Column<int>(type: "int", nullable: false),
                    Path = table.Column<string>(type: "varchar(95) CHARACTER SET utf8mb4", nullable: false, defaultValue: "Memories"),
                    IsShared = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(95) CHARACTER SET utf8mb4", maxLength: 95, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifeRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LifeRecord_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LifeRecord_MusicRecord_MusicRecordId",
                        column: x => x.MusicRecordId,
                        principalTable: "MusicRecord",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LifeRecord_TextType_TextTypeId",
                        column: x => x.TextTypeId,
                        principalTable: "TextType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LifeRecord_LocationId",
                table: "LifeRecord",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LifeRecord_MusicRecordId",
                table: "LifeRecord",
                column: "MusicRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_LifeRecord_Path",
                table: "LifeRecord",
                column: "Path");

            migrationBuilder.CreateIndex(
                name: "IX_LifeRecord_PublishTime",
                table: "LifeRecord",
                column: "PublishTime");

            migrationBuilder.CreateIndex(
                name: "IX_LifeRecord_TextTypeId",
                table: "LifeRecord",
                column: "TextTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientRequest");

            migrationBuilder.DropTable(
                name: "LifeRecord");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "MusicRecord");

            migrationBuilder.DropTable(
                name: "TextType");
        }
    }
}
