using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NI_BE.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabaseMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TRANS_MW_ERC_PM_TN_RADIO_LINK_POWERs",
                columns: table => new
                {
                    NETWORK_SID = table.Column<int>(type: "int", nullable: false),
                    DATETIME_KEY = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NEID = table.Column<float>(type: "real", nullable: false),
                    OBJECT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    INTERVAL = table.Column<int>(type: "int", nullable: false),
                    DIRECTION = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NEALIAS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NETYPE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RXLEVELBELOWTS1 = table.Column<float>(type: "real", nullable: false),
                    RXLEVELBELOWTS2 = table.Column<float>(type: "real", nullable: false),
                    MINRXLEVEL = table.Column<float>(type: "real", nullable: false),
                    MAXRXLEVEL = table.Column<float>(type: "real", nullable: false),
                    TXLEVELABOVETS1 = table.Column<float>(type: "real", nullable: false),
                    MINTXLEVEL = table.Column<float>(type: "real", nullable: false),
                    MAXTXLEVEL = table.Column<float>(type: "real", nullable: false),
                    FAILUREDESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LINK = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FARENDTID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SLOT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PORT = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TRANS_MW_ERC_PM_WAN_RFINPUTPOWERs",
                columns: table => new
                {
                    NETWORK_SID = table.Column<int>(type: "int", nullable: false),
                    DATETIME_KEY = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NODENAME = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NEID = table.Column<float>(type: "real", nullable: false),
                    OBJECT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    INTERVAL = table.Column<int>(type: "int", nullable: false),
                    DIRECTION = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NEALIAS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NETYPE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RFINPUTPOWER = table.Column<float>(type: "real", nullable: false),
                    TID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FARENDTID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SLOT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PORT = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TRANS_MW_ERC_PM_TN_RADIO_LINK_POWERs");

            migrationBuilder.DropTable(
                name: "TRANS_MW_ERC_PM_WAN_RFINPUTPOWERs");
        }
    }
}
