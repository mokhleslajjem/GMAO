using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniGMAO.Data;
using MiniGMAO.Models;
using MiniGMAO.Services.Interfaces;

namespace MiniGMAO.Services.Implementations;

public class TechnicienService : ITechnicienService
{
    private readonly GmaoDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public TechnicienService(GmaoDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<Technicien>> GetAllAsync(string? searchTerm = null, bool? disponible = null)
    {
        var query = _context.Techniciens.Include(t => t.Interventions).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(t =>
                t.Nom.Contains(searchTerm) ||
                t.Prenom.Contains(searchTerm) ||
                t.Specialite.Contains(searchTerm));

        if (disponible.HasValue)
            query = query.Where(t => t.EstDisponible == disponible.Value);

        return await query.OrderBy(t => t.Nom).ToListAsync();
    }

    public async Task<Technicien?> GetByIdAsync(int id)
        => await _context.Techniciens
            .Include(t => t.Interventions)
            .ThenInclude(i => i.Equipement)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<Technicien> CreateAsync(Technicien technicien)
    {
        _context.Techniciens.Add(technicien);
        await _context.SaveChangesAsync();
        return technicien;
    }

    public async Task<Technicien> UpdateAsync(Technicien technicien)
    {
        var existing = await _context.Techniciens.FindAsync(technicien.Id)
            ?? throw new KeyNotFoundException();

        var oldEmail = existing.Email;

        existing.Nom           = technicien.Nom;
        existing.Prenom        = technicien.Prenom;
        existing.Specialite    = technicien.Specialite;
        existing.Email         = technicien.Email;
        existing.Telephone     = technicien.Telephone;
        existing.EstDisponible = technicien.EstDisponible;

        // Si l'email a changé et que le technicien a un compte → mettre à jour le compte
        if (oldEmail != technicien.Email && existing.UserId != null)
        {
            var user = await _userManager.FindByIdAsync(existing.UserId);
            if (user != null && !string.IsNullOrEmpty(technicien.Email))
            {
                user.Email    = technicien.Email;
                user.UserName = technicien.Email;
                user.NormalizedEmail    = technicien.Email.ToUpperInvariant();
                user.NormalizedUserName = technicien.Email.ToUpperInvariant();
                await _userManager.UpdateAsync(user);
            }
        }

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var t = await _context.Techniciens.FindAsync(id)
            ?? throw new KeyNotFoundException();

        // Supprimer le compte Identity associé si existant
        if (t.UserId != null)
        {
            var user = await _userManager.FindByIdAsync(t.UserId);
            if (user != null)
                await _userManager.DeleteAsync(user);
        }

        _context.Techniciens.Remove(t);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Technicien>> GetDisponiblesAsync()
        => await _context.Techniciens
            .Where(t => t.EstDisponible)
            .OrderBy(t => t.Nom)
            .ToListAsync();

    /// <summary>
    /// Crée un compte Identity pour le technicien avec un mot de passe temporaire.
    /// Retourne (true, motDePasse) en cas de succès.
    /// </summary>
    public async Task<(bool success, string password)> CreateCompteAsync(int technicienId)
    {
        var technicien = await _context.Techniciens.FindAsync(technicienId)
            ?? throw new KeyNotFoundException("Technicien introuvable.");

        if (string.IsNullOrEmpty(technicien.Email))
            throw new InvalidOperationException("Le technicien n'a pas d'email. Veuillez en ajouter un avant de créer son compte.");

        // Vérifier si un compte existe déjà
        if (technicien.UserId != null)
            throw new InvalidOperationException("Ce technicien a déjà un compte.");

        var existingUser = await _userManager.FindByEmailAsync(technicien.Email);
        if (existingUser != null)
            throw new InvalidOperationException($"Un compte avec l'email {technicien.Email} existe déjà.");

        // Générer un mot de passe temporaire
        var password = GenererMotDePasse(technicien.Prenom);

        var user = new IdentityUser
        {
            UserName       = technicien.Email,
            Email          = technicien.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Erreur lors de la création du compte : {errors}");
        }

        // Assigner le rôle Technicien
        await _userManager.AddToRoleAsync(user, "Technicien");

        // Lier le compte au technicien
        technicien.UserId = user.Id;
        await _context.SaveChangesAsync();

        return (true, password);
    }

    /// <summary>
    /// Supprime le compte Identity du technicien (sans supprimer le technicien).
    /// </summary>
    public async Task DeleteCompteAsync(int technicienId)
    {
        var technicien = await _context.Techniciens.FindAsync(technicienId)
            ?? throw new KeyNotFoundException();

        if (technicien.UserId == null)
            throw new InvalidOperationException("Ce technicien n'a pas de compte.");

        var user = await _userManager.FindByIdAsync(technicien.UserId);
        if (user != null)
            await _userManager.DeleteAsync(user);

        technicien.UserId = null;
        await _context.SaveChangesAsync();
    }

    // Génère un mot de passe simple : Prénom + année + @Gmao
    // Ex: Karim2026@Gmao
    private static string GenererMotDePasse(string prenom)
    {
        var annee = DateTime.Now.Year;
        var prenomClean = new string(prenom.Where(char.IsLetter).ToArray());
        prenomClean = char.ToUpper(prenomClean[0]) + prenomClean[1..].ToLower();
        return $"{prenomClean}{annee}@Gmao";
    }
}