using System.Collections.Generic;

namespace webOnpeMVC.Models
{
    public class Ubigeo
    {
        public List<Departamento> Nacionales { get; set; } = new List<Departamento>();
        public List<Departamento> Extranjeros { get; set; } = new List<Departamento>();
    }
}
