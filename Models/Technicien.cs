using System.ComponentModel.DataAnnotations;

namespace MiniGMAO.Models;

public class Technicien
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [StringLength(50)]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est obligatoire")]
    [StringLength(50)]
    public string Prenom { get; set; } = string.Empty;

    [Required(ErrorMessage = "La spécialité est obligatoire")]
    public string Specialite { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email invalide")]
    public string? Email { get; set; }

    [Phone]
    public string? Telephone { get; set; }

    public bool EstDisponible { get; set; } = true;

    // Lien avec le compte Identity (null = pas encore de compte)
    public string? UserId { get; set; }

    public string NomComplet => $"{Prenom} {Nom}";

    public ICollection<Intervention> Interventions { get; set; } = new List<Intervention>();
}