using MiniGMAO.Models;

namespace MiniGMAO.Services.Interfaces;

public interface IEquipementService
{
    Task<List<Equipement>> GetAllAsync(string? searchTerm = null, string? statut = null, string? type = null);
    Task<Equipement?> GetByIdAsync(int id);
    Task<Equipement> CreateAsync(Equipement equipement);
    Task<Equipement> UpdateAsync(Equipement equipement);
    Task DeleteAsync(int id);
    Task<List<string>> GetTypesAsync();
}

public interface ITechnicienService
{
    Task<List<Technicien>> GetAllAsync(string? searchTerm = null, bool? disponible = null);
    Task<Technicien?> GetByIdAsync(int id);
    Task<Technicien> CreateAsync(Technicien technicien);
    Task<Technicien> UpdateAsync(Technicien technicien);
    Task DeleteAsync(int id);
    Task<List<Technicien>> GetDisponiblesAsync();
    Task<(bool success, string password)> CreateCompteAsync(int technicienId);
    Task DeleteCompteAsync(int technicienId);
}

public interface IInterventionService
{
    Task<List<Intervention>> GetAllAsync(string? searchTerm = null, string? statut = null, string? priorite = null, int? equipementId = null, int? technicienId = null);
    Task<Intervention?> GetByIdAsync(int id);
    Task<Intervention> CreateAsync(Intervention intervention);
    Task<Intervention> UpdateAsync(Intervention intervention);
    Task DeleteAsync(int id);
    Task<Intervention> ChangerStatutAsync(int id, string nouveauStatut);
    Task AddPieceDetacheeAsync(int interventionId, PieceDetachee piece);
    Task RemovePieceDetacheeAsync(int pieceId);
}

public interface IDashboardService
{
    Task<DashboardStats> GetStatsAsync();
}