using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using webOnpeMVC.Models;

namespace webOnpeMVC.Controllers
{
    public class ActasController : Controller
    {
        private readonly string _connectionString;

        private const string SqlDepartamentos = "SELECT idDepartamento, Detalle FROM Departamento ORDER BY Detalle";

        public ActasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OnpeDB") ?? "";
        }

        private SqlConnection GetConnection() => new SqlConnection(_connectionString);

        public IActionResult Ubigeo()
        {
            var lista = new List<Departamento>();
            using var cn = GetConnection();
            using var cmd = new SqlCommand(SqlDepartamentos, cn);
            cn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(new Departamento
                {
                    IdDepartamento = reader.GetInt32(0),
                    Detalle = reader.GetString(1).Trim()
                });
            return View(lista);
        }

        public IActionResult Numero()
        {
            return View();
        }
    }
}
