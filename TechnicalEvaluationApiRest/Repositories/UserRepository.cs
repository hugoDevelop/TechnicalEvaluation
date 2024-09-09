using Microsoft.EntityFrameworkCore;
using Npgsql;
using TechnicalEvaluationApiRest.Data;
using TechnicalEvaluationApiRest.DTOs;
using TechnicalEvaluationApiRest.Interfaces;
using TechnicalEvaluationApiRest.Models;

namespace TechnicalEvaluationApiRest.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreateUserDTO> AddAsync(CreateUserDTO user)
        {
            var parameters = new[]
            {
                new NpgsqlParameter("@p_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = user.Name ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_cellphone", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = user.Cellphone ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_address", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = user.Address ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_country_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = user.CountryId ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_department_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = user.DepartmentId ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_municipality_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = user.MunicipalityId ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_country_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = user.CountryName ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_department_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = user.DepartmentName ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_municipality_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = user.MunicipalityName ?? (object)DBNull.Value }
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL users.sp_insert_user(@p_name, @p_cellphone, @p_address, @p_country_id, @p_department_id, @p_municipality_id, @p_country_name, @p_department_name, @p_municipality_name)",
                    parameters);
                return user;
            }
            catch (Exception ex)
            {
                // Puedes manejar la excepción de manera más específica si es necesario
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
                _context.Database.ExecuteSqlRaw("CALL users.sp_delete_user(@p_id)", parameters);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<User>> GetAllAsync()
        {
            var users = new List<User>();

            // Utiliza un DataReader para ejecutar la función y leer los resultados
            await using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT * FROM users.fn_get_users()";
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
                        var user = new User
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Cellphone = reader.GetString(2),
                            Address = reader.GetString(3),
                            MunicipalityId = reader.GetInt32(4),
                            Municipality = new Municipality
                            {
                                Id = reader.GetInt32(4),
                                Name = reader.GetString(5),
                                DepartmentId = reader.GetInt32(6),
                                Department = new Department
                                {
                                    Id = reader.GetInt32(6),
                                    Name = reader.GetString(7),
                                    CountryId = reader.GetInt32(8),
                                    Country = new Country
                                    {
                                        Id = reader.GetInt32(8),
                                        Name = reader.GetString(9)
                                    }
                                }
                            }
                        };

                        users.Add(user);
                    }
                }
            }

            return users;
        }

        public Task<UpdateUserDTO> UpdateAsync(UpdateUserDTO user)
        {
            var parameters = new[]
            {
                new NpgsqlParameter("@p_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = user.Id },
                new NpgsqlParameter("@p_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = user.Name ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_cellphone", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = user.Cellphone ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_address", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = user.Address ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_municipality_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = user.MunicipalityId ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_municipality_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = user.MunicipalityName ?? (object)DBNull.Value }
            };

            try
            {
                _context.Database.ExecuteSqlRaw(
                    "CALL users.sp_update_user(@p_id, @p_name, @p_cellphone, @p_address, @p_municipality_id, @p_municipality_name)",
                    parameters);
                return Task.FromResult(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
