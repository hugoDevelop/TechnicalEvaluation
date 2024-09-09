using TechnicalEvaluationApiRest.DTOs;
using TechnicalEvaluationApiRest.Interfaces;
using TechnicalEvaluationApiRest.Models;

namespace TechnicalEvaluationApiRest.Services
{
    public class CountryService
    {
        private readonly ICountryRepository _countryRepository;

        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task RegisterCountry(CreateCountryDTO countryDto)
        {
            try
            {
                await _countryRepository.AddAsync(countryDto);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar el país. " + ex.Message);
            }
        }

        public async Task<List<Country>> GetCountries()
        {
            try
            {
                var countries = await _countryRepository.GetAllAsync();
                return countries;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los países. " + ex.Message);
            }
        }

        public async Task DeleteCountry(int id)
        {
            try
            {
                await _countryRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el país. " + ex.Message);
            }
        }

        public async Task UpdateCountry(UpdateCountryDTO countryDto)
        {
            try
            {
                await _countryRepository.UpdateAsync(countryDto);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el país. " + ex.Message);
            }
        }
    }
}