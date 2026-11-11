using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaRepuestos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarUsuarioIdEnVentas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotente: aplica columna, índice y llave foránea sin fallar si algún objeto ya existía.
            migrationBuilder.Sql(@"
                ALTER TABLE ""Ventas"" ADD COLUMN IF NOT EXISTS ""UsuarioId"" uuid NULL;
                CREATE INDEX IF NOT EXISTS ""IX_Ventas_UsuarioId"" ON ""Ventas"" (""UsuarioId"");
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM pg_constraint WHERE conname = 'FK_Ventas_usuarios_UsuarioId'
                    ) THEN
                        ALTER TABLE ""Ventas""
                            ADD CONSTRAINT ""FK_Ventas_usuarios_UsuarioId""
                            FOREIGN KEY (""UsuarioId"") REFERENCES public.usuarios(""Id"") ON DELETE RESTRICT;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_usuarios_UsuarioId",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_UsuarioId",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Ventas");
        }
    }
}
