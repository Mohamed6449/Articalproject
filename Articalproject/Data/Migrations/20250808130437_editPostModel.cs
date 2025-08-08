using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Articalproject.Data.Migrations
{
    /// <inheritdoc />
    public partial class editPostModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorPosts_AspNetUsers_UserId",
                table: "AuthorPosts");

            migrationBuilder.DropIndex(
                name: "IX_AuthorPosts_UserId",
                table: "AuthorPosts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AuthorPosts");

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "AuthorPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GetPostsViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoryNameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryNameEn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GetPostsViewModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorPosts_AuthorId",
                table: "AuthorPosts",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorPosts_Authors_AuthorId",
                table: "AuthorPosts",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorPosts_Authors_AuthorId",
                table: "AuthorPosts");

            migrationBuilder.DropTable(
                name: "GetPostsViewModel");

            migrationBuilder.DropIndex(
                name: "IX_AuthorPosts_AuthorId",
                table: "AuthorPosts");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "AuthorPosts");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AuthorPosts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorPosts_UserId",
                table: "AuthorPosts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorPosts_AspNetUsers_UserId",
                table: "AuthorPosts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
