using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lives_StreamId_UrlAlias_Titulo_Descricao_LiveEstaAberta_UltimaAtualizacao_Visibility_PerfilId",
                table: "Lives"
            );

            migrationBuilder.AlterColumn<string>(
                name: "UrlAlias",
                table: "Lives",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "StreamId",
                table: "Lives",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Lives_Descricao_Visibility",
                table: "Lives",
                columns: new[] { "Descricao", "Visibility" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Lives_LiveEstaAberta",
                table: "Lives",
                column: "LiveEstaAberta"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Lives_Titulo_Visibility",
                table: "Lives",
                columns: new[] { "Titulo", "Visibility" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Lives_UltimaAtualizacao",
                table: "Lives",
                column: "UltimaAtualizacao"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Lives_Descricao_Visibility", table: "Lives");

            migrationBuilder.DropIndex(name: "IX_Lives_LiveEstaAberta", table: "Lives");

            migrationBuilder.DropIndex(name: "IX_Lives_Titulo_Visibility", table: "Lives");

            migrationBuilder.DropIndex(name: "IX_Lives_UltimaAtualizacao", table: "Lives");

            migrationBuilder.AlterColumn<string>(
                name: "UrlAlias",
                table: "Lives",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "StreamId",
                table: "Lives",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Lives_StreamId_UrlAlias_Titulo_Descricao_LiveEstaAberta_UltimaAtualizacao_Visibility_PerfilId",
                table: "Lives",
                columns: new[]
                {
                    "StreamId",
                    "UrlAlias",
                    "Titulo",
                    "Descricao",
                    "LiveEstaAberta",
                    "UltimaAtualizacao",
                    "Visibility",
                    "PerfilId"
                }
            );
        }
    }
}
