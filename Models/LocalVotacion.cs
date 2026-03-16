namespace webOnpeMVC.Models
{
    public class LocalVotacion
    {
        public int IdLocalVotacion { get; set; }
        public int IdDistrito { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
    }
}

