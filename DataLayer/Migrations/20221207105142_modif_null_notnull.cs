using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    public partial class modif_null_notnull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "nombre_choux_total",
                table: "Tirages",
                type: "int",
                maxLength: 6,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<string>(
                name: "tirage_utilisateur",
                table: "Participations",
                type: "nvarchar(17)",
                maxLength: 17,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(17)",
                oldMaxLength: 17,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "nombres_trouves",
                table: "Participations",
                type: "int",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 1);

            migrationBuilder.AlterColumn<int>(
                name: "nombre_choux_remportes",
                table: "Participations",
                type: "int",
                maxLength: 6,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<string>(
                name: "guid_participation",
                table: "Participations",
                type: "nvarchar(22)",
                maxLength: 22,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(22)",
                oldMaxLength: 22,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "nombre_choux_total",
                table: "Tirages",
                type: "int",
                maxLength: 6,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "tirage_utilisateur",
                table: "Participations",
                type: "nvarchar(17)",
                maxLength: 17,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(17)",
                oldMaxLength: 17);

            migrationBuilder.AlterColumn<int>(
                name: "nombres_trouves",
                table: "Participations",
                type: "int",
                maxLength: 1,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "nombre_choux_remportes",
                table: "Participations",
                type: "int",
                maxLength: 6,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "guid_participation",
                table: "Participations",
                type: "nvarchar(22)",
                maxLength: 22,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(22)",
                oldMaxLength: 22);
        }
    }
}
