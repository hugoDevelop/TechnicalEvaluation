using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using TechnicalEvaluationApiRest.DTOs;
using TechnicalEvaluationApiRest.Models;
using TechnicalEvaluationApiRest.Services;

namespace TechnicalEvaluationApiRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserDTO createUserDTO)
        {
            // cronometrar el tiempo de ejecución
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("CreateUser method called");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrWhiteSpace(createUserDTO.Name) || createUserDTO.Name.Length > 100)
            {
                return BadRequest("El nombre es obligatorio y no debe exceder los 100 caracteres.");
            }
            if (createUserDTO.Cellphone.Length < 8)
            {
                return BadRequest("El número de teléfono debe tener al menos 8 caracteres.");
            }
            if (!Regex.IsMatch(createUserDTO.Name, @"^[a-zA-Z\s]+$")) // Solo permite letras y espacios
            {
                return BadRequest("El nombre contiene caracteres inválidos.");
            }
            if (!Regex.IsMatch(createUserDTO.Cellphone, @"^[0-9]+$")) // Solo permite números
            {
                return BadRequest("El número de teléfono contiene caracteres inválidos.");
            }
            if (!string.IsNullOrWhiteSpace(createUserDTO.CountryName))
            {
                if (!Regex.IsMatch(createUserDTO.CountryName, @"^[a-zA-Z\s]+$")) // Solo permite letras y espacios
                {
                    return BadRequest("El nombre del país contiene caracteres inválidos.");
                }

                createUserDTO.CountryName = createUserDTO.CountryName.Trim();
            }
            if (!string.IsNullOrWhiteSpace(createUserDTO.DepartmentName))
            {
                if (!Regex.IsMatch(createUserDTO.DepartmentName, @"^[a-zA-Z\s]+$")) // Solo permite letras y espacios
                {
                    return BadRequest("El nombre del departamento contiene caracteres inválidos.");
                }

                createUserDTO.DepartmentName = createUserDTO.DepartmentName.Trim();
            }
            if (!string.IsNullOrWhiteSpace(createUserDTO.MunicipalityName))
            {
                if (!Regex.IsMatch(createUserDTO.MunicipalityName, @"^[a-zA-Z\s]+$")) // Solo permite letras y espacios
                {
                    return BadRequest("El nombre del municipio contiene caracteres inválidos.");
                }

                createUserDTO.MunicipalityName = createUserDTO.MunicipalityName.Trim();
            }

            try
            {
                await _userService.RegisterUser(createUserDTO);
                watch.Stop();
                _logger.LogInformation("User created successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok("Usuario creado correctamente");
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error creating user, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUsers")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("GetUsers method called");

            try
            {
                var users = await _userService.GetUsers();
                watch.Stop();
                _logger.LogInformation("Users retrieved successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok(users);
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error retrieving users, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult<User>> UpdateUser(UpdateUserDTO updateUserDTO)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("UpdateUser method called");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (updateUserDTO.Id == 0)
            {
                return BadRequest("El id es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(updateUserDTO.Name) || updateUserDTO.Name.Length > 100)
            {
                return BadRequest("El nombre es obligatorio y no debe exceder los 100 caracteres.");
            }
            if (updateUserDTO.Cellphone.Length < 8)
            {
                return BadRequest("El número de teléfono debe tener al menos 8 caracteres.");
            }
            if (!Regex.IsMatch(updateUserDTO.Name, @"^[a-zA-Z\s]+$")) // Solo permite letras y espacios
            {
                return BadRequest("El nombre contiene caracteres inválidos.");
            }
            if (!Regex.IsMatch(updateUserDTO.Cellphone, @"^[0-9]+$")) // Solo permite números
            {
                return BadRequest("El número de teléfono contiene caracteres inválidos.");
            }
            if (!string.IsNullOrWhiteSpace(updateUserDTO.MunicipalityName))
            {
                if (!Regex.IsMatch(updateUserDTO.MunicipalityName, @"^[a-zA-Z\s]+$")) // Solo permite letras y espacios
                {
                    return BadRequest("El nombre del municipio contiene caracteres inválidos.");
                }

                updateUserDTO.MunicipalityName = updateUserDTO.MunicipalityName.Trim();
            }

            try
            {
                await _userService.UpdateUser(updateUserDTO);
                watch.Stop();
                _logger.LogInformation("User updated successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok("Usuario actualizado correctamente");
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error updating user, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteUser")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("DeleteUser method called");

            try
            {
                await _userService.DeleteUser(id);
                watch.Stop();
                _logger.LogInformation("User deleted successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok("Usuario eliminado correctamente");
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error deleting user, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }
    }
}