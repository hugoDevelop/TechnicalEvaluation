namespace TechnicalEvaluationApiRest.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TechnicalEvaluationApiRest.Data;
    using TechnicalEvaluationApiRest.DTOs;
    using TechnicalEvaluationApiRest.Interfaces;
    using TechnicalEvaluationApiRest.Models;

    public class MunicipalityRepository : IMunicipalityRepository
    {
        private readonly AppDbContext _context;

        public MunicipalityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreateMunicipalityDTO> AddAsync(CreateMunicipalityDTO municipality)
        {
            var parameters = new[]
            {
                new Npgsql.NpgsqlParameter("@p_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = municipality.Name },
                new Npgsql.NpgsqlParameter("@p_department_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = municipality.DepartmentId ?? (object)DBNull.Value },
                new Npgsql.NpgsqlParameter("@p_department_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = municipality.DepartmentName ?? (object)DBNull.Value }
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL locations.sp_insert_municipality(@p_name, @p_department_id, @p_department_name)",
                    parameters);
                return municipality;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task DeleteAsync(int id)
        {
            var parameters = new[]
            {
                new Npgsql.NpgsqlParameter("@p_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = id }
            };

            try
            {
                _context.Database.ExecuteSqlRaw("CALL locations.sp_delete_municipality(@p_id)", parameters);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Municipality>> GetAllAsync()
        {
            var municipalities = new List<Municipality>();

            // Utiliza un DataReader para ejecutar la función y leer los resultados
            await using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT * FROM locations.fn_get_municipalities()";
                command.CommandType = System.Data.CommandType.Text;

                // Asegúrate de que la conexión esté abierta
                if (command.Connection?.State != System.Data.ConnectionState.Open)
                {
                    if (command.Connection != null)
                    {
                        await command.Connection.OpenAsync();
                    }
                }

                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var municipality = new Municipality
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            DepartmentId = reader.GetInt32(2),
                            Department = new Department
                            {
                                Id = reader.GetInt32(2),
                                Name = reader.GetString(3),
                                CountryId = reader.GetInt32(4),
                                Country = new Country
                                {
                                    Id = reader.GetInt32(4),
                                    Name = reader.GetString(5)
                                }
                            }
                        };

                        municipalities.Add(municipality);
                    }
                }
            }

            return municipalities;
        }

        public Task<UpdateMunicipalityDTO> UpdateAsync(UpdateMunicipalityDTO municipality)
        {
            var parameters = new[]
            {
                new Npgsql.NpgsqlParameter("@p_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = municipality.Id },
                new Npgsql.NpgsqlParameter("@p_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = municipality.Name },
                new Npgsql.NpgsqlParameter("@p_department_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = municipality.DepartmentId ?? (object)DBNull.Value },
                new Npgsql.NpgsqlParameter("@p_department_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = municipality.DepartmentName ?? (object)DBNull.Value }
            };

            try
            {
                _context.Database.ExecuteSqlRaw(
                    "CALL locations.sp_update_municipality(@p_id, @p_name, @p_department_id, @p_department_name)",
                    parameters);
                return Task.FromResult(municipality);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}