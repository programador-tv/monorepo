using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AllOtherTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "FeedbackJoinTimes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JoinTimeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataTentativaMarcacao = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true
                    ),
                    AvaliadoCompareceu = table.Column<bool>(type: "bit", nullable: false),
                    AvaliadorCompareceu = table.Column<bool>(type: "bit", nullable: false),
                    DataCancelamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DuracaoPrevista = table.Column<TimeSpan>(type: "time", nullable: true),
                    DuracaoPrevistaFormatada = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: true
                    ),
                    EstimativaSenioridadeAvaliado = table.Column<int>(type: "int", nullable: true),
                    EstimativaSalarioAvaliado = table.Column<decimal>(
                        type: "decimal(18,2)",
                        nullable: true
                    ),
                    ConheciaAvaliadoPreviamente = table.Column<bool>(type: "bit", nullable: true),
                    SatisfacaoExperiencia = table.Column<int>(type: "int", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackJoinTimes", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "FeedbackTimeSelections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeSelectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataDeclaracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aceite = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AvaliadoCompareceu = table.Column<bool>(type: "bit", nullable: false),
                    AvaliadorCompareceu = table.Column<bool>(type: "bit", nullable: false),
                    DataCancelamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DuracaoPrevista = table.Column<TimeSpan>(type: "time", nullable: true),
                    DuracaoPrevistaFormatada = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: true
                    ),
                    EstimativaSenioridadeAvaliado = table.Column<int>(type: "int", nullable: true),
                    EstimativaSalarioAvaliado = table.Column<decimal>(
                        type: "decimal(18,2)",
                        nullable: true
                    ),
                    ConheciaAvaliadoPreviamente = table.Column<bool>(type: "bit", nullable: true),
                    SatisfacaoExperiencia = table.Column<int>(type: "int", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackTimeSelections", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Follows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FollowerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FollowingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Follows", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "FreeTimeBackstages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeSelectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxParticipants = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreeTimeBackstages", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "JoinTimes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeSelectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusJoinTime = table.Column<int>(type: "int", nullable: false),
                    NotifiedMentoriaProxima = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinTimes", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsLiked = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "LiveBackstages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeSelectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TituloTemporario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveBackstages", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Lives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimaAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FormatedDuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodigoLive = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordedUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreamId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LiveEstaAberta = table.Column<bool>(type: "bit", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Visibility = table.Column<bool>(type: "bit", nullable: false),
                    TentativasDeObterUrl = table.Column<int>(type: "int", nullable: false),
                    StatusLive = table.Column<int>(type: "int", nullable: false),
                    IsUsingObs = table.Column<bool>(type: "bit", nullable: false),
                    UrlAlias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lives", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinoPerfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GeradorPerfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoNotificacao = table.Column<int>(type: "int", nullable: false),
                    Vizualizado = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Conteudo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecundaryLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "NotifyUserLiveEarlies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    HasNotificated = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotifyUserLiveEarlies", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Presentes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstaPresente = table.Column<bool>(type: "bit", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Foto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataEntrou = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimaAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presentes", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "PresentesOpenRooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstaPresente = table.Column<bool>(type: "bit", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Foto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataEntrou = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimaAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentesOpenRooms", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodigoSala = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimaAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstaAberto = table.Column<bool>(type: "bit", nullable: false),
                    TipoSala = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Privado = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LiveRelacao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoomRelacao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FreeTimeRelacao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PreLiveRelacao = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "TimeSelections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TituloTemporario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    NotifiedMentoriaProxima = table.Column<bool>(type: "bit", nullable: false),
                    PreviewImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoAction = table.Column<int>(type: "int", nullable: false),
                    Variacao = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSelections", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Visualizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IPV4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataEntrou = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visualizations", x => x.Id);
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Comments");

            migrationBuilder.DropTable(name: "FeedbackJoinTimes");

            migrationBuilder.DropTable(name: "FeedbackTimeSelections");

            migrationBuilder.DropTable(name: "Follows");

            migrationBuilder.DropTable(name: "FreeTimeBackstages");

            migrationBuilder.DropTable(name: "JoinTimes");

            migrationBuilder.DropTable(name: "Likes");

            migrationBuilder.DropTable(name: "LiveBackstages");

            migrationBuilder.DropTable(name: "Lives");

            migrationBuilder.DropTable(name: "Notifications");

            migrationBuilder.DropTable(name: "NotifyUserLiveEarlies");

            migrationBuilder.DropTable(name: "Presentes");

            migrationBuilder.DropTable(name: "PresentesOpenRooms");

            migrationBuilder.DropTable(name: "Rooms");

            migrationBuilder.DropTable(name: "Tags");

            migrationBuilder.DropTable(name: "TimeSelections");

            migrationBuilder.DropTable(name: "Visualizations");
        }
    }
}
