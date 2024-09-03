using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class key : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Titulo",
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

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Lives",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_TimeSelections_StartTime",
                table: "TimeSelections",
                column: "StartTime"
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
                    "PerfilId",
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_LiveBackstages_LiveId_TimeSelectionId",
                table: "LiveBackstages",
                columns: new[] { "LiveId", "TimeSelectionId" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeSelections_StartTime",
                table: "TimeSelections"
            );

            migrationBuilder.DropIndex(
                name: "IX_Lives_StreamId_UrlAlias_Titulo_Descricao_LiveEstaAberta_UltimaAtualizacao_Visibility_PerfilId",
                table: "Lives"
            );

            migrationBuilder.DropIndex(
                name: "IX_LiveBackstages_LiveId_TimeSelectionId",
                table: "LiveBackstages"
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
                name: "Titulo",
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

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Lives",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true
            );
        }
    }
}
