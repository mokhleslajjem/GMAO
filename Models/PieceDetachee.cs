using System.ComponentModel.DataAnnotations;

namespace MiniGMAO.Models;

public class PieceDetachee
{
    public int Id { get; set; }

    [Required]
    public int InterventionId { get; set; }

    [Required(ErrorMessage = "La référence est obligatoire")]
    [StringLength(50)]
    public string Reference { get; set; } = string.Empty;

    [Required(ErrorMessage = "La désignation est obligatoire")]
    [StringLength(200)]
    public string Designation { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être ≥ 1")]
    public int Quantite { get; set; } = 1;

    [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être positif")]
    public decimal PrixUnitaire { get; set; } = 0;

    public decimal CoutTotal => Quantite * PrixUnitaire;

    public Intervention Intervention { get; set; } = null!;
}