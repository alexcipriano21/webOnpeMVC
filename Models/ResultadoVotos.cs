namespace webOnpeMVC.Models
{
    public class ResultadoVotos
    {
        public string Nombre { get; set; } = string.Empty;
        public int TotalVotantes { get; set; }
        public string PctTotalVotantes { get; set; } = string.Empty;
        public int TotalAusentes { get; set; }
        public string PctTotalAusentes { get; set; } = string.Empty;
        public int ElectoresHabiles { get; set; }
        public string Url { get; set; } = string.Empty;
    }
}

