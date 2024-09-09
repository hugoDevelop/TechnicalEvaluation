using Microsoft.EntityFrameworkCore;
using Npgsql;
using TechnicalEvaluationApiRest.Data;
using TechnicalEvaluationApiRest.DTOs;
using TechnicalEvaluationApiRest.Interfaces;
using TechnicalEvaluationApiRest.Models;

namespace TechnicalEvaluationApiRest.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;

        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreateDepartmentDTO> AddAsync(CreateDepartmentDTO department)
        {
            var parameters = new[]
            {
                new NpgsqlParameter("@p_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = department.Name },
                new NpgsqlParameter("@p_country_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = department.CountryId ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_country_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = department.CountryName ?? (object)DBNull.Value }
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL locations.sp_insert_department(@p_name, @p_country_id, @p_country_name)",
                    parameters);
                return department;
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
                new NpgsqlParameter("@p_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = id }
            };

            try
            {
                _context.Database.ExecuteSqlRaw("CALL locations.sp_delete_department(@p_id)", parameters);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Department>> GetAllAsync()
        {
            var departments = new List<Department>();

            // Utiliza un DataReader para ejecutar la función y leer los resultados
            await using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT * FROM locations.fn_get_departments()";
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
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            CountryId = reader.GetInt32(2),
                            Country = new Country
                            {
                                Id = reader.GetInt32(2),
                                Name = reader.GetString(3)
                            }
                        });
                    }
                }
            }

            return departments;
        }

        public Task<UpdateDepartmentDTO> UpdateAsync(UpdateDepartmentDTO country)
        {
            var parameters = new[]
            {
                new NpgsqlParameter("@p_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = country.Id },
                new NpgsqlParameter("@p_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = country.Name },
                new NpgsqlParameter("@p_country_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = country.CountryId ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_country_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = country.CountryName ?? (object)DBNull.Value }
            };

            try
            {
                _context.Database.ExecuteSqlRaw(
                    "CALL locations.sp_update_department(@p_id, @p_name, @p_country_id, @p_country_name)",
                    parameters);
                return Task.FromResult(country);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}