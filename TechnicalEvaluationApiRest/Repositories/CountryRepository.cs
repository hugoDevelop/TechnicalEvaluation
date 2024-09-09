using Microsoft.EntityFrameworkCore;
using Npgsql;
using TechnicalEvaluationApiRest.Data;
using TechnicalEvaluationApiRest.DTOs;
using TechnicalEvaluationApiRest.Interfaces;
using TechnicalEvaluationApiRest.Models;

namespace TechnicalEvaluationApiRest.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly AppDbContext _context;

        public CountryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreateCountryDTO> AddAsync(CreateCountryDTO country)
        {
            // Llamar al procedimiento almacenado para registrar el país
            var parameters = new[]
            {
                new NpgsqlParameter("@p_name", country.Name)
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync("CALL locations.sp_insert_country(@p_name)", parameters);
                return country;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Task DeleteAsync(int id)
        {
            var parameters = new[]
            {
                new NpgsqlParameter("@p_id", id)
            };

            try
            {
                _context.Database.ExecuteSqlRaw("CALL locations.sp_delete_country(@p_id)", parameters);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Country>> GetAllAsync()
        {
            var countries = new List<Country>();

            // Utiliza un DataReader para ejecutar la función y leer los resultados
            await using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT * FROM locations.fn_get_countries()";
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
                        countries.Add(new Country
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }

            return countries;
        }

        public Task<UpdateCountryDTO> UpdateAsync(UpdateCountryDTO country)
        {
            var parameters = new[]
            {
                new NpgsqlParameter("@p_id", country.Id),
                new NpgsqlParameter("@p_new_name", country.Name)
            };

            try
            {
                _context.Database.ExecuteSqlRaw("CALL locations.sp_update_country(@p_id, @p_new_name)", parameters);
                return Task.FromResult(country);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}