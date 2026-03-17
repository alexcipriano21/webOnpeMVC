using System;
using System.Collections.Generic;

namespace webOnpeMVC.Models
{
    public class ParticipacionTotal
    {
        public List<ResultadoVotos> Resultados { get; set; } = new List<ResultadoVotos>();

        public string Ambito { get; set; } = "Nacional";
        public int Nivel { get; set; } = 0;
        public string Val { get; set; } = "";
        public string Padre { get; set; } = "";
        public string Abuelo { get; set; } = "";

        public bool EsExtranjero => Ambito == "Extranjero";

        public string ColHeader { get; set; } = "";
        public string Lbl1 { get; set; } = "";
        public string Lbl2 { get; set; } = "";
        public string Lbl3 { get; set; } = "";

        public string N1Val { get; set; } = "";
        public string N2Val { get; set; } = "";
        public string N3Val { get; set; } = "";
        public List<string> DetallesAmbito
        {
            get
            {
                var detalles = new List<string>();
                if (Nivel >= 1) detalles.Add($"{Lbl1}: {N1Val}");
                if (Nivel >= 2) detalles.Add($"{Lbl2}: {N2Val}");
                if (Nivel >= 3) detalles.Add($"{Lbl3}: {N3Val}");
                return detalles;
            }
        }
        public int TotalTV { get; set; } = 0;
        public int TotalTA { get; set; } = 0;
        public int TotalEH { get; set; } = 0;

        public string PctTVStr { get; set; } = "0.000";
        public string PctTAStr { get; set; } = "0.000";

        public bool MostrarTabla => Nivel < 3 && Resultados != null && Resultados.Count > 0;
    }
}
