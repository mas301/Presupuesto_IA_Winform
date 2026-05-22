using System;
using System.Collections.Generic;
using System.Data;
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
        public decimal? Cuadrilla { get; set; }
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
        public decimal? Cuadrilla { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? ValorUnitario { get; set; }
        public decimal? ValorTotal { get; set; }
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
        private const string ConnectionString = "Data Source=caleb.pe;Initial Catalog=dlk;User Id=sadlk;Password=Mauricio2004;TrustServerCertificate=True;Encrypt=True;";

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
                        int cuadrillaOrdinal = TryGetOrdinal(reader, "Cuadrilla", "RendimientoEquipos");

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
                                Cuadrilla = cuadrillaOrdinal >= 0 && !reader.IsDBNull(cuadrillaOrdinal)
                                    ? (decimal?)Convert.ToDecimal(reader.GetValue(cuadrillaOrdinal))
                                    : null
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
    SELECT EmpresaId, PresupuestoId, Alias, RecursoxPresupuestoId, RecursoxPresupuestoPadreId, Orden, Nivel, TipoRecursoId, RecursoId, UnidadId, TipoCalculoId, HorasJornal, Rendimiento, Cuadrilla, Cantidad, ValorUnitario, ValorTotal
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
                        int cuadrillaOrdinal = reader.GetOrdinal("Cuadrilla");
                        int cantidadOrdinal = reader.GetOrdinal("Cantidad");
                        int valorUnitarioOrdinal = reader.GetOrdinal("ValorUnitario");
                        int valorTotalOrdinal = reader.GetOrdinal("ValorTotal");

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
                                Cuadrilla = reader.IsDBNull(cuadrillaOrdinal) ? (decimal?)null : reader.GetDecimal(cuadrillaOrdinal),
                                Cantidad = reader.IsDBNull(cantidadOrdinal) ? (decimal?)null : reader.GetDecimal(cantidadOrdinal),
                                ValorUnitario = reader.IsDBNull(valorUnitarioOrdinal) ? (decimal?)null : reader.GetDecimal(valorUnitarioOrdinal),
                                ValorTotal = reader.IsDBNull(valorTotalOrdinal) ? (decimal?)null : reader.GetDecimal(valorTotalOrdinal)
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
            decimal? cuadrilla)
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
DECLARE @HasCuadrilla bit = CASE WHEN COL_LENGTH('PreRecurso', 'Cuadrilla') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimientoEquipos bit = CASE WHEN COL_LENGTH('PreRecurso', 'RendimientoEquipos') IS NOT NULL THEN 1 ELSE 0 END;

DECLARE @Sql nvarchar(max) = N'
INSERT INTO PreRecurso (EmpresaId, TipoRecursoId, Recurso, UnidadId';

IF @HasTipoCalculo = 1
    SET @Sql += N', TipoCalculoId';

IF @HasRendimiento = 1
    SET @Sql += N', Rendimiento';

IF @HasRendimientoManoObra = 1
    SET @Sql += N', RendimientoManoObra';

IF @HasCuadrilla = 1
    SET @Sql += N', Cuadrilla';

IF @HasRendimientoEquipos = 1
    SET @Sql += N', RendimientoEquipos';

SET @Sql += N') VALUES (@EmpresaId, @TipoRecursoId, @Recurso, @UnidadId';

IF @HasTipoCalculo = 1
    SET @Sql += N', @TipoCalculoId';

IF @HasRendimiento = 1
    SET @Sql += N', @Rendimiento';

IF @HasRendimientoManoObra = 1
    SET @Sql += N', @Rendimiento';

IF @HasCuadrilla = 1
    SET @Sql += N', @Cuadrilla';

IF @HasRendimientoEquipos = 1
    SET @Sql += N', @Cuadrilla';

SET @Sql += N');
SELECT CAST(SCOPE_IDENTITY() AS int);';

EXEC sp_executesql
    @Sql,
    N'@EmpresaId int, @TipoRecursoId int, @Recurso nvarchar(250), @UnidadId int, @TipoCalculoId int, @Rendimiento decimal(18,5), @Cuadrilla decimal(18,5)',
    @EmpresaId = @EmpresaId,
    @TipoRecursoId = @TipoRecursoId,
    @Recurso = @Recurso,
    @UnidadId = @UnidadId,
    @TipoCalculoId = @TipoCalculoId,
    @Rendimiento = @Rendimiento,
    @Cuadrilla = @Cuadrilla;";

            const string nextIdSql = @"
SELECT ISNULL(MAX(RecursoId), 0) + 1
FROM PreRecurso
WHERE EmpresaId = @EmpresaId;";

                        const string insertRegularSql = @"
DECLARE @HasTipoCalculo bit = CASE WHEN COL_LENGTH('PreRecurso', 'TipoCalculoId') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimiento bit = CASE WHEN COL_LENGTH('PreRecurso', 'Rendimiento') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimientoManoObra bit = CASE WHEN COL_LENGTH('PreRecurso', 'RendimientoManoObra') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasCuadrilla bit = CASE WHEN COL_LENGTH('PreRecurso', 'Cuadrilla') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimientoEquipos bit = CASE WHEN COL_LENGTH('PreRecurso', 'RendimientoEquipos') IS NOT NULL THEN 1 ELSE 0 END;

DECLARE @Sql nvarchar(max) = N'
INSERT INTO PreRecurso (EmpresaId, RecursoId, TipoRecursoId, Recurso, UnidadId';

IF @HasTipoCalculo = 1
    SET @Sql += N', TipoCalculoId';

IF @HasRendimiento = 1
    SET @Sql += N', Rendimiento';

IF @HasRendimientoManoObra = 1
    SET @Sql += N', RendimientoManoObra';

IF @HasCuadrilla = 1
    SET @Sql += N', Cuadrilla';

IF @HasRendimientoEquipos = 1
    SET @Sql += N', RendimientoEquipos';

SET @Sql += N') VALUES (@EmpresaId, @RecursoId, @TipoRecursoId, @Recurso, @UnidadId';

IF @HasTipoCalculo = 1
    SET @Sql += N', @TipoCalculoId';

IF @HasRendimiento = 1
    SET @Sql += N', @Rendimiento';

IF @HasRendimientoManoObra = 1
    SET @Sql += N', @Rendimiento';

IF @HasCuadrilla = 1
    SET @Sql += N', @Cuadrilla';

IF @HasRendimientoEquipos = 1
    SET @Sql += N', @Cuadrilla';

SET @Sql += N');';

EXEC sp_executesql
    @Sql,
    N'@EmpresaId int, @RecursoId int, @TipoRecursoId int, @Recurso nvarchar(250), @UnidadId int, @TipoCalculoId int, @Rendimiento decimal(18,5), @Cuadrilla decimal(18,5)',
    @EmpresaId = @EmpresaId,
    @RecursoId = @RecursoId,
    @TipoRecursoId = @TipoRecursoId,
    @Recurso = @Recurso,
    @UnidadId = @UnidadId,
    @TipoCalculoId = @TipoCalculoId,
    @Rendimiento = @Rendimiento,
    @Cuadrilla = @Cuadrilla;";

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
                                    cuadrilla);
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

                                SqlParameter cuadrillaParameter = insertCommand.Parameters.Add("@Cuadrilla", SqlDbType.Decimal);
                                cuadrillaParameter.Precision = 18;
                                cuadrillaParameter.Scale = 5;
                                cuadrillaParameter.Value = cuadrilla.HasValue ? (object)cuadrilla.Value : DBNull.Value;

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

                                SqlParameter cuadrillaParameter = insertCommand.Parameters.Add("@Cuadrilla", SqlDbType.Decimal);
                                cuadrillaParameter.Precision = 18;
                                cuadrillaParameter.Scale = 5;
                                cuadrillaParameter.Value = cuadrilla.HasValue ? (object)cuadrilla.Value : DBNull.Value;

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
            decimal? cuadrilla)
        {
            if (string.IsNullOrWhiteSpace(recurso))
                throw new ArgumentException("El nombre del recurso es obligatorio.", "recurso");

                        const string sql = @"
DECLARE @HasTipoCalculo bit = CASE WHEN COL_LENGTH('PreRecurso', 'TipoCalculoId') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimiento bit = CASE WHEN COL_LENGTH('PreRecurso', 'Rendimiento') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimientoManoObra bit = CASE WHEN COL_LENGTH('PreRecurso', 'RendimientoManoObra') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasCuadrilla bit = CASE WHEN COL_LENGTH('PreRecurso', 'Cuadrilla') IS NOT NULL THEN 1 ELSE 0 END;
DECLARE @HasRendimientoEquipos bit = CASE WHEN COL_LENGTH('PreRecurso', 'RendimientoEquipos') IS NOT NULL THEN 1 ELSE 0 END;

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

IF @HasCuadrilla = 1
    SET @Sql += N', Cuadrilla = @Cuadrilla';

IF @HasRendimientoEquipos = 1
    SET @Sql += N', RendimientoEquipos = @Cuadrilla';

SET @Sql += N'
WHERE EmpresaId = @EmpresaId AND RecursoId = @RecursoId;';

EXEC sp_executesql
    @Sql,
    N'@EmpresaId int, @RecursoId int, @TipoRecursoId int, @Recurso nvarchar(250), @UnidadId int, @TipoCalculoId int, @Rendimiento decimal(18,5), @Cuadrilla decimal(18,5)',
    @EmpresaId = @EmpresaId,
    @RecursoId = @RecursoId,
    @TipoRecursoId = @TipoRecursoId,
    @Recurso = @Recurso,
    @UnidadId = @UnidadId,
    @TipoCalculoId = @TipoCalculoId,
    @Rendimiento = @Rendimiento,
    @Cuadrilla = @Cuadrilla;";

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

                    SqlParameter cuadrillaParameter = command.Parameters.Add("@Cuadrilla", SqlDbType.Decimal);
                    cuadrillaParameter.Precision = 18;
                    cuadrillaParameter.Scale = 5;
                    cuadrillaParameter.Value = cuadrilla.HasValue ? (object)cuadrilla.Value : DBNull.Value;

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

            const string deleteSql = "DELETE FROM PreRecursoxPresupuesto WHERE EmpresaId = @EmpresaId AND PresupuestoId = @PresupuestoId;";
            const string insertIdentitySql = @"
INSERT INTO PreRecursoxPresupuesto (EmpresaId, PresupuestoId, Alias, RecursoxPresupuestoPadreId, Orden, Nivel, TipoRecursoId, RecursoId, UnidadId, TipoCalculoId, HorasJornal, Rendimiento, Cuadrilla, Cantidad, ValorUnitario, ValorTotal)
VALUES (@EmpresaId, @PresupuestoId, @Alias, @PadreId, @Orden, @Nivel, @TipoRecursoId, @RecursoId, @UnidadId, @TipoCalculoId, @HorasJornal, @Rendimiento, @Cuadrilla, @Cantidad, @ValorUnitario, @ValorTotal);
SELECT CAST(SCOPE_IDENTITY() AS int);";
            const string insertRegularSql = @"
INSERT INTO PreRecursoxPresupuesto (EmpresaId, PresupuestoId, RecursoxPresupuestoId, Alias, RecursoxPresupuestoPadreId, Orden, Nivel, TipoRecursoId, RecursoId, UnidadId, TipoCalculoId, HorasJornal, Rendimiento, Cuadrilla, Cantidad, ValorUnitario, ValorTotal)
VALUES (@EmpresaId, @PresupuestoId, @Id, @Alias, @PadreId, @Orden, @Nivel, @TipoRecursoId, @RecursoId, @UnidadId, @TipoCalculoId, @HorasJornal, @Rendimiento, @Cuadrilla, @Cantidad, @ValorUnitario, @ValorTotal);";

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        bool isIdentity = IsBudgetTableIdentity(connection, transaction);

                        using (var deleteCommand = new SqlCommand(deleteSql, connection, transaction))
                        {
                            deleteCommand.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                            deleteCommand.Parameters.Add("@PresupuestoId", SqlDbType.Int).Value = presupuestoId;
                            deleteCommand.ExecuteNonQuery();
                        }

                        var dbIdsByClientIds = new Dictionary<int, int>();

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

                            int persistedId;
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
                                    insertCommand.Parameters.Add("@Cuadrilla", SqlDbType.Decimal).Value = item.Cuadrilla.HasValue ? (object)item.Cuadrilla.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@Cantidad", SqlDbType.Decimal).Value = item.Cantidad.HasValue ? (object)item.Cantidad.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@ValorUnitario", SqlDbType.Decimal).Value = item.ValorUnitario.HasValue ? (object)item.ValorUnitario.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@ValorTotal", SqlDbType.Decimal).Value = item.ValorTotal.HasValue ? (object)item.ValorTotal.Value : DBNull.Value;
                                    persistedId = Convert.ToInt32(insertCommand.ExecuteScalar());
                                }
                            }
                            else
                            {
                                using (var insertCommand = new SqlCommand(insertRegularSql, connection, transaction))
                                {
                                    insertCommand.Parameters.Add("@Id", SqlDbType.Int).Value = item.RecursoxPresupuestoId;
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
                                    insertCommand.Parameters.Add("@Cuadrilla", SqlDbType.Decimal).Value = item.Cuadrilla.HasValue ? (object)item.Cuadrilla.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@Cantidad", SqlDbType.Decimal).Value = item.Cantidad.HasValue ? (object)item.Cantidad.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@ValorUnitario", SqlDbType.Decimal).Value = item.ValorUnitario.HasValue ? (object)item.ValorUnitario.Value : DBNull.Value;
                                    insertCommand.Parameters.Add("@ValorTotal", SqlDbType.Decimal).Value = item.ValorTotal.HasValue ? (object)item.ValorTotal.Value : DBNull.Value;
                                    insertCommand.ExecuteNonQuery();
                                }

                                persistedId = item.RecursoxPresupuestoId;
                            }

                            dbIdsByClientIds[item.RecursoxPresupuestoId] = persistedId;
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