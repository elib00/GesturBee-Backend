using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GesturBee_Backend.Migrations
{
    /// <inheritdoc />
    public partial class RemovedExerciseIdForeignKeyFromExerciseContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseContents_Exercises_ExerciseId",
                table: "ExerciseContents");

            migrationBuilder.DropIndex(
                name: "IX_ExerciseContents_ExerciseId",
                table: "ExerciseContents");

            migrationBuilder.DropColumn(
                name: "ExerciseId",
                table: "ExerciseContents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExerciseId",
                table: "ExerciseContents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseContents_ExerciseId",
                table: "ExerciseContents",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseContents_Exercises_ExerciseId",
                table: "ExerciseContents",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
