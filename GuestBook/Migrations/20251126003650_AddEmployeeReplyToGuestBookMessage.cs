using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuestBook.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeReplyToGuestBookMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RepliedAt",
                table: "GuestBookMessages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepliedByUserId",
                table: "GuestBookMessages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reply",
                table: "GuestBookMessages",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuestBookMessages_RepliedByUserId",
                table: "GuestBookMessages",
                column: "RepliedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GuestBookMessages_AspNetUsers_RepliedByUserId",
                table: "GuestBookMessages",
                column: "RepliedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuestBookMessages_AspNetUsers_RepliedByUserId",
                table: "GuestBookMessages");

            migrationBuilder.DropIndex(
                name: "IX_GuestBookMessages_RepliedByUserId",
                table: "GuestBookMessages");

            migrationBuilder.DropColumn(
                name: "RepliedAt",
                table: "GuestBookMessages");

            migrationBuilder.DropColumn(
                name: "RepliedByUserId",
                table: "GuestBookMessages");

            migrationBuilder.DropColumn(
                name: "Reply",
                table: "GuestBookMessages");
        }
    }
}
