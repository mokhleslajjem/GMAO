using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniGMAO.Models;

namespace MiniGMAO.Data;

public class GmaoDbContext : IdentityDbContext<IdentityUser>
{
    public GmaoDbContext(DbContextOptions<GmaoDbContext> options) : base(options) { }

    public DbSet<Equipement> Equipements => Set<Equipement>();
    public DbSet<Technicien> Techniciens => Set<Technicien>();
    public DbSet<Intervention> Interventions => Set<Intervention>();
    public DbSet<PieceDetachee> PiecesDetachees => Set<PieceDetachee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Intervention>()
            .HasOne(i => i.Equipement)
            .WithMany(e => e.Interventions)
            .HasForeignKey(i => i.EquipementId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Intervention>()
            .HasOne(i => i.Technicien)
            .WithMany(t => t.Interventions)
            .HasForeignKey(i => i.TechnicienId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PieceDetachee>()
            .HasOne(p => p.Intervention)
            .WithMany(i => i.PiecesDetachees)
            .HasForeignKey(p => p.InterventionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Intervention>()
            .Property(i => i.CoutHeuresTotalTechnicien)
            .HasPrecision(10, 2);

        modelBuilder.Entity<PieceDetachee>()
            .Property(p => p.PrixUnitaire)
            .HasPrecision(10, 2);

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Equipement>().HasData(
            new Equipement { Id = 1, Nom = "Compresseur A1", Localisation = "Atelier Nord", TypeEquipement = "Compresseur", Statut = "Opérationnel", DateInstallation = new DateTime(2020, 3, 15) },
            new Equipement { Id = 2, Nom = "Convoyeur B2", Localisation = "Ligne 2", TypeEquipement = "Convoyeur", Statut = "En panne", DateInstallation = new DateTime(2019, 7, 1) },
            new Equipement { Id = 3, Nom = "Chaudière C3", Localisation = "Salle des machines", TypeEquipement = "Chaudière", Statut = "En maintenance", DateInstallation = new DateTime(2018, 1, 20) },
            new Equipement { Id = 4, Nom = "Pompe D4", Localisation = "Station pompage", TypeEquipement = "Pompe", Statut = "Opérationnel", DateInstallation = new DateTime(2021, 5, 10) },
            new Equipement { Id = 5, Nom = "Groupe électrogène E5", Localisation = "Extérieur", TypeEquipement = "Électrique", Statut = "Opérationnel", DateInstallation = new DateTime(2022, 9, 5) }
        );

        modelBuilder.Entity<Technicien>().HasData(
            new Technicien { Id = 1, Nom = "Bensalem", Prenom = "Karim", Specialite = "Électromécanique", Email = "k.bensalem@gmao.tn", EstDisponible = true },
            new Technicien { Id = 2, Nom = "Oueslati", Prenom = "Sana", Specialite = "Hydraulique", Email = "s.oueslati@gmao.tn", EstDisponible = false },
            new Technicien { Id = 3, Nom = "Trabelsi", Prenom = "Mohamed", Specialite = "Électrique", Email = "m.trabelsi@gmao.tn", EstDisponible = true }
        );

        modelBuilder.Entity<Intervention>().HasData(
            new Intervention { Id = 1, EquipementId = 2, TechnicienId = 1, Description = "Blocage du moteur principal du convoyeur", Priorite = "Critique", Statut = "En cours", TypeIntervention = "Corrective", DateCreation = new DateTime(2026, 3, 21), DatePrisEnCharge = new DateTime(2026, 3, 22), CoutHeuresTotalTechnicien = 150 },
            new Intervention { Id = 2, EquipementId = 3, TechnicienId = 2, Description = "Entretien annuel chaudière", Priorite = "Normale", Statut = "En cours", TypeIntervention = "Préventive", DateCreation = new DateTime(2026, 3, 19), DatePrisEnCharge = new DateTime(2026, 3, 20), CoutHeuresTotalTechnicien = 200 },
            new Intervention { Id = 3, EquipementId = 1, TechnicienId = 3, Description = "Fuite d'huile compresseur", Priorite = "Haute", Statut = "Résolue", TypeIntervention = "Corrective", DateCreation = new DateTime(2026, 3, 14), DatePrisEnCharge = new DateTime(2026, 3, 14), DateResolution = new DateTime(2026, 3, 16), Rapport = "Remplacement joint torique + vidange complète", CoutHeuresTotalTechnicien = 80 }
        );

        modelBuilder.Entity<PieceDetachee>().HasData(
            new PieceDetachee { Id = 1, InterventionId = 3, Reference = "JT-001", Designation = "Joint torique 50mm", Quantite = 2, PrixUnitaire = 15 },
            new PieceDetachee { Id = 2, InterventionId = 3, Reference = "HYD-220", Designation = "Huile hydraulique 5L", Quantite = 1, PrixUnitaire = 45 }
        );
    }
}