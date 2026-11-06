using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaRepuestos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SoporteCargaMasivaYMultiorigen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_productos_CodigoParte",
                table: "productos");

            migrationBuilder.DropIndex(
                name: "IX_productos_Nombre",
                table: "productos");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioVenta",
                table: "productos",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "CodigoParte",
                table: "productos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "productos",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Condicion",
                table: "productos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Costo",
                table: "productos",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "productos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MarcaRepuesto",
                table: "productos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MarcaVehiculo",
                table: "productos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Origen",
                table: "productos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ubicacion",
                table: "productos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "productos",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateIndex(
                name: "IX_productos_CodigoParte",
                table: "productos",
                column: "CodigoParte",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CodigoParte_Origen",
                table: "productos",
                columns: new[] { "CodigoParte", "Origen" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_productos_CodigoParte",
                table: "productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_CodigoParte_Origen",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "Condicion",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "Costo",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "MarcaRepuesto",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "MarcaVehiculo",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "Origen",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "Ubicacion",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "productos");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioVenta",
                table: "productos",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "CodigoParte",
                table: "productos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "productos",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.CreateIndex(
                name: "IX_productos_CodigoParte",
                table: "productos",
                column: "CodigoParte");

            migrationBuilder.CreateIndex(
                name: "IX_productos_Nombre",
                table: "productos",
                column: "Nombre");
        }
    }
}
