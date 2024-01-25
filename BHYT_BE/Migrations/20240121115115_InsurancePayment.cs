using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BHYT_BE.Migrations
{
    /// <inheritdoc />
    public partial class InsurancePayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Insurances",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Insurances",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoRenewal",
                table: "Insurances",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPaymentDate",
                table: "Insurances",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PremiumAmount",
                table: "Insurances",
                type: "numeric",
                maxLength: 64,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Insurances",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Insurances_UserID",
                table: "Insurances",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Insurances_UserID",
                table: "Insurances");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Insurances");

            migrationBuilder.DropColumn(
                name: "IsAutoRenewal",
                table: "Insurances");

            migrationBuilder.DropColumn(
                name: "LastPaymentDate",
                table: "Insurances");

            migrationBuilder.DropColumn(
                name: "PremiumAmount",
                table: "Insurances");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Insurances");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Insurances",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
