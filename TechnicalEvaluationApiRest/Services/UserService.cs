using TechnicalEvaluationApiRest.DTOs;
using TechnicalEvaluationApiRest.Interfaces;
using TechnicalEvaluationApiRest.Models;

namespace TechnicalEvaluationApiRest.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task RegisterUser(CreateUserDTO userDto)
        {
            try
            {
                await _userRepository.AddAsync(userDto);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar el usuario. " + ex.Message);
            }
        }

        public async Task<List<User>> GetUsers()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los usuarios. " + ex.Message);
            }
        }

        public async Task UpdateUser(UpdateUserDTO userDto)
        {
            try
            {
                await _userRepository.UpdateAsync(userDto);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el usuario. " + ex.Message);
            }
        }

        public async Task DeleteUser(int id)
        {
            try
            {
                await _userRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el usuario. " + ex.Message);
            }
        }
    }
}