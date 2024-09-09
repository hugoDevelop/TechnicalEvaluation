using TechnicalEvaluationApiRest.DTOs;
using TechnicalEvaluationApiRest.Models;

namespace TechnicalEvaluationApiRest.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<CreateDepartmentDTO> AddAsync(CreateDepartmentDTO country);
        Task<List<Department>> GetAllAsync();
        Task<UpdateDepartmentDTO> UpdateAsync(UpdateDepartmentDTO country);
        Task DeleteAsync(int id);
    }
}