using Microsoft.EntityFrameworkCore;
using MiniGMAO.Data;
using MiniGMAO.Models;
using MiniGMAO.Services.Interfaces;

namespace MiniGMAO.Services.Implementations;

public class InterventionService : IInterventionService
{
    private readonly GmaoDbContext _context;
    public InterventionService(GmaoDbContext context) { _context = context; }

    public async Task<List<Intervention>> GetAllAsync(string? searchTerm = null, string? statut = null, string? priorite = null, int? equipementId = null, int? technicienId = null)
    {
        var query = _context.Interventions
            .Include(i => i.Equipement).Include(i => i.Technicien).Include(i => i.PiecesDetachees).AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(i => i.Description.Contains(searchTerm) || i.Equipement.Nom.Contains(searchTerm));
        if (!string.IsNullOrWhiteSpace(statut)) query = query.Where(i => i.Statut == statut);
        if (!string.IsNullOrWhiteSpace(priorite)) query = query.Where(i => i.Priorite == priorite);
        if (equipementId.HasValue) query = query.Where(i => i.EquipementId == equipementId);
        if (technicienId.HasValue) query = query.Where(i => i.TechnicienId == technicienId);
        return await query.OrderByDescending(i => i.DateCreation).ToListAsync();
    }

    public async Task<Intervention?> GetByIdAsync(int id)
        => await _context.Interventions
            .Include(i => i.Equipement).Include(i => i.Technicien).Include(i => i.PiecesDetachees)
            .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<Intervention> CreateAsync(Intervention intervention)
    {
        intervention.DateCreation = DateTime.Now;
        _context.Interventions.Add(intervention);
        var equip = await _context.Equipements.FindAsync(intervention.EquipementId);
        if (equip != null && intervention.Statut != "Résolue") equip.Statut = "En panne";
        await _context.SaveChangesAsync();
        return intervention;
    }

    public async Task<Intervention> UpdateAsync(Intervention intervention)
{
    var existing = await _context.Interventions.FindAsync(intervention.Id);
    if (existing == null) throw new KeyNotFoundException();

    existing.EquipementId              = intervention.EquipementId;
    existing.TechnicienId              = intervention.TechnicienId;
    existing.Description               = intervention.Description;
    existing.Priorite                  = intervention.Priorite;
    existing.Statut                    = intervention.Statut;
    existing.TypeIntervention          = intervention.TypeIntervention;
    existing.Rapport                   = intervention.Rapport;
    existing.CoutHeuresTotalTechnicien = intervention.CoutHeuresTotalTechnicien;

    await _context.SaveChangesAsync();
    return existing;
}

    public async Task DeleteAsync(int id)
    {
        var i = await _context.Interventions.FindAsync(id) ?? throw new KeyNotFoundException();
        _context.Interventions.Remove(i);
        await _context.SaveChangesAsync();
    }

    public async Task<Intervention> ChangerStatutAsync(int id, string nouveauStatut)
    {
        var intervention = await _context.Interventions.Include(i => i.Equipement).FirstOrDefaultAsync(i => i.Id == id)
            ?? throw new KeyNotFoundException();
        intervention.Statut = nouveauStatut;
        if (nouveauStatut == "En cours" && intervention.DatePrisEnCharge == null) intervention.DatePrisEnCharge = DateTime.Now;
        if (nouveauStatut == "Résolue" && intervention.DateResolution == null)
        {
            intervention.DateResolution = DateTime.Now;
            if (intervention.Equipement != null) intervention.Equipement.Statut = "Opérationnel";
        }
        await _context.SaveChangesAsync();
        return intervention;
    }

    public async Task AddPieceDetacheeAsync(int interventionId, PieceDetachee piece)
    {
        piece.InterventionId = interventionId;
        _context.PiecesDetachees.Add(piece);
        await _context.SaveChangesAsync();
    }

    public async Task RemovePieceDetacheeAsync(int pieceId)
    {
        var p = await _context.PiecesDetachees.FindAsync(pieceId) ?? throw new KeyNotFoundException();
        _context.PiecesDetachees.Remove(p);
        await _context.SaveChangesAsync();
    }
}