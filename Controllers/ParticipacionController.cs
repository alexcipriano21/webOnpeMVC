using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using webOnpeMVC.Models;

namespace webOnpeMVC.Controllers
{
    public class ParticipacionController : Controller
    {
        private readonly string _connectionString;

        private const int InicioDeptNacional = 1;
        private const int FinDeptNacional = 25;
        private const int InicioDeptExtranjero = 26;
        private const int FinDeptExtranjero = 30;

        public ParticipacionController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OnpeDB") ?? "";
        }

        private SqlConnection GetConnection() => new SqlConnection(_connectionString);

        public IActionResult Index()
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand("SELECT * FROM vTotalVotos", cn);
            cn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return View((TotalVotos?)null);
            var model = new TotalVotos
            {
                TotalAsistentes = reader["Total Asistentes"] != DBNull.Value ? Convert.ToInt64(reader["Total Asistentes"]) : 0,
                PctTotalAsistentes = reader["% Total Asistentes"]?.ToString() ?? "",
                TotalAusentes = reader["Total Ausentes"] != DBNull.Value ? Convert.ToInt64(reader["Total Ausentes"]) : 0,
                PctTotalAusentes = reader["% Total Ausentes"]?.ToString() ?? "",
                ElectoresHabiles = reader[4] != DBNull.Value ? Convert.ToInt64(reader[4]) : 0
            };
            return View(model);
        }


        public IActionResult Total(string ambito = "Nacional", int nivel = 0,
                                   string val = "", string padre = "", string abuelo = "")
        {
            var resultados = new List<ResultadoVotos>();

            using var cn = GetConnection();
            cn.Open();

            if (nivel == 0)
            {
                bool esExtranjero = ambito == "Extranjero";
                int inicio = esExtranjero ? InicioDeptExtranjero : InicioDeptNacional;
                int fin = esExtranjero ? FinDeptExtranjero : FinDeptNacional;
                using var cmd = new SqlCommand("usp_getVotos", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inicio", inicio);
                cmd.Parameters.AddWithValue("@fin", fin);
                resultados = LeerResultados(cmd);
            }
            else if (nivel == 1)
            {
                using var cmd = new SqlCommand("usp_getVotosDepartamento", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Departamento", val);
                resultados = LeerResultados(cmd);
            }
            else if (nivel == 2)
            {
                using var cmd = new SqlCommand("usp_getVotosProvincia", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Provincia", val);
                resultados = LeerResultados(cmd);
            }
            else if (nivel == 3)
            {
                using var cmd = new SqlCommand("usp_getVotosProvincia", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Provincia", padre);
                var todos = LeerResultados(cmd);
                resultados = todos
                    .Where(r => r.Nombre.Trim().Equals(val.Trim(), StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            bool esExt = ambito == "Extranjero";
            var model = new ParticipacionTotal
            {
                Resultados = resultados,
                Ambito = ambito,
                Nivel = nivel,
                Val = val,
                Padre = padre,
                Abuelo = abuelo,

                ColHeader = nivel == 0
                    ? (esExt ? "CONTINENTE" : "DEPARTAMENTO")
                    : nivel == 1
                        ? (esExt ? "PAÍS" : "PROVINCIA")
                        : (esExt ? "CIUDAD" : "DISTRITO"),

                Lbl1 = esExt ? "Continente" : "Departamento",
                Lbl2 = esExt ? "País" : "Provincia",
                Lbl3 = esExt ? "Ciudad" : "Distrito",

                N1Val = nivel == 1 ? val ?? "" : (nivel == 2 ? padre ?? "" : abuelo ?? ""),
                N2Val = nivel == 2 ? val ?? "" : (nivel == 3 ? padre ?? "" : ""),
                N3Val = nivel == 3 ? val ?? "" : ""
            };

            foreach (var r in resultados)
            {
                r.Url = Url.Action("Total", "Participacion", new
                {
                    ambito = ambito,
                    nivel = nivel + 1,
                    val = r.Nombre,
                    padre = nivel >= 1 ? val : "",
                    abuelo = nivel >= 2 ? padre : ""
                }) ?? "#";

                model.TotalTV += r.TotalVotantes;
                model.TotalTA += r.TotalAusentes;
                model.TotalEH += r.ElectoresHabiles;
            }

            model.PctTVStr = model.TotalEH > 0
                ? Math.Round((double)model.TotalTV * 100.0 / model.TotalEH, 3).ToString("0.000", System.Globalization.CultureInfo.InvariantCulture)
                : "0.000";
            model.PctTAStr = model.TotalEH > 0
                ? Math.Round((double)model.TotalTA * 100.0 / model.TotalEH, 3).ToString("0.000", System.Globalization.CultureInfo.InvariantCulture)
                : "0.000";

            return View(model);
        }

        private List<ResultadoVotos> LeerResultados(SqlCommand cmd)
        {
            var lista = new List<ResultadoVotos>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(new ResultadoVotos
                {
                    Nombre = reader["DPD"].ToString()?.Trim() ?? "",
                    TotalVotantes = reader["TV"] != DBNull.Value ? Convert.ToInt32(reader["TV"]) : 0,
                    PctTotalVotantes = reader["PTV"].ToString() ?? "",
                    TotalAusentes = reader["TA"] != DBNull.Value ? Convert.ToInt32(reader["TA"]) : 0,
                    PctTotalAusentes = reader["PTA"].ToString() ?? "",
                    ElectoresHabiles = reader["EH"] != DBNull.Value ? Convert.ToInt32(reader["EH"]) : 0,
                });
            return lista;
        }
    }
}
