using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniGMAO.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Localisation = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TypeEquipement = table.Column<string>(type: "TEXT", nullable: false),
                    Statut = table.Column<string>(type: "TEXT", nullable: false),
                    DateInstallation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Techniciens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Prenom = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Specialite = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Telephone = table.Column<string>(type: "TEXT", nullable: true),
                    EstDisponible = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Techniciens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Interventions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EquipementId = table.Column<int>(type: "INTEGER", nullable: false),
                    TechnicienId = table.Column<int>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Priorite = table.Column<string>(type: "TEXT", nullable: false),
                    Statut = table.Column<string>(type: "TEXT", nullable: false),
                    TypeIntervention = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DatePrisEnCharge = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateResolution = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Rapport = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CoutHeuresTotalTechnicien = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interventions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interventions_Equipements_EquipementId",
                        column: x => x.EquipementId,
                        principalTable: "Equipements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interventions_Techniciens_TechnicienId",
                        column: x => x.TechnicienId,
                        principalTable: "Techniciens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PiecesDetachees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InterventionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Reference = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Quantite = table.Column<int>(type: "INTEGER", nullable: false),
                    PrixUnitaire = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PiecesDetachees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PiecesDetachees_Interventions_InterventionId",
                        column: x => x.InterventionId,
                        principalTable: "Interventions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Equipements",
                columns: new[] { "Id", "DateInstallation", "Description", "Localisation", "Nom", "Statut", "TypeEquipement" },
                values: new object[,]
                {
                    { 1, new DateTime(2020, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Atelier Nord", "Compresseur A1", "Opérationnel", "Compresseur" },
                    { 2, new DateTime(2019, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Ligne 2", "Convoyeur B2", "En panne", "Convoyeur" },
                    { 3, new DateTime(2018, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Salle des machines", "Chaudière C3", "En maintenance", "Chaudière" },
                    { 4, new DateTime(2021, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Station pompage", "Pompe D4", "Opérationnel", "Pompe" },
                    { 5, new DateTime(2022, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Extérieur", "Groupe électrogène E5", "Opérationnel", "Électrique" }
                });

            migrationBuilder.InsertData(
                table: "Techniciens",
                columns: new[] { "Id", "Email", "EstDisponible", "Nom", "Prenom", "Specialite", "Telephone" },
                values: new object[,]
                {
                    { 1, "k.bensalem@gmao.tn", true, "Bensalem", "Karim", "Électromécanique", null },
                    { 2, "s.oueslati@gmao.tn", false, "Oueslati", "Sana", "Hydraulique", null },
                    { 3, "m.trabelsi@gmao.tn", true, "Trabelsi", "Mohamed", "Électrique", null }
                });

            migrationBuilder.InsertData(
                table: "Interventions",
                columns: new[] { "Id", "CoutHeuresTotalTechnicien", "DateCreation", "DatePrisEnCharge", "DateResolution", "Description", "EquipementId", "Priorite", "Rapport", "Statut", "TechnicienId", "TypeIntervention" },
                values: new object[,]
                {
                    { 1, 150m, new DateTime(2026, 3, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Blocage du moteur principal du convoyeur", 2, "Critique", null, "En cours", 1, "Corrective" },
                    { 2, 200m, new DateTime(2026, 3, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Entretien annuel chaudière", 3, "Normale", null, "En cours", 2, "Préventive" },
                    { 3, 80m, new DateTime(2026, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fuite d'huile compresseur", 1, "Haute", "Remplacement joint torique + vidange complète", "Résolue", 3, "Corrective" }
                });

            migrationBuilder.InsertData(
                table: "PiecesDetachees",
                columns: new[] { "Id", "Designation", "InterventionId", "PrixUnitaire", "Quantite", "Reference" },
                values: new object[,]
                {
                    { 1, "Joint torique 50mm", 3, 15m, 2, "JT-001" },
                    { 2, "Huile hydraulique 5L", 3, 45m, 1, "HYD-220" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_EquipementId",
                table: "Interventions",
                column: "EquipementId");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_TechnicienId",
                table: "Interventions",
                column: "TechnicienId");

            migrationBuilder.CreateIndex(
                name: "IX_PiecesDetachees_InterventionId",
                table: "PiecesDetachees",
                column: "InterventionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PiecesDetachees");

            migrationBuilder.DropTable(
                name: "Interventions");

            migrationBuilder.DropTable(
                name: "Equipements");

            migrationBuilder.DropTable(
                name: "Techniciens");
        }
    }
}
