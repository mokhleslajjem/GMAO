using System.ComponentModel.DataAnnotations;

namespace MiniGMAO.Models;

public class Equipement
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [StringLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "La localisation est obligatoire")]
    [StringLength(200)]
    public string Localisation { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le type est obligatoire")]
    public string TypeEquipement { get; set; } = string.Empty;

    public string Statut { get; set; } = "Opérationnel";

    [DataType(DataType.Date)]
    public DateTime DateInstallation { get; set; } = DateTime.Today;

    public string? Description { get; set; }

    public ICollection<Intervention> Interventions { get; set; } = new List<Intervention>();
}