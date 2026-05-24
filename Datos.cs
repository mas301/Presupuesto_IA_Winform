using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PresupuestoIA
{
    public sealed class TipoRecursoDto
    {
        public int EmpresaId { get; set; }
        public int TipoRecursoId { get; set; }
        public string TipoRecurso { get; set; }
        public bool Activo { get; set; }
    }

    public sealed class RecursoDto
    {
        public int EmpresaId { get; set; }
        public int RecursoId { get; set; }
        public int TipoRecursoId { get; set; }
        public string Recurso { get; set; }
        public int? UnidadId { get; set; }
        public int? TipoCalculoId { get; set; }
        public string Alias { get; set; }
        public decimal? Rendimiento { get; set; }
        public decimal? RendimientoEquipos { get; set; }
        public decimal? DiasDuracion { get; set; }
        public decimal? HorasJornal { get; set; }
        public bool Independiente { get; set; }
    }

    public sealed class UnidadDto
    {
        public int UnidadId { get; set; }
        public string Codigo { get; set; }
        public string Unidad { get; set; }
        public string Simbolo { get; set; }
    }

    public sealed class RecursoPresupuestoDto
    {
        public int EmpresaId { get; set; }
        public int PresupuestoId { get; set; }
        public string Alias { get; set; }
        public int RecursoxPresupuestoId { get; set; }
        public int? RecursoxPresupuestoPadreId { get; set; }
        public int Orden { get; set; }
        public int Nivel { get; set; }
        public int TipoRecursoId { get; set; }
        public int? RecursoId { get; set; }
        public int? UnidadId { get; set; }
        public int? TipoCalculoId { get; set; }
        public decimal? HorasJornal { get; set; }
        public decimal? Rendimiento { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? PesoUnitario { get; set; }
        public decimal? DiasDuracion { get; set; }
        public decimal? CantidadTotal { get; set; }
        public decimal? ValorUnitario { get; set; }
        public decimal? ValorTotal { get; set; }
        public bool Expandido { get; set; }
    }

    public sealed class RecursoPartidaDto
    {
        public int EmpresaId { get; set; }
        public int PartidaId { get; set; }
        public int RecursoId { get; set; }
        public int? TipoCalculoId { get; set; }
        public int? UnidadId { get; set; }
        public decimal? Rendimiento { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? PesoUnitario { get; set; }
        public decimal? DiasDuracion { get; set; }
        public decimal CantidadTotal { get; set; }
        public int Orden { get; set; }
    }

    public sealed class TipoCalculoDto
    {
        public int EmpresaId { get; set; }
        public int TipoCalculoId { get; set; }
        public string CodigoTipoCalculoId { get; set; }
        public string TipoCalculo { get; set; }
    }

    public static class Datos
    {
        private const string DefaultConnectionString = "Data Source=caleb.pe;Initial Catalog=dlk;User Id=sadlk;Password=Mauricio2004;TrustServerCertificate=True;Encrypt=True;";

        private static string connectionString = DefaultConnectionString;

        /// <summary>
        /// Cadena de conexion usada por todas las operaciones de datos.
        /// Configurable desde el host (ver <see cref="Configure(string)"/>).
        /// </summary>
        public static string ConnectionString
        {
            get { return connectionString; }
        }

        /// <summary>
        /// Permite a una aplicacion host configurar la cadena de conexion
        /// antes de usar la libreria o de abrir el formulario PresupuestoIA.
        /// </summary>
        public static void Configure(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("La cadena de conexion no puede estar vacia.", nameof(connectionString));
            Datos.connectionString = connectionString;
        }

        public static List<TipoRecursoDto> ObtenerTiposRecurso(int empresaId, bool activa)
        {
            const string sql = @"
SELECT EmpresaId, TipoRecursoId, TipoRecurso, Activo
FROM PreTipoRecurso
WHERE EmpresaId = @EmpresaId AND Activo = @Activo
ORDER BY OrdenVisual, TipoRecurso;";

            var resultado = new List<TipoRecursoDto>();

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                    command.Parameters.Add("@Activo", SqlDbType.Bit).Value = activa;

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
            }
            catch (SqlException ex)
            {
                throw new DataException("Error al consultar tipos de recurso.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new DataException("Error de operación al consultar tipos de recurso.", ex);
            }

            return resultado;
        }

        public static List<RecursoDto> ObtenerRecursos(int empresaId)
        {
            const string sql = @"
SELECT *
FROM PreRecurso
WHERE EmpresaId = @EmpresaId
ORDER BY Recurso;";

            var resultado = new List<RecursoDto>();

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        int empresaIdOrdinal = reader.GetOrdinal("EmpresaId");
                        int recursoIdOrdinal = reader.GetOrdinal("RecursoId");
                        int tipoRecursoIdOrdinal = reader.GetOrdinal("TipoRecursoId");
                        int recursoOrdinal = reader.GetOrdinal("Recurso");
                        int unidadIdOrdinal = reader.GetOrdinal("UnidadId");
                        int tipoCalculoIdOrdinal = TryGetOrdinal(reader, "TipoCalculoId");
                        int aliasOrdinal = TryGetOrdinal(reader, "Alias");
                        int rendimientoOrdinal = TryGetOrdinal(reader, "Rendimiento", "RendimientoManoObra");
                        int rendimientoEquiposOrdinal = TryGetOrdinal(reader, "RendimientoEquipos");
                        int diasDuracionOrdinal = reader.GetOrdinal("DiasDuracion");
                        int horasJornalOrdinal = TryGetOrdinal(reader, "HorasJornal");
                        int independienteOrdinal = TryGetOrdinal(reader, "Independiente");

                        while (reader.Read())
                        {
                            resultado.Add(new RecursoDto
                            {
                                EmpresaId = reader.GetInt32(empresaIdOrdinal),
                                RecursoId = reader.GetInt32(recursoIdOrdinal),
                                TipoRecursoId = reader.GetInt32(tipoRecursoIdOrdinal),
                                Recurso = reader.IsDBNull(recursoOrdinal) ? string.Empty : reader.GetString(recursoOrdinal),
                                UnidadId = reader.IsDBNull(unidadIdOrdinal) ? (int?)null : reader.GetInt32(unidadIdOrdinal),
                                TipoCalculoId = tipoCalculoIdOrdinal >= 0 && !reader.IsDBNull(tipoCalculoIdOrdinal)
                                    ? (int?)Convert.ToInt32(reader.GetValue(tipoCalculoIdOrdinal))
                                    : null,
                                Alias = aliasOrdinal >= 0 && !reader.IsDBNull(aliasOrdinal)
                                    ? Convert.ToString(reader.GetValue(aliasOrdinal))
                                    : string.Empty,
                                Rendimiento = rendimientoOrdinal >= 0 && !reader.IsDBNull(rendimientoOrdinal)
                                    ? (decimal?)Convert.ToDecimal(reader.GetValue(rendimientoOrdinal))
                                    : null,
                                RendimientoEquipos = rendimientoEquiposOrdinal >= 0 && !reader.IsDBNull(rendimientoEquiposOrdinal)
                                    ? (decimal?)Convert.ToDecimal(reader.GetValue(rendimientoEquiposOrdinal))
                                    : null,
                                DiasDuracion = reader.IsDBNull(diasDuracionOrdinal) ? (decimal?)null : reader.GetDecimal(diasDuracionOrdinal),
                                HorasJornal = horasJornalOrdinal >= 0 && !reader.IsDBNull(horasJornalOrdinal)
                                    ? (decimal?)Convert.ToDecimal(reader.GetValue(horasJornalOrdinal))
                                    : null,
                                Independiente = independienteOrdinal >= 0 && !reader.IsDBNull(independienteOrdinal)
                                    && Convert.ToBoolean(reader.GetValue(independienteOrdinal))
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DataException("Error al consultar recursos.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new DataException("Error de operación al consultar recursos.", ex);
            }

            return resultado;
        }

        public static List<UnidadDto> ObtenerUnidades()
        {
            const string sql = @"
SELECT UnidadId, Codigo, Unidad, Simbolo
FROM AlmUnidad
ORDER BY Unidad;";

            var resultado = new List<UnidadDto>();

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        int unidadIdOrdinal = reader.GetOrdinal("UnidadId");
                        int codigoOrdinal = TryGetOrdinal(reader, "Codigo");
                        int unidadOrdinal = TryGetOrdinal(reader, "Unidad");
                        int simboloOrdinal = TryGetOrdinal(reader, "Simbolo");

                        while (reader.Read())
                        {
                            resultado.Add(new UnidadDto
                            {
                                UnidadId = reader.GetInt32(unidadIdOrdinal),
                                Codigo = codigoOrdinal >= 0 && !reader.IsDBNull(codigoOrdinal)
                                    ? Convert.ToString(reader.GetValue(codigoOrdinal))
                                    : string.Empty,
                                Unidad = unidadOrdinal >= 0 && !reader.IsDBNull(unidadOrdinal)
                                    ? Convert.ToString(reader.GetValue(unidadOrdinal))
                                    : string.Empty,
                                Simbolo = simboloOrdinal >= 0 && !reader.IsDBNull(simboloOrdinal)
                                    ? Convert.ToString(reader.GetValue(simboloOrdinal))
                                    : string.Empty
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DataException("Error al consultar unidades.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new DataException("Error de operación al consultar unidades.", ex);
            }

            return resultado;
        }

        public static List<RecursoPresupuestoDto> ObtenerRecursosPresupuesto(int empresaId, int presupuestoId)
        {
            const string sql = @"
    SELECT EmpresaId, PresupuestoId, Alias, RecursoxPresupuestoId, RecursoxPresupuestoPadreId, Orden, Nivel, TipoRecursoId, RecursoId, UnidadId, TipoCalculoId, HorasJornal, Rendimiento, Cantidad, PesoUnitario, DiasDuracion, CantidadTotal, ValorUnitario, ValorTotal, Expandido
FROM PreRecursoxPresupuesto
    WHERE EmpresaId = @EmpresaId AND PresupuestoId = @PresupuestoId
ORDER BY Nivel, Orden, RecursoxPresupuestoId;";

            var resultado = new List<RecursoPresupuestoDto>();

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                    command.Parameters.Add("@PresupuestoId", SqlDbType.Int).Value = presupuestoId;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        int empresaIdOrdinal = reader.GetOrdinal("EmpresaId");
                        int presupuestoIdOrdinal = reader.GetOrdinal("PresupuestoId");
                        int aliasOrdinal = TryGetOrdinal(reader, "Alias");
                        int idOrdinal = reader.GetOrdinal("RecursoxPresupuestoId");
                        int parentIdOrdinal = reader.GetOrdinal("RecursoxPresupuestoPadreId");
                        int orderOrdinal = reader.GetOrdinal("Orden");
                        int levelOrdinal = reader.GetOrdinal("Nivel");
                        int resourceTypeIdOrdinal = reader.GetOrdinal("TipoRecursoId");
                        int resourceIdOrdinal = reader.GetOrdinal("RecursoId");
                        int unidadIdOrdinal = TryGetOrdinal(reader, "UnidadId");
                        int tipoCalculoIdOrdinal = reader.GetOrdinal("TipoCalculoId");
                        int horasJornalOrdinal = reader.GetOrdinal("HorasJornal");
                        int rendimientoOrdinal = reader.GetOrdinal("Rendimiento");
                        int cantidadOrdinal = reader.GetOrdinal("Cantidad");
                        int pesoUnitarioOrdinal = reader.GetOrdinal("PesoUnitario");
                        int diasDuracionOrdinal = reader.GetOrdinal("DiasDuracion");
                        int cantidadTotalOrdinal = reader.GetOrdinal("CantidadTotal");
                        int valorUnitarioOrdinal = reader.GetOrdinal("ValorUnitario");
                        int valorTotalOrdinal = reader.GetOrdinal("ValorTotal");
                        int expandidoOrdinal = TryGetOrdinal(reader, "Expandido");

                        while (reader.Read())
                        {
                            resultado.Add(new RecursoPresupuestoDto
                            {
                                EmpresaId = reader.GetInt32(empresaIdOrdinal),
                                PresupuestoId = reader.GetInt32(presupuestoIdOrdinal),
                                Alias = aliasOrdinal >= 0 && !reader.IsDBNull(aliasOrdinal)
                                    ? Convert.ToString(reader.GetValue(aliasOrdinal))
                                    : string.Empty,
                                RecursoxPresupuestoId = reader.GetInt32(idOrdinal),
                                RecursoxPresupuestoPadreId = reader.IsDBNull(parentIdOrdinal) ? (int?)null : reader.GetInt32(parentIdOrdinal),
                                Orden = reader.GetInt32(orderOrdinal),
                                Nivel = reader.GetInt32(levelOrdinal),
                                TipoRecursoId = reader.GetInt32(resourceTypeIdOrdinal),
                                RecursoId = reader.IsDBNull(resourceIdOrdinal) ? (int?)null : reader.GetInt32(resourceIdOrdinal),
                                UnidadId = unidadIdOrdinal >= 0 && !reader.IsDBNull(unidadIdOrdinal)
                                    ? (int?)reader.GetInt32(unidadIdOrdinal)
                                    : null,
                                TipoCalculoId = reader.IsDBNull(tipoCalculoIdOrdinal) ? (int?)null : reader.GetInt32(tipoCalculoIdOrdinal),
                                HorasJornal = reader.IsDBNull(horasJornalOrdinal) ? (decimal?)null : reader.GetDecimal(horasJornalOrdinal),
                                Rendimiento = reader.IsDBNull(rendimientoOrdinal) ? (decimal?)null : reader.GetDecimal(rendimientoOrdinal),
                                Cantidad = reader.IsDBNull(cantidadOrdinal) ? (decimal?)null : reader.GetDecimal(cantidadOrdinal),
                                PesoUnitario = reader.IsDBNull(pesoUnitarioOrdinal) ? (decimal?)null : reader.GetDecimal(pesoUnitarioOrdinal),
                                DiasDuracion = reader.IsDBNull(diasDuracionOrdinal) ? (decimal?)null : reader.GetDecimal(diasDuracionOrdinal),
                                CantidadTotal = reader.IsDBNull(cantidadTotalOrdinal) ? (decimal?)null : reader.GetDecimal(cantidadTotalOrdinal),
                                ValorUnitario = reader.IsDBNull(valorUnitarioOrdinal) ? (decimal?)null : reader.GetDecimal(valorUnitarioOrdinal),
                                ValorTotal = reader.IsDBNull(valorTotalOrdinal) ? (decimal?)null : reader.GetDecimal(valorTotalOrdinal),
                                Expandido = expandidoOrdinal >= 0 && !reader.IsDBNull(expandidoOrdinal) && reader.GetBoolean(expandidoOrdinal)
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DataException("Error al consultar items del presupuesto.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new DataException("Error de operación al consultar items del presupuesto.", ex);
            }

            return resultado;
        }

        public static List<TipoCalculoDto> ObtenerTiposCalculo(int empresaId)
        {
            const string sql = @"
SELECT *
FROM PreTipoCalculo
WHERE EmpresaId = @EmpresaId;";

            var resultado = new List<TipoCalculoDto>();

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        int empresaIdOrdinal = TryGetOrdinal(reader, "EmpresaId");
                        int idOrdinal = reader.GetOrdinal("TipoCalculoId");
                        int codeOrdinal = TryGetOrdinal(reader, "CodigoTipoCalculoId", "CodigoTipoCalculo", "Codigo");
                        int nameOrdinal = TryGetOrdinal(reader, "TipoCalculo", "NombreTipoCalculo", "Descripcion", "Detalle");

                        while (reader.Read())
                        {
                            string codigo = codeOrdinal >= 0 && !reader.IsDBNull(codeOrdinal)
                                ? Convert.ToString(reader.GetValue(codeOrdinal))
                                : string.Empty;

                            string nombre = nameOrdinal >= 0 && !reader.IsDBNull(nameOrdinal)
                                ? Convert.ToString(reader.GetValue(nameOrdinal))
                                : codigo;

                            resultado.Add(new TipoCalculoDto
                            {
                                EmpresaId = empresaIdOrdinal >= 0 ? reader.GetInt32(empresaIdOrdinal) : 1,
                                TipoCalculoId = reader.GetInt32(idOrdinal),
                                CodigoTipoCalculoId = codigo,
                                TipoCalculo = nombre
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DataException("Error al consultar tipos de calculo.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new DataException("Error de operación al consultar tipos de calculo.", ex);
            }

            return resultado;
        }

        public static List<RecursoPartidaDto> ObtenerRecursosPartida(int empresaId, int partidaId)
        {
            const string sql = @"
SELECT EmpresaId, PartidaId, RecursoId, TipoCalculoId, UnidadId, Rendimiento, Cantidad, PesoUnitario, DiasDuracion, CantidadTotal, Orden
FROM PreRecursoxPartida
WHERE EmpresaId = @EmpresaId AND PartidaId = @PartidaId
ORDER BY Orden;";

            var resultado = new List<RecursoPartidaDto>();

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                    command.Parameters.Add("@PartidaId", SqlDbType.Int).Value = partidaId;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        int empresaIdOrdinal = reader.GetOrdinal("EmpresaId");
                        int partidaIdOrdinal = reader.GetOrdinal("PartidaId");
                        int recursoIdOrdinal = reader.GetOrdinal("RecursoId");
                        int tipoCalculoIdOrdinal = reader.GetOrdinal("TipoCalculoId");
                        int unidadIdOrdinal = reader.GetOrdinal("UnidadId");
                        int rendimientoOrdinal = reader.GetOrdinal("Rendimiento");
                        int cantidadOrdinal = reader.GetOrdinal("Cantidad");
                        int pesoUnitarioOrdinal = reader.GetOrdinal("PesoUnitario");
                        int diasDuracionOrdinal = reader.GetOrdinal("DiasDuracion");
                        int cantidadTotalOrdinal = reader.GetOrdinal("CantidadTotal");
                        int ordenOrdinal = reader.GetOrdinal("Orden");

                        while (reader.Read())
                        {
                            resultado.Add(new RecursoPartidaDto
                            {
                                EmpresaId = reader.GetInt32(empresaIdOrdinal),
                                PartidaId = reader.GetInt32(partidaIdOrdinal),
                                RecursoId = reader.GetInt32(recursoIdOrdinal),
                                TipoCalculoId = reader.IsDBNull(tipoCalculoIdOrdinal) ? (int?)null : reader.GetInt32(tipoCalculoIdOrdinal),
                                UnidadId = reader.IsDBNull(unidadIdOrdinal) ? (int?)null : reader.GetInt32(unidadIdOrdinal),
                                Rendimiento = reader.IsDBNull(rendimientoOrdinal) ? (decimal?)null : reader.GetDecimal(rendimientoOrdinal),
                                Cantidad = reader.IsDBNull(cantidadOrdinal) ? (decimal?)null : reader.GetDecimal(cantidadOrdinal),
                                PesoUnitario = reader.IsDBNull(pesoUnitarioOrdinal) ? (decimal?)null : reader.GetDecimal(pesoUnitarioOrdinal),
                                DiasDuracion = reader.IsDBNull(diasDuracionOrdinal) ? (decimal?)null : reader.GetDecimal(diasDuracionOrdinal),
                                CantidadTotal = reader.IsDBNull(cantidadTotalOrdinal) ? 0m : reader.GetDecimal(cantidadTotalOrdinal),
                                Orden = reader.GetInt32(ordenOrdinal)
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DataException("Error al consultar recursos por partida.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new DataException("Error de operación al consultar recursos por partida.", ex);
            }

            return resultado;
        }

        public static bool ExisteRecursosPartida(int empresaId, int partidaId)
        {
            const string sql = @"
SELECT TOP 1 1
FROM PreRecursoxPartida
WHERE EmpresaId = @EmpresaId AND PartidaId = @PartidaId;";

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                    command.Parameters.Add("@PartidaId", SqlDbType.Int).Value = partidaId;

                    connection.Open();
                    object value = command.ExecuteScalar();
                    return value != null && value != DBNull.Value;
                }
            }
            catch (SqlException ex)
            {
                throw new DataException("Error al validar relacion de recursos por partida.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new DataException("Error de operación al validar relacion de recursos por partida.", ex);
            }
        }

        public static decimal? ObtenerHorasJornalPresupuesto(int empresaId, int presupuestoId)
        {
            const string sql = @"
DECLARE @ColumnName sysname;
SELECT TOP 1 @ColumnName = c.name
FROM sys.columns c
WHERE c.object_id = OBJECT_ID('PrePresupuesto')
AND c.name IN ('HorasJornal', 'HorasJornada', 'Horas');

IF @ColumnName IS NULL
BEGIN
    SELECT CAST(NULL AS decimal(18, 5));
END
ELSE
BEGIN
    DECLARE @Sql nvarchar(max) = N'
        SELECT TOP 1 CAST([' + @ColumnName + N'] AS decimal(18, 5))
        FROM PrePresupuesto
        WHERE EmpresaId = @EmpresaId AND PresupuestoId = @PresupuestoId;';

    EXEC sp_executesql
        @Sql,
        N'@EmpresaId int, @PresupuestoId int',
        @EmpresaId = @EmpresaId,
        @PresupuestoId = @PresupuestoId;
END";

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                    command.Parameters.Add("@PresupuestoId", SqlDbType.Int).Value = presupuestoId;

                    connection.Open();
                    object value = command.ExecuteScalar();
                    if (value == null || value == DBNull.Value)
                        return null;

                    decimal parsed;
                    return decimal.TryParse(Convert.ToString(value), out parsed) ? (decimal?)parsed : null;
                }
            }
            catch (SqlException ex)
            {
                throw new DataException("Error al consultar Horas Jornal del presupuesto.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new DataException("Error de operación al consultar Horas Jornal del presupuesto.", ex);
            }
        }

        public static int CrearRecurso(
            int empresaId,
            int tipoRecursoId,
            string recurso,
            int? unidadId,
            int? tipoCalculoId,
            decimal? rendimiento,
            decimal? rendimientoEquipos,
            decimal? diasDuracion,
            decimal? horasJornal,
            bool independiente)
        {
            if (string.IsNullOrWhiteSpace(recurso))
                throw new ArgumentException("El nombre del recurso es obligatorio.", "recurso");

            const string findExistingSql = @"
SELECT TOP 1 RecursoId
FROM PreRecurso
WHERE EmpresaId = @EmpresaId AND TipoRecursoId = @TipoRecursoId AND Recurso = @Recurso;";

            const string identitySql = @"
SELECT COLUMNPROPERTY(OBJECT_ID('PreRecurso'), 'RecursoId', 'IsIdentity');";

                        const string insertIdentitySql = @"
DECLARE @HasTipoCalculo bit = CASE WHEN COL_LENGTH('PreRecurso', 'TipoCalculoId') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimiento bit = CASE WHEN COL_LENGTH('PreRecurso', 'Rendimiento') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimientoManoObra bit = CASE WHEN COL_LENGTH('PreRecurso', 'RendimientoManoObra') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimientoEquipos bit = CASE WHEN COL_LENGTH('PreRecurso', 'RendimientoEquipos') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasHorasJornal bit = CASE WHEN COL_LENGTH('PreRecurso', 'HorasJornal') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasIndependiente bit = CASE WHEN COL_LENGTH('PreRecurso', 'Independiente') IS NOT NULL THEN 1 ELSE 0 END;

DECLARE @Sql nvarchar(max) = N'
INSERT INTO PreRecurso (EmpresaId, TipoRecursoId, Recurso, UnidadId';

IF @HasTipoCalculo = 1
    SET @Sql += N', TipoCalculoId';

IF @HasRendimiento = 1
    SET @Sql += N', Rendimiento';

IF @HasRendimientoManoObra = 1
    SET @Sql += N', RendimientoManoObra';

IF @HasRendimientoEquipos = 1
    SET @Sql += N', RendimientoEquipos';

SET @Sql += N', DiasDuracion';

IF @HasHorasJornal = 1
    SET @Sql += N', HorasJornal';

IF @HasIndependiente = 1
    SET @Sql += N', Independiente';

SET @Sql += N') VALUES (@EmpresaId, @TipoRecursoId, @Recurso, @UnidadId';

IF @HasTipoCalculo = 1
    SET @Sql += N', @TipoCalculoId';

IF @HasRendimiento = 1
    SET @Sql += N', @Rendimiento';

IF @HasRendimientoManoObra = 1
    SET @Sql += N', @Rendimiento';

IF @HasRendimientoEquipos = 1
    SET @Sql += N', @RendimientoEquipos';

SET @Sql += N', @DiasDuracion';

IF @HasHorasJornal = 1
    SET @Sql += N', @HorasJornal';

IF @HasIndependiente = 1
    SET @Sql += N', @Independiente';

SET @Sql += N');
SELECT CAST(SCOPE_IDENTITY() AS int);';

EXEC sp_executesql
    @Sql,
    N'@EmpresaId int, @TipoRecursoId int, @Recurso nvarchar(250), @UnidadId int, @TipoCalculoId int, @Rendimiento decimal(18,5), @RendimientoEquipos decimal(18,5), @DiasDuracion decimal(18,5), @HorasJornal decimal(18,5), @Independiente bit',
    @EmpresaId = @EmpresaId,
    @TipoRecursoId = @TipoRecursoId,
    @Recurso = @Recurso,
    @UnidadId = @UnidadId,
    @TipoCalculoId = @TipoCalculoId,
    @Rendimiento = @Rendimiento,
    @RendimientoEquipos = @RendimientoEquipos,
    @DiasDuracion = @DiasDuracion,
    @HorasJornal = @HorasJornal,
    @Independiente = @Independiente;";

            const string nextIdSql = @"
SELECT ISNULL(MAX(RecursoId), 0) + 1
FROM PreRecurso
WHERE EmpresaId = @EmpresaId;";

                        const string insertRegularSql = @"
DECLARE @HasTipoCalculo bit = CASE WHEN COL_LENGTH('PreRecurso', 'TipoCalculoId') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimiento bit = CASE WHEN COL_LENGTH('PreRecurso', 'Rendimiento') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimientoManoObra bit = CASE WHEN COL_LENGTH('PreRecurso', 'RendimientoManoObra') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimientoEquipos bit = CASE WHEN COL_LENGTH('PreRecurso', 'RendimientoEquipos') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasHorasJornal bit = CASE WHEN COL_LENGTH('PreRecurso', 'HorasJornal') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasIndependiente bit = CASE WHEN COL_LENGTH('PreRecurso', 'Independiente') IS NOT NULL THEN 1 ELSE 0 END;

DECLARE @Sql nvarchar(max) = N'
INSERT INTO PreRecurso (EmpresaId, RecursoId, TipoRecursoId, Recurso, UnidadId';

IF @HasTipoCalculo = 1
    SET @Sql += N', TipoCalculoId';

IF @HasRendimiento = 1
    SET @Sql += N', Rendimiento';

IF @HasRendimientoManoObra = 1
    SET @Sql += N', RendimientoManoObra';

IF @HasRendimientoEquipos = 1
    SET @Sql += N', RendimientoEquipos';

SET @Sql += N', DiasDuracion';

IF @HasHorasJornal = 1
    SET @Sql += N', HorasJornal';

IF @HasIndependiente = 1
    SET @Sql += N', Independiente';

SET @Sql += N') VALUES (@EmpresaId, @RecursoId, @TipoRecursoId, @Recurso, @UnidadId';

IF @HasTipoCalculo = 1
    SET @Sql += N', @TipoCalculoId';

IF @HasRendimiento = 1
    SET @Sql += N', @Rendimiento';

IF @HasRendimientoManoObra = 1
    SET @Sql += N', @Rendimiento';

IF @HasRendimientoEquipos = 1
    SET @Sql += N', @RendimientoEquipos';

SET @Sql += N', @DiasDuracion';

IF @HasHorasJornal = 1
    SET @Sql += N', @HorasJornal';

IF @HasIndependiente = 1
    SET @Sql += N', @Independiente';

SET @Sql += N');';

EXEC sp_executesql
    @Sql,
    N'@EmpresaId int, @RecursoId int, @TipoRecursoId int, @Recurso nvarchar(250), @UnidadId int, @TipoCalculoId int, @Rendimiento decimal(18,5), @RendimientoEquipos decimal(18,5), @DiasDuracion decimal(18,5), @HorasJornal decimal(18,5), @Independiente bit',
    @EmpresaId = @EmpresaId,
    @RecursoId = @RecursoId,
    @TipoRecursoId = @TipoRecursoId,
    @Recurso = @Recurso,
    @UnidadId = @UnidadId,
    @TipoCalculoId = @TipoCalculoId,
    @Rendimiento = @Rendimiento,
    @RendimientoEquipos = @RendimientoEquipos,
    @DiasDuracion = @DiasDuracion,
    @HorasJornal = @HorasJornal,
    @Independiente = @Independiente;";

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        using (var findExistingCommand = new SqlCommand(findExistingSql, connection, transaction))
                        {
                            findExistingCommand.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                            findExistingCommand.Parameters.Add("@TipoRecursoId", SqlDbType.Int).Value = tipoRecursoId;
                            findExistingCommand.Parameters.Add("@Recurso", SqlDbType.NVarChar, 250).Value = recurso.Trim();
                            object existing = findExistingCommand.ExecuteScalar();
                            if (existing != null && existing != DBNull.Value)
                            {
                                int existingResourceId = Convert.ToInt32(existing);
                                ActualizarRecurso(
                                    empresaId,
                                    existingResourceId,
                                    tipoRecursoId,
                                    recurso,
                                    unidadId,
                                    tipoCalculoId,
                                    rendimiento,
                                    rendimientoEquipos,
                                    diasDuracion,
                                    horasJornal,
                                    independiente);
                                transaction.Commit();
                                return existingResourceId;
                            }
                        }

                        bool isIdentity;
                        using (var identityCommand = new SqlCommand(identitySql, connection, transaction))
                        {
                            object value = identityCommand.ExecuteScalar();
                            isIdentity = value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
                        }

                        int recursoId;
                        if (isIdentity)
                        {
                            using (var insertCommand = new SqlCommand(insertIdentitySql, connection, transaction))
                            {
                                insertCommand.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                                insertCommand.Parameters.Add("@TipoRecursoId", SqlDbType.Int).Value = tipoRecursoId;
                                insertCommand.Parameters.Add("@Recurso", SqlDbType.NVarChar, 250).Value = recurso.Trim();
                                insertCommand.Parameters.Add("@UnidadId", SqlDbType.Int).Value = unidadId.HasValue ? (object)unidadId.Value : DBNull.Value;
                                insertCommand.Parameters.Add("@TipoCalculoId", SqlDbType.Int).Value = tipoCalculoId.HasValue ? (object)tipoCalculoId.Value : DBNull.Value;

                                SqlParameter rendimientoParameter = insertCommand.Parameters.Add("@Rendimiento", SqlDbType.Decimal);
                                rendimientoParameter.Precision = 18;
                                rendimientoParameter.Scale = 5;
                                rendimientoParameter.Value = rendimiento.HasValue ? (object)rendimiento.Value : DBNull.Value;

                                SqlParameter rendimientoEquiposParameter = insertCommand.Parameters.Add("@RendimientoEquipos", SqlDbType.Decimal);
                                rendimientoEquiposParameter.Precision = 18;
                                rendimientoEquiposParameter.Scale = 5;
                                rendimientoEquiposParameter.Value = rendimientoEquipos.HasValue ? (object)rendimientoEquipos.Value : DBNull.Value;

                                SqlParameter diasDuracionParameter = insertCommand.Parameters.Add("@DiasDuracion", SqlDbType.Decimal);
                                diasDuracionParameter.Precision = 18;
                                diasDuracionParameter.Scale = 5;
                                diasDuracionParameter.Value = diasDuracion.HasValue ? (object)diasDuracion.Value : DBNull.Value;

                                SqlParameter horasJornalParameter = insertCommand.Parameters.Add("@HorasJornal", SqlDbType.Decimal);
                                horasJornalParameter.Precision = 18;
                                horasJornalParameter.Scale = 5;
                                horasJornalParameter.Value = horasJornal.HasValue ? (object)horasJornal.Value : DBNull.Value;

                                insertCommand.Parameters.Add("@Independiente", SqlDbType.Bit).Value = independiente;

                                recursoId = Convert.ToInt32(insertCommand.ExecuteScalar());
                            }
                        }
                        else
                        {
                            int nextId;
                            using (var nextIdCommand = new SqlCommand(nextIdSql, connection, transaction))
                            {
                                nextIdCommand.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                                nextId = Convert.ToInt32(nextIdCommand.ExecuteScalar());
                            }

                            using (var insertCommand = new SqlCommand(insertRegularSql, connection, transaction))
                            {
                                insertCommand.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                                insertCommand.Parameters.Add("@RecursoId", SqlDbType.Int).Value = nextId;
                                insertCommand.Parameters.Add("@TipoRecursoId", SqlDbType.Int).Value = tipoRecursoId;
                                insertCommand.Parameters.Add("@Recurso", SqlDbType.NVarChar, 250).Value = recurso.Trim();
                                insertCommand.Parameters.Add("@UnidadId", SqlDbType.Int).Value = unidadId.HasValue ? (object)unidadId.Value : DBNull.Value;
                                insertCommand.Parameters.Add("@TipoCalculoId", SqlDbType.Int).Value = tipoCalculoId.HasValue ? (object)tipoCalculoId.Value : DBNull.Value;

                                SqlParameter rendimientoParameter = insertCommand.Parameters.Add("@Rendimiento", SqlDbType.Decimal);
                                rendimientoParameter.Precision = 18;
                                rendimientoParameter.Scale = 5;
                                rendimientoParameter.Value = rendimiento.HasValue ? (object)rendimiento.Value : DBNull.Value;

                                SqlParameter rendimientoEquiposParameter = insertCommand.Parameters.Add("@RendimientoEquipos", SqlDbType.Decimal);
                                rendimientoEquiposParameter.Precision = 18;
                                rendimientoEquiposParameter.Scale = 5;
                                rendimientoEquiposParameter.Value = rendimientoEquipos.HasValue ? (object)rendimientoEquipos.Value : DBNull.Value;

                                SqlParameter diasDuracionParameter = insertCommand.Parameters.Add("@DiasDuracion", SqlDbType.Decimal);
                                diasDuracionParameter.Precision = 18;
                                diasDuracionParameter.Scale = 5;
                                diasDuracionParameter.Value = diasDuracion.HasValue ? (object)diasDuracion.Value : DBNull.Value;

                                SqlParameter horasJornalParameter = insertCommand.Parameters.Add("@HorasJornal", SqlDbType.Decimal);
                                horasJornalParameter.Precision = 18;
                                horasJornalParameter.Scale = 5;
                                horasJornalParameter.Value = horasJornal.HasValue ? (object)horasJornal.Value : DBNull.Value;

                                insertCommand.Parameters.Add("@Independiente", SqlDbType.Bit).Value = independiente;

                                insertCommand.ExecuteNonQuery();
                            }

                            recursoId = nextId;
                        }

                        transaction.Commit();
                        return recursoId;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DataException("Error al crear el recurso.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new DataException("Error de operación al crear el recurso.", ex);
            }
        }

        public static void ActualizarRecurso(
            int empresaId,
            int recursoId,
            int tipoRecursoId,
            string recurso,
            int? unidadId,
            int? tipoCalculoId,
            decimal? rendimiento,
            decimal? rendimientoEquipos,
            decimal? diasDuracion,
            decimal? horasJornal,
            bool independiente)
        {
            if (string.IsNullOrWhiteSpace(recurso))
                throw new ArgumentException("El nombre del recurso es obligatorio.", "recurso");

                        const string sql = @"
DECLARE @HasTipoCalculo bit = CASE WHEN COL_LENGTH('PreRecurso', 'TipoCalculoId') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimiento bit = CASE WHEN COL_LENGTH('PreRecurso', 'Rendimiento') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimientoManoObra bit = CASE WHEN COL_LENGTH('PreRecurso', 'RendimientoManoObra') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimientoEquipos bit = CASE WHEN COL_LENGTH('PreRecurso', 'RendimientoEquipos') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasHorasJornal bit = CASE WHEN COL_LENGTH('PreRecurso', 'HorasJornal') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasIndependiente bit = CASE WHEN COL_LENGTH('PreRecurso', 'Independiente') IS NOT NULL THEN 1 ELSE 0 END;

DECLARE @Sql nvarchar(max) = N'
UPDATE PreRecurso
SET TipoRecursoId = @TipoRecursoId,
    Recurso = @Recurso,
    UnidadId = @UnidadId';

IF @HasTipoCalculo = 1
    SET @Sql += N', TipoCalculoId = @TipoCalculoId';

IF @HasRendimiento = 1
    SET @Sql += N', Rendimiento = @Rendimiento';

IF @HasRendimientoManoObra = 1
    SET @Sql += N', RendimientoManoObra = @Rendimiento';

IF @HasRendimientoEquipos = 1
    SET @Sql += N', RendimientoEquipos = @RendimientoEquipos';

SET @Sql += N', DiasDuracion = @DiasDuracion';

IF @HasHorasJornal = 1
    SET @Sql += N', HorasJornal = @HorasJornal';

IF @HasIndependiente = 1
    SET @Sql += N', Independiente = @Independiente';

SET @Sql += N'
WHERE EmpresaId = @EmpresaId AND RecursoId = @RecursoId;';

EXEC sp_executesql
    @Sql,
    N'@EmpresaId int, @RecursoId int, @TipoRecursoId int, @Recurso nvarchar(250), @UnidadId int, @TipoCalculoId int, @Rendimiento decimal(18,5), @RendimientoEquipos decimal(18,5), @DiasDuracion decimal(18,5), @HorasJornal decimal(18,5), @Independiente bit',
    @EmpresaId = @EmpresaId,
    @RecursoId = @RecursoId,
    @TipoRecursoId = @TipoRecursoId,
    @Recurso = @Recurso,
    @UnidadId = @UnidadId,
    @TipoCalculoId = @TipoCalculoId,
    @Rendimiento = @Rendimiento,
    @RendimientoEquipos = @RendimientoEquipos,
    @DiasDuracion = @DiasDuracion,
    @HorasJornal = @HorasJornal,
    @Independiente = @Independiente;";

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                    command.Parameters.Add("@RecursoId", SqlDbType.Int).Value = recursoId;
                    command.Parameters.Add("@TipoRecursoId", SqlDbType.Int).Value = tipoRecursoId;
                    command.Parameters.Add("@Recurso", SqlDbType.NVarChar, 250).Value = recurso.Trim();
                    command.Parameters.Add("@UnidadId", SqlDbType.Int).Value = unidadId.HasValue ? (object)unidadId.Value : DBNull.Value;
                    command.Parameters.Add("@TipoCalculoId", SqlDbType.Int).Value = tipoCalculoId.HasValue ? (object)tipoCalculoId.Value : DBNull.Value;

                    SqlParameter rendimientoParameter = command.Parameters.Add("@Rendimiento", SqlDbType.Decimal);
                    rendimientoParameter.Precision = 18;
                    rendimientoParameter.Scale = 5;
                    rendimientoParameter.Value = rendimiento.HasValue ? (object)rendimiento.Value : DBNull.Value;

                    SqlParameter rendimientoEquiposParameter = command.Parameters.Add("@RendimientoEquipos", SqlDbType.Decimal);
                    rendimientoEquiposParameter.Precision = 18;
                    rendimientoEquiposParameter.Scale = 5;
                    rendimientoEquiposParameter.Value = rendimientoEquipos.HasValue ? (object)rendimientoEquipos.Value : DBNull.Value;

                    SqlParameter diasDuracionParameter = command.Parameters.Add("@DiasDuracion", SqlDbType.Decimal);
                    diasDuracionParameter.Precision = 18;
                    diasDuracionParameter.Scale = 5;
                    diasDuracionParameter.Value = diasDuracion.HasValue ? (object)diasDuracion.Value : DBNull.Value;

                    SqlParameter horasJornalParameter = command.Parameters.Add("@HorasJornal", SqlDbType.Decimal);
                    horasJornalParameter.Precision = 18;
                    horasJornalParameter.Scale = 5;
                    horasJornalParameter.Value = horasJornal.HasValue ? (object)horasJornal.Value : DBNull.Value;

                    command.Parameters.Add("@Independiente", SqlDbType.Bit).Value = independiente;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new DataException("Error al actualizar el recurso.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new DataException("Error de operación al actualizar el recurso.", ex);
            }
        }

        public static void GuardarRecursosPresupuesto(int empresaId, int presupuestoId, IList<RecursoPresupuestoDto> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            const string insertIdentitySql = @"
INSERT INTO PreRecursoxPresupuesto (EmpresaId, PresupuestoId, Alias, RecursoxPresupuestoPadreId, Orden, Nivel, TipoRecursoId, RecursoId, UnidadId, TipoCalculoId, HorasJornal, Rendimiento, Cantidad, PesoUnitario, DiasDuracion, CantidadTotal, ValorUnitario, ValorTotal, Expandido)
 VALUES (@EmpresaId, @PresupuestoId, @Alias, @PadreId, @Orden, @Nivel, @TipoRecursoId, @RecursoId, @UnidadId, @TipoCalculoId, @HorasJornal, @Rendimiento, @Cantidad, @PesoUnitario, @DiasDuracion, @CantidadTotal, @ValorUnitario, @ValorTotal, @Expandido);
 SELECT CAST(SCOPE_IDENTITY() AS int);";
            const string insertRegularSql = @"
INSERT INTO PreRecursoxPresupuesto (EmpresaId, PresupuestoId, RecursoxPresupuestoId, Alias, RecursoxPresupuestoPadreId, Orden, Nivel, TipoRecursoId, RecursoId, UnidadId, TipoCalculoId, HorasJornal, Rendimiento, Cantidad, PesoUnitario, DiasDuracion, CantidadTotal, ValorUnitario, ValorTotal, Expandido)
 VALUES (@EmpresaId, @PresupuestoId, @Id, @Alias, @PadreId, @Orden, @Nivel, @TipoRecursoId, @RecursoId, @UnidadId, @TipoCalculoId, @HorasJornal, @Rendimiento, @Cantidad, @PesoUnitario, @DiasDuracion, @CantidadTotal, @ValorUnitario, @ValorTotal, @Expandido);";
            const string updateSql = @"
 UPDATE PreRecursoxPresupuesto
 SET Alias = @Alias,
     RecursoxPresupuestoPadreId = @PadreId,
     Orden = @Orden,
     Nivel = @Nivel,
     TipoRecursoId = @TipoRecursoId,
     RecursoId = @RecursoId,
     UnidadId = @UnidadId,
     TipoCalculoId = @TipoCalculoId,
     HorasJornal = @HorasJornal,
     Rendimiento = @Rendimiento,
     Cantidad = @Cantidad,
     PesoUnitario = @PesoUnitario,
     DiasDuracion = @DiasDuracion,
     CantidadTotal = @CantidadTotal,
     ValorUnitario = @ValorUnitario,
     ValorTotal = @ValorTotal,
     Expandido = @Expandido
 WHERE EmpresaId = @EmpresaId AND PresupuestoId = @PresupuestoId AND RecursoxPresupuestoId = @Id;";
            const string deleteByIdSql = @"
DELETE FROM PreRecursoxPresupuesto
WHERE EmpresaId = @EmpresaId AND PresupuestoId = @PresupuestoId AND RecursoxPresupuestoId = @Id;";

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        bool isIdentity = IsBudgetTableIdentity(connection, transaction);

                        List<RecursoPresupuestoDto> existingRows = ObtenerRecursosPresupuesto(connection, transaction, empresaId, presupuestoId);
                        Dictionary<string, RecursoPresupuestoDto> existingByPath = BuildBudgetRowsByPath(existingRows);
                        Dictionary<int, string> incomingPathsByClientId = BuildBudgetPathsByClientId(items);

                        int nextId = 0;
                        if (!isIdentity)
                            nextId = ObtenerSiguienteRecursoPresupuestoId(connection, transaction, empresaId, presupuestoId);

                        var dbIdsByClientIds = new Dictionary<int, int>();
                        var usedDbIds = new HashSet<int>();

                        for (int i = 0; i < items.Count; i++)
                        {
                            RecursoPresupuestoDto item = items[i];
                            object parentDbId = DBNull.Value;
                            if (item.RecursoxPresupuestoPadreId.HasValue)
                            {
                                int parentClientId = item.RecursoxPresupuestoPadreId.Value;
                                if (dbIdsByClientIds.ContainsKey(parentClientId))
                                    parentDbId = dbIdsByClientIds[parentClientId];
                            }

                            if (!incomingPathsByClientId.TryGetValue(item.RecursoxPresupuestoId, out string currentPath))
                                continue;

                            if (existingByPath.TryGetValue(currentPath, out RecursoPresupuestoDto existing))
                            {
                                int persistedId = existing.RecursoxPresupuestoId;
                                int? parentId = parentDbId == DBNull.Value ? (int?)null : Convert.ToInt32(parentDbId);

                                if (HasBudgetRowChanges(existing, item, parentId))
                                {
                                    using (var updateCommand = new SqlCommand(updateSql, connection, transaction))
                                    {
                                        updateCommand.Parameters.Add("@Id", SqlDbType.Int).Value = persistedId;
                                        updateCommand.Parameters.Add("@PadreId", SqlDbType.Int).Value = parentDbId;
                                        updateCommand.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                                        updateCommand.Parameters.Add("@PresupuestoId", SqlDbType.Int).Value = presupuestoId;
                                        updateCommand.Parameters.Add("@Alias", SqlDbType.NVarChar, 200).Value = item.Alias ?? string.Empty;
                                        updateCommand.Parameters.Add("@Orden", SqlDbType.Int).Value = item.Orden;
                                        updateCommand.Parameters.Add("@Nivel", SqlDbType.Int).Value = item.Nivel;
                                        updateCommand.Parameters.Add("@TipoRecursoId", SqlDbType.Int).Value = item.TipoRecursoId;
                                        updateCommand.Parameters.Add("@RecursoId", SqlDbType.Int).Value = item.RecursoId.HasValue ? (object)item.RecursoId.Value : DBNull.Value;
                                        updateCommand.Parameters.Add("@UnidadId", SqlDbType.Int).Value = item.UnidadId.HasValue ? (object)item.UnidadId.Value : DBNull.Value;
                                        updateCommand.Parameters.Add("@TipoCalculoId", SqlDbType.Int).Value = item.TipoCalculoId.HasValue ? (object)item.TipoCalculoId.Value : DBNull.Value;
                                        updateCommand.Parameters.Add("@HorasJornal", SqlDbType.Decimal).Value = item.HorasJornal.HasValue ? (object)item.HorasJornal.Value : DBNull.Value;
                                        updateCommand.Parameters.Add("@Rendimiento", SqlDbType.Decimal).Value = item.Rendimiento.HasValue ? (object)item.Rendimiento.Value : DBNull.Value;
                                        updateCommand.Parameters.Add("@Cantidad", SqlDbType.Decimal).Value = item.Cantidad.HasValue ? (object)item.Cantidad.Value : DBNull.Value;
                                        updateCommand.Parameters.Add("@PesoUnitario", SqlDbType.Decimal).Value = item.PesoUnitario.HasValue ? (object)item.PesoUnitario.Value : DBNull.Value;
                                        updateCommand.Parameters.Add("@DiasDuracion", SqlDbType.Decimal).Value = item.DiasDuracion.HasValue ? (object)item.DiasDuracion.Value : DBNull.Value;
                                        updateCommand.Parameters.Add("@CantidadTotal", SqlDbType.Decimal).Value = item.CantidadTotal.HasValue ? (object)item.CantidadTotal.Value : DBNull.Value;
                                        updateCommand.Parameters.Add("@ValorUnitario", SqlDbType.Decimal).Value = item.ValorUnitario.HasValue ? (object)item.ValorUnitario.Value : DBNull.Value;
                                        updateCommand.Parameters.Add("@ValorTotal", SqlDbType.Decimal).Value = item.ValorTotal.HasValue ? (object)item.ValorTotal.Value : DBNull.Value;
                                        updateCommand.Parameters.Add("@Expandido", SqlDbType.Bit).Value = item.Expandido;
                                        updateCommand.ExecuteNonQuery();
                                    }
                                }

                                dbIdsByClientIds[item.RecursoxPresupuestoId] = persistedId;
                                usedDbIds.Add(persistedId);
                                continue;
                            }

                            int newPersistedId;
                            if (isIdentity)
                            {
                                using (var insertCommand = new SqlCommand(insertIdentitySql, connection, transaction))
                                {
                                    insertCommand.Parameters.Add("@PadreId", SqlDbType.Int).Value = parentDbId;
                                    insertCommand.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                                    insertCommand.Parameters.Add("@PresupuestoId", SqlDbType.Int).Value = presupuestoId;
                                    insertCommand.Parameters.Add("@Alias", SqlDbType.NVarChar, 200).Value = item.Alias ?? string.Empty;
                                    insertCommand.Parameters.Add("@Orden", SqlDbType.Int).Value = item.Orden;
                                    insertCommand.Parameters.Add("@Nivel", SqlDbType.Int).Value = item.Nivel;
                                    insertCommand.Parameters.Add("@TipoRecursoId", SqlDbType.Int).Value = item.TipoRecursoId;
                                    insertCommand.Parameters.Add("@RecursoId", SqlDbType.Int).Value = item.RecursoId.HasValue ? (object)item.RecursoId.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@UnidadId", SqlDbType.Int).Value = item.UnidadId.HasValue ? (object)item.UnidadId.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@TipoCalculoId", SqlDbType.Int).Value = item.TipoCalculoId.HasValue ? (object)item.TipoCalculoId.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@HorasJornal", SqlDbType.Decimal).Value = item.HorasJornal.HasValue ? (object)item.HorasJornal.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@Rendimiento", SqlDbType.Decimal).Value = item.Rendimiento.HasValue ? (object)item.Rendimiento.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@Cantidad", SqlDbType.Decimal).Value = item.Cantidad.HasValue ? (object)item.Cantidad.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@PesoUnitario", SqlDbType.Decimal).Value = item.PesoUnitario.HasValue ? (object)item.PesoUnitario.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@DiasDuracion", SqlDbType.Decimal).Value = item.DiasDuracion.HasValue ? (object)item.DiasDuracion.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@CantidadTotal", SqlDbType.Decimal).Value = item.CantidadTotal.HasValue ? (object)item.CantidadTotal.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@ValorUnitario", SqlDbType.Decimal).Value = item.ValorUnitario.HasValue ? (object)item.ValorUnitario.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@ValorTotal", SqlDbType.Decimal).Value = item.ValorTotal.HasValue ? (object)item.ValorTotal.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@Expandido", SqlDbType.Bit).Value = item.Expandido;
                                    newPersistedId = Convert.ToInt32(insertCommand.ExecuteScalar());
                                }
                            }
                            else
                            {
                                nextId++;
                                newPersistedId = nextId;
                                using (var insertCommand = new SqlCommand(insertRegularSql, connection, transaction))
                                {
                                    insertCommand.Parameters.Add("@Id", SqlDbType.Int).Value = newPersistedId;
                                    insertCommand.Parameters.Add("@PadreId", SqlDbType.Int).Value = parentDbId;
                                    insertCommand.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                                    insertCommand.Parameters.Add("@PresupuestoId", SqlDbType.Int).Value = presupuestoId;
                                    insertCommand.Parameters.Add("@Alias", SqlDbType.NVarChar, 200).Value = item.Alias ?? string.Empty;
                                    insertCommand.Parameters.Add("@Orden", SqlDbType.Int).Value = item.Orden;
                                    insertCommand.Parameters.Add("@Nivel", SqlDbType.Int).Value = item.Nivel;
                                    insertCommand.Parameters.Add("@TipoRecursoId", SqlDbType.Int).Value = item.TipoRecursoId;
                                    insertCommand.Parameters.Add("@RecursoId", SqlDbType.Int).Value = item.RecursoId.HasValue ? (object)item.RecursoId.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@UnidadId", SqlDbType.Int).Value = item.UnidadId.HasValue ? (object)item.UnidadId.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@TipoCalculoId", SqlDbType.Int).Value = item.TipoCalculoId.HasValue ? (object)item.TipoCalculoId.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@HorasJornal", SqlDbType.Decimal).Value = item.HorasJornal.HasValue ? (object)item.HorasJornal.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@Rendimiento", SqlDbType.Decimal).Value = item.Rendimiento.HasValue ? (object)item.Rendimiento.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@Cantidad", SqlDbType.Decimal).Value = item.Cantidad.HasValue ? (object)item.Cantidad.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@PesoUnitario", SqlDbType.Decimal).Value = item.PesoUnitario.HasValue ? (object)item.PesoUnitario.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@DiasDuracion", SqlDbType.Decimal).Value = item.DiasDuracion.HasValue ? (object)item.DiasDuracion.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@CantidadTotal", SqlDbType.Decimal).Value = item.CantidadTotal.HasValue ? (object)item.CantidadTotal.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@ValorUnitario", SqlDbType.Decimal).Value = item.ValorUnitario.HasValue ? (object)item.ValorUnitario.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@ValorTotal", SqlDbType.Decimal).Value = item.ValorTotal.HasValue ? (object)item.ValorTotal.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@Expandido", SqlDbType.Bit).Value = item.Expandido;
                                    insertCommand.ExecuteNonQuery();
                                }
                            }

                            dbIdsByClientIds[item.RecursoxPresupuestoId] = newPersistedId;
                            usedDbIds.Add(newPersistedId);
                        }

                        var rowsToDelete = new List<RecursoPresupuestoDto>();
                        for (int i = 0; i < existingRows.Count; i++)
                        {
                            RecursoPresupuestoDto existing = existingRows[i];
                            if (usedDbIds.Contains(existing.RecursoxPresupuestoId))
                                continue;
                            rowsToDelete.Add(existing);
                        }
                        // Borrar hijos antes que padres para respetar la FK auto-referenciada.
                        rowsToDelete.Sort((a, b) => b.Nivel.CompareTo(a.Nivel));
                        for (int i = 0; i < rowsToDelete.Count; i++)
                        {
                            RecursoPresupuestoDto existing = rowsToDelete[i];
                            using (var deleteCommand = new SqlCommand(deleteByIdSql, connection, transaction))
                            {
                                deleteCommand.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                                deleteCommand.Parameters.Add("@PresupuestoId", SqlDbType.Int).Value = presupuestoId;
                                deleteCommand.Parameters.Add("@Id", SqlDbType.Int).Value = existing.RecursoxPresupuestoId;
                                deleteCommand.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DataException("Error al guardar items del presupuesto.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new DataException("Error de operación al guardar items del presupuesto.", ex);
            }
        }

        public static void GuardarRecursosPartida(int empresaId, IList<RecursoPartidaDto> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            var filteredItems = new List<RecursoPartidaDto>();
            var partidaIds = new HashSet<int>();
            for (int i = 0; i < items.Count; i++)
            {
                RecursoPartidaDto item = items[i];
                if (item == null || item.PartidaId <= 0 || item.RecursoId <= 0)
                    continue;

                filteredItems.Add(item);
                partidaIds.Add(item.PartidaId);
            }

            if (partidaIds.Count == 0)
                return;

            const string insertSql = @"
INSERT INTO PreRecursoxPartida (EmpresaId, PartidaId, RecursoId, TipoCalculoId, UnidadId, Rendimiento, Cantidad, PesoUnitario, DiasDuracion, CantidadTotal, Orden)
VALUES (@EmpresaId, @PartidaId, @RecursoId, @TipoCalculoId, @UnidadId, @Rendimiento, @Cantidad, @PesoUnitario, @DiasDuracion, @CantidadTotal, @Orden);";
            const string updateSql = @"
UPDATE PreRecursoxPartida
SET RecursoId = @RecursoId,
    TipoCalculoId = @TipoCalculoId,
    UnidadId = @UnidadId,
    Rendimiento = @Rendimiento,
    Cantidad = @Cantidad,
    PesoUnitario = @PesoUnitario,
    DiasDuracion = @DiasDuracion,
    CantidadTotal = @CantidadTotal
WHERE RecursoxPartidaId = @RecursoxPartidaId;";
            const string deleteSql = @"
DELETE FROM PreRecursoxPartida
WHERE RecursoxPartidaId = @RecursoxPartidaId;";

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        List<RecursoPartidaStoredRow> existingRows = ObtenerRecursosPartida(connection, transaction, empresaId, partidaIds);
                        var existingByKey = new Dictionary<string, RecursoPartidaStoredRow>(StringComparer.Ordinal);
                        for (int i = 0; i < existingRows.Count; i++)
                        {
                            RecursoPartidaStoredRow row = existingRows[i];
                            string key = BuildPartidaKey(row.PartidaId, row.Orden);
                            existingByKey[key] = row;
                        }

                        var incomingByKey = new Dictionary<string, RecursoPartidaDto>(StringComparer.Ordinal);
                        for (int i = 0; i < filteredItems.Count; i++)
                        {
                            RecursoPartidaDto row = filteredItems[i];
                            string key = BuildPartidaKey(row.PartidaId, row.Orden);
                            incomingByKey[key] = row;
                        }

                        foreach (KeyValuePair<string, RecursoPartidaDto> entry in incomingByKey)
                        {
                            if (existingByKey.TryGetValue(entry.Key, out RecursoPartidaStoredRow existing))
                            {
                                if (!HasPartidaRowChanges(existing, entry.Value))
                                    continue;

                                using (var updateCommand = new SqlCommand(updateSql, connection, transaction))
                                {
                                    updateCommand.Parameters.Add("@RecursoxPartidaId", SqlDbType.Int).Value = existing.RecursoxPartidaId;
                                    updateCommand.Parameters.Add("@RecursoId", SqlDbType.Int).Value = entry.Value.RecursoId;
                                    updateCommand.Parameters.Add("@TipoCalculoId", SqlDbType.Int).Value = entry.Value.TipoCalculoId.HasValue ? (object)entry.Value.TipoCalculoId.Value : DBNull.Value;
                                    updateCommand.Parameters.Add("@UnidadId", SqlDbType.Int).Value = entry.Value.UnidadId.HasValue ? (object)entry.Value.UnidadId.Value : DBNull.Value;
                                    updateCommand.Parameters.Add("@Rendimiento", SqlDbType.Decimal).Value = entry.Value.Rendimiento.HasValue ? (object)entry.Value.Rendimiento.Value : DBNull.Value;
                                    updateCommand.Parameters.Add("@Cantidad", SqlDbType.Decimal).Value = entry.Value.Cantidad.HasValue ? (object)entry.Value.Cantidad.Value : DBNull.Value;
                                    updateCommand.Parameters.Add("@PesoUnitario", SqlDbType.Decimal).Value = entry.Value.PesoUnitario.HasValue ? (object)entry.Value.PesoUnitario.Value : DBNull.Value;
                                    updateCommand.Parameters.Add("@DiasDuracion", SqlDbType.Decimal).Value = entry.Value.DiasDuracion.HasValue ? (object)entry.Value.DiasDuracion.Value : DBNull.Value;
                                    updateCommand.Parameters.Add("@CantidadTotal", SqlDbType.Decimal).Value = entry.Value.CantidadTotal;
                                    updateCommand.ExecuteNonQuery();
                                }

                                continue;
                            }

                            using (var insertCommand = new SqlCommand(insertSql, connection, transaction))
                            {
                                insertCommand.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                                insertCommand.Parameters.Add("@PartidaId", SqlDbType.Int).Value = entry.Value.PartidaId;
                                insertCommand.Parameters.Add("@RecursoId", SqlDbType.Int).Value = entry.Value.RecursoId;
                                insertCommand.Parameters.Add("@TipoCalculoId", SqlDbType.Int).Value = entry.Value.TipoCalculoId.HasValue ? (object)entry.Value.TipoCalculoId.Value : DBNull.Value;
                                insertCommand.Parameters.Add("@UnidadId", SqlDbType.Int).Value = entry.Value.UnidadId.HasValue ? (object)entry.Value.UnidadId.Value : DBNull.Value;
                                insertCommand.Parameters.Add("@Rendimiento", SqlDbType.Decimal).Value = entry.Value.Rendimiento.HasValue ? (object)entry.Value.Rendimiento.Value : DBNull.Value;
                                insertCommand.Parameters.Add("@Cantidad", SqlDbType.Decimal).Value = entry.Value.Cantidad.HasValue ? (object)entry.Value.Cantidad.Value : DBNull.Value;
                                insertCommand.Parameters.Add("@PesoUnitario", SqlDbType.Decimal).Value = entry.Value.PesoUnitario.HasValue ? (object)entry.Value.PesoUnitario.Value : DBNull.Value;
                                insertCommand.Parameters.Add("@DiasDuracion", SqlDbType.Decimal).Value = entry.Value.DiasDuracion.HasValue ? (object)entry.Value.DiasDuracion.Value : DBNull.Value;
                                insertCommand.Parameters.Add("@CantidadTotal", SqlDbType.Decimal).Value = entry.Value.CantidadTotal;
                                insertCommand.Parameters.Add("@Orden", SqlDbType.Int).Value = entry.Value.Orden;
                                insertCommand.ExecuteNonQuery();
                            }
                        }

                        foreach (KeyValuePair<string, RecursoPartidaStoredRow> entry in existingByKey)
                        {
                            if (incomingByKey.ContainsKey(entry.Key))
                                continue;

                            using (var deleteCommand = new SqlCommand(deleteSql, connection, transaction))
                            {
                                deleteCommand.Parameters.Add("@RecursoxPartidaId", SqlDbType.Int).Value = entry.Value.RecursoxPartidaId;
                                deleteCommand.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DataException("Error al guardar relacion de recursos por partida.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new DataException("Error de operación al guardar relacion de recursos por partida.", ex);
            }
        }

        private static bool IsBudgetTableIdentity(SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = @"
SELECT COLUMNPROPERTY(OBJECT_ID('PreRecursoxPresupuesto'), 'RecursoxPresupuestoId', 'IsIdentity');";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                object value = command.ExecuteScalar();
                return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
            }
        }

        private sealed class RecursoPartidaStoredRow
        {
            public int RecursoxPartidaId { get; set; }
            public int PartidaId { get; set; }
            public int RecursoId { get; set; }
            public int? TipoCalculoId { get; set; }
            public int? UnidadId { get; set; }
            public decimal? Rendimiento { get; set; }
            public decimal? Cantidad { get; set; }
            public decimal? PesoUnitario { get; set; }
            public decimal? DiasDuracion { get; set; }
            public decimal CantidadTotal { get; set; }
            public int Orden { get; set; }
        }

        private static List<RecursoPresupuestoDto> ObtenerRecursosPresupuesto(SqlConnection connection, SqlTransaction transaction, int empresaId, int presupuestoId)
        {
            const string sql = @"
SELECT EmpresaId, PresupuestoId, Alias, RecursoxPresupuestoId, RecursoxPresupuestoPadreId, Orden, Nivel, TipoRecursoId, RecursoId, UnidadId, TipoCalculoId, HorasJornal, Rendimiento, DiasDuracion, CantidadTotal, ValorUnitario, ValorTotal, Expandido
FROM PreRecursoxPresupuesto
WHERE EmpresaId = @EmpresaId AND PresupuestoId = @PresupuestoId;";

            var result = new List<RecursoPresupuestoDto>();
            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                command.Parameters.Add("@PresupuestoId", SqlDbType.Int).Value = presupuestoId;

                using (var reader = command.ExecuteReader())
                {
                    int aliasOrdinal = TryGetOrdinal(reader, "Alias");
                    int idOrdinal = reader.GetOrdinal("RecursoxPresupuestoId");
                    int parentIdOrdinal = reader.GetOrdinal("RecursoxPresupuestoPadreId");
                    int orderOrdinal = reader.GetOrdinal("Orden");
                    int levelOrdinal = reader.GetOrdinal("Nivel");
                    int resourceTypeIdOrdinal = reader.GetOrdinal("TipoRecursoId");
                    int resourceIdOrdinal = reader.GetOrdinal("RecursoId");
                    int unidadIdOrdinal = TryGetOrdinal(reader, "UnidadId");
                    int tipoCalculoIdOrdinal = reader.GetOrdinal("TipoCalculoId");
                    int horasJornalOrdinal = reader.GetOrdinal("HorasJornal");
                    int rendimientoOrdinal = reader.GetOrdinal("Rendimiento");
                    int diasDuracionOrdinal = reader.GetOrdinal("DiasDuracion");
                    int cantidadTotalOrdinal = reader.GetOrdinal("CantidadTotal");
                    int valorUnitarioOrdinal = reader.GetOrdinal("ValorUnitario");
                    int valorTotalOrdinal = reader.GetOrdinal("ValorTotal");
                    int expandidoOrdinal = TryGetOrdinal(reader, "Expandido");

                    while (reader.Read())
                    {
                        result.Add(new RecursoPresupuestoDto
                        {
                            EmpresaId = empresaId,
                            PresupuestoId = presupuestoId,
                            Alias = aliasOrdinal >= 0 && !reader.IsDBNull(aliasOrdinal)
                                ? Convert.ToString(reader.GetValue(aliasOrdinal))
                                : string.Empty,
                            RecursoxPresupuestoId = reader.GetInt32(idOrdinal),
                            RecursoxPresupuestoPadreId = reader.IsDBNull(parentIdOrdinal) ? (int?)null : reader.GetInt32(parentIdOrdinal),
                            Orden = reader.GetInt32(orderOrdinal),
                            Nivel = reader.GetInt32(levelOrdinal),
                            TipoRecursoId = reader.GetInt32(resourceTypeIdOrdinal),
                            RecursoId = reader.IsDBNull(resourceIdOrdinal) ? (int?)null : reader.GetInt32(resourceIdOrdinal),
                            UnidadId = unidadIdOrdinal >= 0 && !reader.IsDBNull(unidadIdOrdinal)
                                ? (int?)reader.GetInt32(unidadIdOrdinal)
                                : null,
                            TipoCalculoId = reader.IsDBNull(tipoCalculoIdOrdinal) ? (int?)null : reader.GetInt32(tipoCalculoIdOrdinal),
                            HorasJornal = reader.IsDBNull(horasJornalOrdinal) ? (decimal?)null : reader.GetDecimal(horasJornalOrdinal),
                            Rendimiento = reader.IsDBNull(rendimientoOrdinal) ? (decimal?)null : reader.GetDecimal(rendimientoOrdinal),
                            DiasDuracion = reader.IsDBNull(diasDuracionOrdinal) ? (decimal?)null : reader.GetDecimal(diasDuracionOrdinal),
                            CantidadTotal = reader.IsDBNull(cantidadTotalOrdinal) ? (decimal?)null : reader.GetDecimal(cantidadTotalOrdinal),
                            ValorUnitario = reader.IsDBNull(valorUnitarioOrdinal) ? (decimal?)null : reader.GetDecimal(valorUnitarioOrdinal),
                            ValorTotal = reader.IsDBNull(valorTotalOrdinal) ? (decimal?)null : reader.GetDecimal(valorTotalOrdinal),
                            Expandido = expandidoOrdinal >= 0 && !reader.IsDBNull(expandidoOrdinal) && reader.GetBoolean(expandidoOrdinal)
                        });
                    }
                }
            }

            return result;
        }

        private static int ObtenerSiguienteRecursoPresupuestoId(SqlConnection connection, SqlTransaction transaction, int empresaId, int presupuestoId)
        {
            const string sql = @"
SELECT ISNULL(MAX(RecursoxPresupuestoId), 0)
FROM PreRecursoxPresupuesto
WHERE EmpresaId = @EmpresaId AND PresupuestoId = @PresupuestoId;";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                command.Parameters.Add("@PresupuestoId", SqlDbType.Int).Value = presupuestoId;
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private static Dictionary<string, RecursoPresupuestoDto> BuildBudgetRowsByPath(IList<RecursoPresupuestoDto> rows)
        {
            var rowsById = new Dictionary<int, RecursoPresupuestoDto>();
            var childrenByParentId = new Dictionary<int, List<RecursoPresupuestoDto>>();
            for (int i = 0; i < rows.Count; i++)
            {
                RecursoPresupuestoDto row = rows[i];
                rowsById[row.RecursoxPresupuestoId] = row;

                int parentId = row.RecursoxPresupuestoPadreId ?? 0;
                if (!childrenByParentId.TryGetValue(parentId, out List<RecursoPresupuestoDto> siblings))
                {
                    siblings = new List<RecursoPresupuestoDto>();
                    childrenByParentId[parentId] = siblings;
                }

                siblings.Add(row);
            }

            foreach (KeyValuePair<int, List<RecursoPresupuestoDto>> entry in childrenByParentId)
                entry.Value.Sort((a, b) => CompareByOrderThenId(a.Orden, b.Orden, a.RecursoxPresupuestoId, b.RecursoxPresupuestoId));

            var result = new Dictionary<string, RecursoPresupuestoDto>(StringComparer.Ordinal);
            BuildBudgetPathsRecursive(childrenByParentId, 0, string.Empty, result);
            return result;
        }

        private static Dictionary<int, string> BuildBudgetPathsByClientId(IList<RecursoPresupuestoDto> items)
        {
            var childrenByParentId = new Dictionary<int, List<RecursoPresupuestoDto>>();
            for (int i = 0; i < items.Count; i++)
            {
                RecursoPresupuestoDto item = items[i];
                int parentId = item.RecursoxPresupuestoPadreId ?? 0;
                if (!childrenByParentId.TryGetValue(parentId, out List<RecursoPresupuestoDto> siblings))
                {
                    siblings = new List<RecursoPresupuestoDto>();
                    childrenByParentId[parentId] = siblings;
                }

                siblings.Add(item);
            }

            foreach (KeyValuePair<int, List<RecursoPresupuestoDto>> entry in childrenByParentId)
                entry.Value.Sort((a, b) => CompareByOrderThenId(a.Orden, b.Orden, a.RecursoxPresupuestoId, b.RecursoxPresupuestoId));

            var result = new Dictionary<int, string>();
            BuildIncomingBudgetPathsRecursive(childrenByParentId, 0, string.Empty, result);
            return result;
        }

        private static void BuildBudgetPathsRecursive(
            IDictionary<int, List<RecursoPresupuestoDto>> childrenByParentId,
            int parentId,
            string parentPath,
            IDictionary<string, RecursoPresupuestoDto> result)
        {
            if (!childrenByParentId.TryGetValue(parentId, out List<RecursoPresupuestoDto> children))
                return;

            for (int i = 0; i < children.Count; i++)
            {
                RecursoPresupuestoDto child = children[i];
                string path = string.IsNullOrEmpty(parentPath)
                    ? child.Orden.ToString()
                    : parentPath + "." + child.Orden.ToString();

                result[path] = child;
                BuildBudgetPathsRecursive(childrenByParentId, child.RecursoxPresupuestoId, path, result);
            }
        }

        private static void BuildIncomingBudgetPathsRecursive(
            IDictionary<int, List<RecursoPresupuestoDto>> childrenByParentId,
            int parentId,
            string parentPath,
            IDictionary<int, string> result)
        {
            if (!childrenByParentId.TryGetValue(parentId, out List<RecursoPresupuestoDto> children))
                return;

            for (int i = 0; i < children.Count; i++)
            {
                RecursoPresupuestoDto child = children[i];
                string path = string.IsNullOrEmpty(parentPath)
                    ? child.Orden.ToString()
                    : parentPath + "." + child.Orden.ToString();

                result[child.RecursoxPresupuestoId] = path;
                BuildIncomingBudgetPathsRecursive(childrenByParentId, child.RecursoxPresupuestoId, path, result);
            }
        }

        private static int CompareByOrderThenId(int orderA, int orderB, int idA, int idB)
        {
            int orderCompare = orderA.CompareTo(orderB);
            if (orderCompare != 0)
                return orderCompare;

            return idA.CompareTo(idB);
        }

        private static bool HasBudgetRowChanges(RecursoPresupuestoDto existing, RecursoPresupuestoDto incoming, int? parentId)
        {
            return !string.Equals(existing.Alias ?? string.Empty, incoming.Alias ?? string.Empty, StringComparison.Ordinal)
                || existing.RecursoxPresupuestoPadreId != parentId
                || existing.Orden != incoming.Orden
                || existing.Nivel != incoming.Nivel
                || existing.TipoRecursoId != incoming.TipoRecursoId
                || existing.RecursoId != incoming.RecursoId
                || existing.UnidadId != incoming.UnidadId
                || existing.TipoCalculoId != incoming.TipoCalculoId
                || existing.HorasJornal != incoming.HorasJornal
                || existing.Rendimiento != incoming.Rendimiento
                || existing.DiasDuracion != incoming.DiasDuracion
                || existing.CantidadTotal != incoming.CantidadTotal
                || existing.ValorUnitario != incoming.ValorUnitario
                || existing.ValorTotal != incoming.ValorTotal
                || existing.Expandido != incoming.Expandido;
        }

        private static List<RecursoPartidaStoredRow> ObtenerRecursosPartida(SqlConnection connection, SqlTransaction transaction, int empresaId, HashSet<int> partidaIds)
        {
            var result = new List<RecursoPartidaStoredRow>();
            var ids = new List<int>(partidaIds);

            var sql = "SELECT RecursoxPartidaId, PartidaId, RecursoId, TipoCalculoId, UnidadId, Rendimiento, Cantidad, PesoUnitario, DiasDuracion, CantidadTotal, Orden FROM PreRecursoxPartida WHERE EmpresaId = @EmpresaId AND PartidaId IN (";
            for (int i = 0; i < ids.Count; i++)
            {
                if (i > 0)
                    sql += ",";

                sql += "@PartidaId" + i.ToString();
            }

            sql += ");";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                for (int i = 0; i < ids.Count; i++)
                    command.Parameters.Add("@PartidaId" + i.ToString(), SqlDbType.Int).Value = ids[i];

                using (var reader = command.ExecuteReader())
                {
                    int idOrdinal = reader.GetOrdinal("RecursoxPartidaId");
                    int partidaIdOrdinal = reader.GetOrdinal("PartidaId");
                    int recursoIdOrdinal = reader.GetOrdinal("RecursoId");
                    int tipoCalculoIdOrdinal = reader.GetOrdinal("TipoCalculoId");
                    int unidadIdOrdinal = reader.GetOrdinal("UnidadId");
                    int rendimientoOrdinal = reader.GetOrdinal("Rendimiento");
                    int cantidadOrdinal = reader.GetOrdinal("Cantidad");
                    int pesoUnitarioOrdinal = reader.GetOrdinal("PesoUnitario");
                    int diasDuracionOrdinal = reader.GetOrdinal("DiasDuracion");
                    int cantidadTotalOrdinal = reader.GetOrdinal("CantidadTotal");
                    int ordenOrdinal = reader.GetOrdinal("Orden");

                    while (reader.Read())
                    {
                        result.Add(new RecursoPartidaStoredRow
                        {
                            RecursoxPartidaId = reader.GetInt32(idOrdinal),
                            PartidaId = reader.GetInt32(partidaIdOrdinal),
                            RecursoId = reader.GetInt32(recursoIdOrdinal),
                            TipoCalculoId = reader.IsDBNull(tipoCalculoIdOrdinal) ? (int?)null : reader.GetInt32(tipoCalculoIdOrdinal),
                            UnidadId = reader.IsDBNull(unidadIdOrdinal) ? (int?)null : reader.GetInt32(unidadIdOrdinal),
                            Rendimiento = reader.IsDBNull(rendimientoOrdinal) ? (decimal?)null : reader.GetDecimal(rendimientoOrdinal),
                            Cantidad = reader.IsDBNull(cantidadOrdinal) ? (decimal?)null : reader.GetDecimal(cantidadOrdinal),
                            PesoUnitario = reader.IsDBNull(pesoUnitarioOrdinal) ? (decimal?)null : reader.GetDecimal(pesoUnitarioOrdinal),
                            DiasDuracion = reader.IsDBNull(diasDuracionOrdinal) ? (decimal?)null : reader.GetDecimal(diasDuracionOrdinal),
                            CantidadTotal = reader.GetDecimal(cantidadTotalOrdinal),
                            Orden = reader.GetInt32(ordenOrdinal)
                        });
                    }
                }
            }

            return result;
        }

        private static string BuildPartidaKey(int partidaId, int orden)
        {
            return partidaId.ToString() + "|" + orden.ToString();
        }

        private static bool HasPartidaRowChanges(RecursoPartidaStoredRow existing, RecursoPartidaDto incoming)
        {
            return existing.RecursoId != incoming.RecursoId
                || existing.TipoCalculoId != incoming.TipoCalculoId
                || existing.UnidadId != incoming.UnidadId
                || existing.Rendimiento != incoming.Rendimiento
                || existing.Cantidad != incoming.Cantidad
                || existing.PesoUnitario != incoming.PesoUnitario
                || existing.DiasDuracion != incoming.DiasDuracion
                || existing.CantidadTotal != incoming.CantidadTotal;
        }

        private static int TryGetOrdinal(SqlDataReader reader, params string[] columnNames)
        {
            for (int i = 0; i < columnNames.Length; i++)
            {
                try
                {
                    return reader.GetOrdinal(columnNames[i]);
                }
                catch (IndexOutOfRangeException)
                {
                }
            }

            return -1;
        }
    }
}