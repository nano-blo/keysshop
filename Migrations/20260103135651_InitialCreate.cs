using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeysShop.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Developer",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    разработчик = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Developer", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    жанр = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genre", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    имя = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    пароль = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    роль = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "User")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    id_игры = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    название = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    год_выпуска = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    описание = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    id_разработчика = table.Column<int>(type: "int", nullable: true),
                    изображение = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    цена = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.id_игры);
                    table.ForeignKey(
                        name: "FK_Games_Developer_id_разработчика",
                        column: x => x.id_разработчика,
                        principalTable: "Developer",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_игры = table.Column<int>(type: "int", nullable: false),
                    id_пользователя = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.id);
                    table.ForeignKey(
                        name: "FK_Favorites_Games_id_игры",
                        column: x => x.id_игры,
                        principalTable: "Games",
                        principalColumn: "id_игры",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Users_id_пользователя",
                        column: x => x.id_пользователя,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Games_Genre",
                columns: table => new
                {
                    id_игры = table.Column<int>(type: "int", nullable: false),
                    id_жанра = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games_Genre", x => new { x.id_игры, x.id_жанра });
                    table.ForeignKey(
                        name: "FK_Games_Genre_Games_id_игры",
                        column: x => x.id_игры,
                        principalTable: "Games",
                        principalColumn: "id_игры",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Genre_Genre_id_жанра",
                        column: x => x.id_жанра,
                        principalTable: "Genre",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Keys",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_игры = table.Column<int>(type: "int", nullable: true),
                    ключ = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    продан = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keys", x => x.id);
                    table.ForeignKey(
                        name: "FK_Keys_Games_id_игры",
                        column: x => x.id_игры,
                        principalTable: "Games",
                        principalColumn: "id_игры");
                });

            migrationBuilder.CreateTable(
                name: "Buy_History",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_пользователь = table.Column<int>(type: "int", nullable: false),
                    id_ключ = table.Column<int>(type: "int", nullable: false),
                    время_покупки = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gameid_игры = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buy_History", x => x.id);
                    table.ForeignKey(
                        name: "FK_Buy_History_Games_Gameid_игры",
                        column: x => x.Gameid_игры,
                        principalTable: "Games",
                        principalColumn: "id_игры",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Buy_History_Keys_id_ключ",
                        column: x => x.id_ключ,
                        principalTable: "Keys",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Buy_History_Users_id_пользователь",
                        column: x => x.id_пользователь,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buy_History_время_покупки",
                table: "Buy_History",
                column: "время_покупки");

            migrationBuilder.CreateIndex(
                name: "IX_Buy_History_Gameid_игры",
                table: "Buy_History",
                column: "Gameid_игры");

            migrationBuilder.CreateIndex(
                name: "IX_Buy_History_id_ключ",
                table: "Buy_History",
                column: "id_ключ");

            migrationBuilder.CreateIndex(
                name: "IX_Buy_History_id_пользователь",
                table: "Buy_History",
                column: "id_пользователь");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_id_игры",
                table: "Favorites",
                column: "id_игры");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_id_пользователя_id_игры",
                table: "Favorites",
                columns: new[] { "id_пользователя", "id_игры" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_id_разработчика",
                table: "Games",
                column: "id_разработчика");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Genre_id_жанра",
                table: "Games_Genre",
                column: "id_жанра");

            migrationBuilder.CreateIndex(
                name: "IX_Keys_id_игры",
                table: "Keys",
                column: "id_игры");

            migrationBuilder.CreateIndex(
                name: "IX_Users_email",
                table: "Users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Buy_History");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Games_Genre");

            migrationBuilder.DropTable(
                name: "Keys");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Developer");
        }
    }
}
