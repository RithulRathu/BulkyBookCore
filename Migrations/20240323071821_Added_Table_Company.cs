using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyBook.Migrations
{
    /// <inheritdoc />
    public partial class Added_Table_Company : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "RegisterModels");

            migrationBuilder.DropColumn(
                name: "ConformPassword",
                table: "RegisterModels");

            migrationBuilder.AddColumn<int>(
                name: "CompanyModelId",
                table: "RegisterModels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompanyModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAuthorizesd = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegisterModels_CompanyModelId",
                table: "RegisterModels",
                column: "CompanyModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegisterModels_CompanyModels_CompanyModelId",
                table: "RegisterModels",
                column: "CompanyModelId",
                principalTable: "CompanyModels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegisterModels_CompanyModels_CompanyModelId",
                table: "RegisterModels");

            migrationBuilder.DropTable(
                name: "CompanyModels");

            migrationBuilder.DropIndex(
                name: "IX_RegisterModels_CompanyModelId",
                table: "RegisterModels");

            migrationBuilder.DropColumn(
                name: "CompanyModelId",
                table: "RegisterModels");

            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "RegisterModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConformPassword",
                table: "RegisterModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
