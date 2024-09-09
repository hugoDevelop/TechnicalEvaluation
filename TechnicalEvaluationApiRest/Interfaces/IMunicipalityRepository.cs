using TechnicalEvaluationApiRest.DTOs;
using TechnicalEvaluationApiRest.Models;

namespace TechnicalEvaluationApiRest.Interfaces
{
    public interface IMunicipalityRepository
    {
        Task<CreateMunicipalityDTO> AddAsync(CreateMunicipalityDTO municipality);
        // obtener todos los municipios
        Task<List<Municipality>> GetAllAsync();
        // actualizar municipio
        Task<UpdateMunicipalityDTO> UpdateAsync(UpdateMunicipalityDTO municipality);
        // eliminar municipio
        Task DeleteAsync(int id);
    }
}