namespace webOnpeMVC.Models
{
    public class TotalVotos
    {
        public long TotalAsistentes { get; set; }
        public string PctTotalAsistentes { get; set; } = string.Empty;
        public long TotalAusentes { get; set; }
        public string PctTotalAusentes { get; set; } = string.Empty;
        public long ElectoresHabiles { get; set; }
    }
}

