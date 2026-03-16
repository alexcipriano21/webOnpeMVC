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

            ViewBag.Ambito = ambito;
            ViewBag.Nivel = nivel;
            ViewBag.Val = val;
            ViewBag.Padre = padre;
            ViewBag.Abuelo = abuelo;

            return View(resultados);
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
