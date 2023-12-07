using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BHYT_BE.Migrations
{
    /// <inheritdoc />
    public partial class Insurance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Insurances",
                columns: table => new
                {
                    InsuranceID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PersonID = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PhoneNumeber = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    DOB = table.Column<DateTime>(type: "date", nullable: false),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Nation = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Nationality = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Sex = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insurances", x => x.InsuranceID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Insurances");
        }
    }
}
