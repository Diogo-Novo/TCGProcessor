using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TCGProcessor.Migrations
{
    /// <inheritdoc />
    public partial class addscryfallid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScryfallId",
                table: "ScryfallCache",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_ScryfallCache_ScryfallId",
                table: "ScryfallCache",
                column: "ScryfallId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScryfallCache_ScryfallId",
                table: "ScryfallCache");

            migrationBuilder.DropColumn(
                name: "ScryfallId",
                table: "ScryfallCache");
        }
    }
}
