using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using TechnicalEvaluationApiRest.DTOs;
using TechnicalEvaluationApiRest.Services;

namespace TechnicalEvaluationApiRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentService _departmentService;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(DepartmentService departmentService, ILogger<DepartmentController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        [HttpPost("CreateDepartment")]
        public async Task<IActionResult> CreateDepartment(CreateDepartmentDTO departmentDto)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("CreateDepartment method called");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!Regex.IsMatch(departmentDto.Name, @"^[a-zA-Z\s]+$")) // Solo permite letras y espacios
            {
                return BadRequest("El nombre contiene caracteres inválidos.");
            }
            if (!string.IsNullOrEmpty(departmentDto.Name))
            {
                departmentDto.Name = departmentDto.Name.Trim();
            }
            if (!string.IsNullOrEmpty(departmentDto.CountryName))
            {
                if (!Regex.IsMatch(departmentDto.CountryName, @"^[a-zA-Z\s]+$")) // Solo permite letras y espacios
                {
                    return BadRequest("El nombre del país contiene caracteres inválidos.");
                }

                departmentDto.CountryName = departmentDto.CountryName.Trim();
            }

            try
            {
                await _departmentService.RegisterDepartment(departmentDto);
                watch.Stop();
                _logger.LogInformation("Department created successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok("Departamento creado correctamente");
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error creating department, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDepartments")]
        public async Task<IActionResult> GetDepartments()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("GetDepartments method called");

            try
            {
                var departments = await _departmentService.GetDepartments();
                watch.Stop();
                _logger.LogInformation("Departments obtained successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok(departments);
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error obtaining departments, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateDepartment")]
        public async Task<IActionResult> UpdateDepartment(UpdateDepartmentDTO departmentDto)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("UpdateDepartment method called");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!Regex.IsMatch(departmentDto.Name, @"^[a-zA-Z\s]+$")) // Solo permite letras y espacios
            {
                return BadRequest("El nombre contiene caracteres inválidos.");
            }
            if (!string.IsNullOrEmpty(departmentDto.Name))
            {
                departmentDto.Name = departmentDto.Name.Trim();
            }
            if (!string.IsNullOrEmpty(departmentDto.CountryName))
            {
                if (!Regex.IsMatch(departmentDto.CountryName, @"^[a-zA-Z\s]+$")) // Solo permite letras y espacios
                {
                    return BadRequest("El nombre del país contiene caracteres inválidos.");
                }

                departmentDto.CountryName = departmentDto.CountryName.Trim();
            }

            try
            {
                await _departmentService.UpdateDepartment(departmentDto);
                watch.Stop();
                _logger.LogInformation("Department updated successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok("Departamento actualizado correctamente");
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error updating department, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteDepartment")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("DeleteDepartment method called");

            try
            {
                await _departmentService.DeleteDepartment(id);
                watch.Stop();
                _logger.LogInformation("Department deleted successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok("Departamento eliminado correctamente");
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error deleting department, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }
    }
}