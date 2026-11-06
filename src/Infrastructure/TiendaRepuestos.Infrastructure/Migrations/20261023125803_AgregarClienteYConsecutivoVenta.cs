using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaRepuestos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarClienteYConsecutivoVenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClienteDocumento",
                table: "Ventas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ClienteId",
                table: "Ventas",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClienteNombre",
                table: "Ventas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Descuento",
                table: "DetallesVenta",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClienteDocumento",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "ClienteNombre",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "Descuento",
                table: "DetallesVenta");
        }
    }
}
