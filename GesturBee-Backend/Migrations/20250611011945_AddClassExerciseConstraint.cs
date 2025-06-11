using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GesturBee_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddClassExerciseConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassExercises_Classes_ClassId",
                table: "ClassExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassExercises_Exercises_ExerciseId",
                table: "ClassExercises");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassExercises_Classes_ClassId",
                table: "ClassExercises",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassExercises_Exercises_ExerciseId",
                table: "ClassExercises",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassExercises_Classes_ClassId",
                table: "ClassExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassExercises_Exercises_ExerciseId",
                table: "ClassExercises");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassExercises_Classes_ClassId",
                table: "ClassExercises",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassExercises_Exercises_ExerciseId",
                table: "ClassExercises",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
