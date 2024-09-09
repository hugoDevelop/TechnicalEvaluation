using TechnicalEvaluationApiRest.DTOs;
using TechnicalEvaluationApiRest.Models;

namespace TechnicalEvaluationApiRest.Interfaces
{
    public interface ICountryRepository
    {
        Task<CreateCountryDTO> AddAsync(CreateCountryDTO country);
        Task<List<Country>> GetAllAsync();
        Task<UpdateCountryDTO> UpdateAsync(UpdateCountryDTO country);
        Task DeleteAsync(int id);
    }
}