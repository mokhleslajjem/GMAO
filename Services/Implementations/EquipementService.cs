using Microsoft.EntityFrameworkCore;
using MiniGMAO.Data;
using MiniGMAO.Models;
using MiniGMAO.Services.Interfaces;

namespace MiniGMAO.Services.Implementations;

public class EquipementService : IEquipementService
{
    private readonly GmaoDbContext _context;
    public EquipementService(GmaoDbContext context) { _context = context; }

    public async Task<List<Equipement>> GetAllAsync(string? searchTerm = null, string? statut = null, string? type = null)
    {
        var query = _context.Equipements.Include(e => e.Interventions).AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(e => e.Nom.Contains(searchTerm) || e.Localisation.Contains(searchTerm));
        if (!string.IsNullOrWhiteSpace(statut))
            query = query.Where(e => e.Statut == statut);
        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(e => e.TypeEquipement == type);
        return await query.OrderBy(e => e.Nom).ToListAsync();
    }

    public async Task<Equipement?> GetByIdAsync(int id)
        => await _context.Equipements
            .Include(e => e.Interventions).ThenInclude(i => i.Technicien)
            .Include(e => e.Interventions).ThenInclude(i => i.PiecesDetachees)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Equipement> CreateAsync(Equipement equipement)
    {
        _context.Equipements.Add(equipement);
        await _context.SaveChangesAsync();
        return equipement;
    }

        public async Task<Equipement> UpdateAsync(Equipement equipement)
    {
        var existing = await _context.Equipements.FindAsync(equipement.Id);
        if (existing == null) throw new KeyNotFoundException();

        existing.Nom              = equipement.Nom;
        existing.TypeEquipement   = equipement.TypeEquipement;
        existing.Localisation     = equipement.Localisation;
        existing.Statut           = equipement.Statut;
        existing.DateInstallation = equipement.DateInstallation;
        existing.Description      = equipement.Description;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _context.Equipements.FindAsync(id) ?? throw new KeyNotFoundException();
        _context.Equipements.Remove(e);
        await _context.SaveChangesAsync();
    }

    public async Task<List<string>> GetTypesAsync()
        => await _context.Equipements.Select(e => e.TypeEquipement).Distinct().OrderBy(t => t).ToListAsync();
}