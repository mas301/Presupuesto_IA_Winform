using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DevExpressTreeListDemo
{
    public sealed class TipoRecursoDto
    {
        public int EmpresaId { get; set; }
        public int TipoRecursoId { get; set; }
        public string TipoRecurso { get; set; }
        public bool Activo { get; set; }
    }

    public static class Datos
    {
        private const string ConnectionString = "Data Source=caleb.pe;Initial Catalog=dlk;User Id=sadlk;Password=Mauricio2004;TrustServerCertificate=True;Encrypt=True;";

        public static List<TipoRecursoDto> ObtenerTiposRecurso(int empresaId, bool activa)
        {
            const string sql = @"
SELECT EmpresaId, TipoRecursoId, TipoRecurso, Activo
FROM PreTipoRecurso
WHERE EmpresaId = @EmpresaId AND Activo = @Activo
ORDER BY OrdenVisual, TipoRecurso;";

            var resultado = new List<TipoRecursoDto>();

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@EmpresaId", empresaId);
                command.Parameters.AddWithValue("@Activo", activa ? 1 : 0);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultado.Add(new TipoRecursoDto
                        {
                            EmpresaId = reader.GetInt32(reader.GetOrdinal("EmpresaId")),
                            TipoRecursoId = reader.GetInt32(reader.GetOrdinal("TipoRecursoId")),
                            TipoRecurso = reader.GetString(reader.GetOrdinal("TipoRecurso")),
                            Activo = Convert.ToInt32(reader["Activo"]) == 1
                        });
                    }
                }
            }

            return resultado;
        }
    }
}