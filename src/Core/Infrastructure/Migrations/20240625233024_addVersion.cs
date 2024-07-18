using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Visualizations",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "TimeSelections",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Tags",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Rooms",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "PresentesOpenRooms",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Presentes",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Perfils",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "NotifyUserLiveEarlies",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Notifications",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Logs",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Lives",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "LiveBackstages",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Likes",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "JoinTimes",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "HelpBackstages",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "FreeTimeBackstages",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Follows",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "FeedbackTimeSelections",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "FeedbackJoinTimes",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Comments",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "ChatMessages",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Version", table: "Visualizations");

            migrationBuilder.DropColumn(name: "Version", table: "TimeSelections");

            migrationBuilder.DropColumn(name: "Version", table: "Tags");

            migrationBuilder.DropColumn(name: "Version", table: "Rooms");

            migrationBuilder.DropColumn(name: "Version", table: "PresentesOpenRooms");

            migrationBuilder.DropColumn(name: "Version", table: "Presentes");

            migrationBuilder.DropColumn(name: "Version", table: "Perfils");

            migrationBuilder.DropColumn(name: "Version", table: "NotifyUserLiveEarlies");

            migrationBuilder.DropColumn(name: "Version", table: "Notifications");

            migrationBuilder.DropColumn(name: "Version", table: "Logs");

            migrationBuilder.DropColumn(name: "Version", table: "Lives");

            migrationBuilder.DropColumn(name: "Version", table: "LiveBackstages");

            migrationBuilder.DropColumn(name: "Version", table: "Likes");

            migrationBuilder.DropColumn(name: "Version", table: "JoinTimes");

            migrationBuilder.DropColumn(name: "Version", table: "HelpBackstages");

            migrationBuilder.DropColumn(name: "Version", table: "FreeTimeBackstages");

            migrationBuilder.DropColumn(name: "Version", table: "Follows");

            migrationBuilder.DropColumn(name: "Version", table: "FeedbackTimeSelections");

            migrationBuilder.DropColumn(name: "Version", table: "FeedbackJoinTimes");

            migrationBuilder.DropColumn(name: "Version", table: "Comments");

            migrationBuilder.DropColumn(name: "Version", table: "ChatMessages");
        }
    }
}
