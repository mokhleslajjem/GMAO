using Microsoft.EntityFrameworkCore;
using MiniGMAO.Data;
using MiniGMAO.Models;
using MiniGMAO.Services.Interfaces;

namespace MiniGMAO.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly GmaoDbContext _context;
    public DashboardService(GmaoDbContext context) { _context = context; }

    public async Task<DashboardStats> GetStatsAsync()
    {
        var equipements = await _context.Equipements.ToListAsync();
        var interventions = await _context.Interventions.Include(i => i.Equipement).Include(i => i.PiecesDetachees).ToListAsync();
        var techniciens = await _context.Techniciens.ToListAsync();

        var stats = new DashboardStats
        {
            TotalEquipements = equipements.Count,
            EquipementsOperationnels = equipements.Count(e => e.Statut == "Opérationnel"),
            EquipementsEnPanne = equipements.Count(e => e.Statut == "En panne"),
            EquipementsEnMaintenance = equipements.Count(e => e.Statut == "En maintenance"),
            TotalInterventions = interventions.Count,
            InterventionsOuvertes = interventions.Count(i => i.Statut == "Ouverte"),
            InterventionsEnCours = interventions.Count(i => i.Statut == "En cours"),
            InterventionsResolues = interventions.Count(i => i.Statut == "Résolue" || i.Statut == "Clôturée"),
            TotalTechniciens = techniciens.Count,
            TechniciensDisponibles = techniciens.Count(t => t.EstDisponible),
            CoutTotalInterventions = interventions.Sum(i => i.CoutHeuresTotalTechnicien + i.PiecesDetachees.Sum(p => p.Quantite * p.PrixUnitaire)),
        };

        stats.TauxResolution = stats.TotalInterventions > 0
            ? Math.Round((double)stats.InterventionsResolues / stats.TotalInterventions * 100, 1) : 0;

        var depuis12Mois = DateTime.Now.AddMonths(-11);
        stats.InterventionsParMois = interventions
            .Where(i => i.DateCreation >= depuis12Mois)
            .GroupBy(i => new { i.DateCreation.Year, i.DateCreation.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new InterventionParMois
            {
                Mois = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yy"),
                Nombre = g.Count(),
                Cout = g.Sum(i => i.CoutHeuresTotalTechnicien + i.PiecesDetachees.Sum(p => p.Quantite * p.PrixUnitaire))
            }).ToList();

        stats.InterventionsParPriorite = interventions
            .GroupBy(i => i.Priorite)
            .Select(g => new InterventionParPriorite { Priorite = g.Key, Nombre = g.Count() })
            .OrderByDescending(x => x.Nombre).ToList();

        stats.Top5EquipementsPannes = interventions
            .GroupBy(i => i.Equipement.Nom)
            .Select(g => new EquipementTauxPanne
            {
                NomEquipement = g.Key,
                NombreInterventions = g.Count(),
                CoutTotal = g.Sum(i => i.CoutHeuresTotalTechnicien + i.PiecesDetachees.Sum(p => p.Quantite * p.PrixUnitaire))
            }).OrderByDescending(x => x.NombreInterventions).Take(5).ToList();

        stats.DernieresInterventions = interventions.OrderByDescending(i => i.DateCreation).Take(5).ToList();
        return stats;
    }
}