using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityTapsiDoc.Identity.Infra.Data.Command.Migrations
{
    /// <inheritdoc />
    public partial class addTapsiUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TapsiUserId",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TapsiUserId",
                table: "Users");
        }
    }
}
