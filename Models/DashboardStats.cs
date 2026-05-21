namespace MiniGMAO.Models;

public class DashboardStats
{
    public int TotalEquipements { get; set; }
    public int EquipementsEnPanne { get; set; }
    public int EquipementsEnMaintenance { get; set; }
    public int EquipementsOperationnels { get; set; }
    public int TotalInterventions { get; set; }
    public int InterventionsOuvertes { get; set; }
    public int InterventionsEnCours { get; set; }
    public int InterventionsResolues { get; set; }
    public int TotalTechniciens { get; set; }
    public int TechniciensDisponibles { get; set; }
    public decimal CoutTotalInterventions { get; set; }
    public double TauxResolution { get; set; }
    public List<InterventionParMois> InterventionsParMois { get; set; } = new();
    public List<InterventionParPriorite> InterventionsParPriorite { get; set; } = new();
    public List<EquipementTauxPanne> Top5EquipementsPannes { get; set; } = new();
    public List<Intervention> DernieresInterventions { get; set; } = new();
}

public class InterventionParMois
{
    public string Mois { get; set; } = string.Empty;
    public int Nombre { get; set; }
    public decimal Cout { get; set; }
}

public class InterventionParPriorite
{
    public string Priorite { get; set; } = string.Empty;
    public int Nombre { get; set; }
}

public class EquipementTauxPanne
{
    public string NomEquipement { get; set; } = string.Empty;
    public int NombreInterventions { get; set; }
    public decimal CoutTotal { get; set; }
}