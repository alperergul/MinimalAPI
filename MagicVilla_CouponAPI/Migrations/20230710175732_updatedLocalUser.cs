using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVilla_CouponAPI.Migrations
{
    /// <inheritdoc />
    public partial class updatedLocalUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "LocalUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "LocalUsers");
        }
    }
}
