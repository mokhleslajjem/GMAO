using System.ComponentModel.DataAnnotations;

namespace MiniGMAO.Models;

public class Intervention
{
    public int Id { get; set; }

    [Required(ErrorMessage = "L'équipement est obligatoire")]
    public int EquipementId { get; set; }

    public int? TechnicienId { get; set; }

    [Required(ErrorMessage = "La description est obligatoire")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Priorite { get; set; } = "Normale";

    public string Statut { get; set; } = "Ouverte";

    public string? TypeIntervention { get; set; } = "Corrective";

    public DateTime DateCreation { get; set; } = DateTime.Now;

    public DateTime? DatePrisEnCharge { get; set; }

    public DateTime? DateResolution { get; set; }

    [StringLength(1000)]
    public string? Rapport { get; set; }

    [Range(0, double.MaxValue)]
    public decimal CoutHeuresTotalTechnicien { get; set; } = 0;

    public decimal CoutTotal => CoutHeuresTotalTechnicien + PiecesDetachees.Sum(p => p.CoutTotal);

    public Equipement Equipement { get; set; } = null!;
    public Technicien? Technicien { get; set; }
    public ICollection<PieceDetachee> PiecesDetachees { get; set; } = new List<PieceDetachee>();
}