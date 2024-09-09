using TechnicalEvaluationApiRest.DTOs;
using TechnicalEvaluationApiRest.Models;

namespace TechnicalEvaluationApiRest.Interfaces
{
    public interface IUserRepository
    {
        Task<CreateUserDTO> AddAsync(CreateUserDTO user);
        Task<List<User>> GetAllAsync();
        Task<UpdateUserDTO> UpdateAsync(UpdateUserDTO user);
        Task DeleteAsync(int id);
    }
}