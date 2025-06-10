using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GesturBee_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddClassExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassExercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassExercises_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassExercises_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseItemAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassExerciseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseItemAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseItemAnswers_ClassExercises_ClassExerciseId",
                        column: x => x.ClassExerciseId,
                        principalTable: "ClassExercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseItemAnswers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassExercises_ClassId",
                table: "ClassExercises",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassExercises_ExerciseId",
                table: "ClassExercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseItemAnswers_ClassExerciseId",
                table: "ExerciseItemAnswers",
                column: "ClassExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseItemAnswers_UserId",
                table: "ExerciseItemAnswers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseItemAnswers");

            migrationBuilder.DropTable(
                name: "ClassExercises");
        }
    }
}
