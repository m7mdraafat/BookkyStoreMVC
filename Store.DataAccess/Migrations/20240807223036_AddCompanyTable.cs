using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: false),
                    StreetAddress = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    City = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    State = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    PostalCode = table.Column<string>(type: "VARCHAR(25)", maxLength: 25, nullable: true),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
