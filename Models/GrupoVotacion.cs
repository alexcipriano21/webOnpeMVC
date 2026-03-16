namespace webOnpeMVC.Models
{
    public class GrupoVotacion
    {
        public string IdGrupoVotacion { get; set; } = string.Empty;
        public int IdLocalVotacion { get; set; }
        public string NCopia { get; set; } = string.Empty;
        public int IdEstadoActa { get; set; }
        public int ElectoresHabiles { get; set; }
        public int TotalVotantes { get; set; }
        public int P1 { get; set; }
        public int P2 { get; set; }
        public int VotosBlancos { get; set; }
        public int VotosNulos { get; set; }
        public int VotosImpugnados { get; set; }
        public string Departamento { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string Distrito { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;

        public string EstadoActaTexto =>
            IdEstadoActa == 1 ? "ACTA ELECTORAL NORMAL" : "ACTA ELECTORAL RESUELTA";
    }
}

