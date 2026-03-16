using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using webOnpeMVC.Models;

namespace webOnpeMVC.Controllers
{
    public class OnpeController : Controller
    {
        private readonly string _connectionString;

        public OnpeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OnpeDB") ?? "";
        }

        private SqlConnection GetConnection() => new SqlConnection(_connectionString);

        private List<T> EjecutarSP<T>(string nombreSP, string nombreParam, object valorParam, Func<SqlDataReader, T> mapear)
        {
            var lista = new List<T>();
            using var cn = GetConnection();
            using var cmd = new SqlCommand(nombreSP, cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue(nombreParam, valorParam);
            cn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(mapear(reader));
            return lista;
        }

        [HttpGet]
        public IActionResult GetProvincias(int idDepartamento)
        {
            var lista = EjecutarSP("usp_getProvincias", "@idDepartamento", idDepartamento,
                r => new Provincia { IdProvincia = r.GetInt32(0), Detalle = r.GetString(1).Trim() });
            return Json(lista);
        }

        [HttpGet]
        public IActionResult GetDistritos(int idProvincia)
        {
            var lista = EjecutarSP("usp_getDistritos", "@idProvincia", idProvincia,
                r => new Distrito { IdDistrito = r.GetInt32(0), Detalle = r.GetString(1).Trim() });
            return Json(lista);
        }

        [HttpGet]
        public IActionResult GetLocalesVotacion(int idDistrito)
        {
            var lista = EjecutarSP("usp_getLocalesVotacion", "@idDistrito", idDistrito,
                r => new LocalVotacion { IdLocalVotacion = r.GetInt32(0), RazonSocial = r.GetString(1).Trim() });
            return Json(lista);
        }

        [HttpGet]
        public IActionResult GetGruposVotacion(int idLocalVotacion)
        {
            var lista = EjecutarSP("usp_getGruposVotacion", "@idLocalVotacion", idLocalVotacion,
                r => r.GetString(0).Trim());
            return Json(lista);
        }

        [HttpGet]
        public IActionResult GetGrupoVotacion(string id)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand("usp_getGrupoVotacion", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@idGrupoVotacion", id);
            cn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return Json(new { error = "El número de mesa que ha ingresado no existe" });

            return Json(new GrupoVotacion
            {
                Departamento = reader["Departamento"].ToString()?.Trim() ?? "",
                Provincia = reader["Provincia"].ToString()?.Trim() ?? "",
                Distrito = reader["Distrito"].ToString()?.Trim() ?? "",
                RazonSocial = reader["RazonSocial"].ToString()?.Trim() ?? "",
                Direccion = reader["Direccion"].ToString()?.Trim() ?? "",
                IdGrupoVotacion = reader["idGrupoVotacion"].ToString()?.Trim() ?? "",
                NCopia = reader["nCopia"].ToString()?.Trim() ?? "",
                IdEstadoActa = Convert.ToInt32(reader["idEstadoActa"]),
                ElectoresHabiles = reader["ElectoresHabiles"] != DBNull.Value ? Convert.ToInt32(reader["ElectoresHabiles"]) : 0,
                TotalVotantes = reader["TotalVotantes"] != DBNull.Value ? Convert.ToInt32(reader["TotalVotantes"]) : 0,
                P1 = reader["P1"] != DBNull.Value ? Convert.ToInt32(reader["P1"]) : 0,
                P2 = reader["P2"] != DBNull.Value ? Convert.ToInt32(reader["P2"]) : 0,
                VotosBlancos = reader["VotosBlancos"] != DBNull.Value ? Convert.ToInt32(reader["VotosBlancos"]) : 0,
                VotosNulos = reader["VotosNulos"] != DBNull.Value ? Convert.ToInt32(reader["VotosNulos"]) : 0,
                VotosImpugnados = reader["VotosImpugnados"] != DBNull.Value ? Convert.ToInt32(reader["VotosImpugnados"]) : 0,
            });
        }
    }
}
