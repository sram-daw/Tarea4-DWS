using System.Data.SqlClient;

namespace RamiloAlonsoSaraTarea4.Models;




public class Conexion
{
    private readonly string _connectionString;

    public Conexion(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqlConnection ObtenerConexion()
    {
        var conexion = new SqlConnection(_connectionString);
        conexion.Open();
        return conexion;
    }

}

