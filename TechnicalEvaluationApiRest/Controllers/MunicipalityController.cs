using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using TechnicalEvaluationApiRest.DTOs;
using TechnicalEvaluationApiRest.Interfaces;
using TechnicalEvaluationApiRest.Services;

namespace TechnicalEvaluationApiRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MunicipalityController : ControllerBase
    {
        private readonly MunicipalityService _municipalityService;
        private readonly ILogger<MunicipalityController> _logger;

        public MunicipalityController(MunicipalityService municipalityService, ILogger<MunicipalityController> logger)
        {
            _municipalityService = municipalityService;
            _logger = logger;
        }

        [HttpPost("CreateMunicipality")]
        public async Task<IActionResult> CreateMunicipality(CreateMunicipalityDTO municipalityDto)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("CreateMunicipality method called");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!Regex.IsMatch(municipalityDto.Name, @"^[a-zA-Z\s]+$")) // Solo permite letras y espacios
            {
                return BadRequest("El nombre contiene caracteres inválidos.");
            }
            if (!string.IsNullOrEmpty(municipalityDto.Name))
            {
                municipalityDto.Name = municipalityDto.Name.Trim();
            }
            if (!string.IsNullOrEmpty(municipalityDto.DepartmentName))
            {
                if (!Regex.IsMatch(municipalityDto.DepartmentName, @"^[a-zA-Z\s]+$")) // Solo permite letras y espacios
                {
                    return BadRequest("El nombre del departamento contiene caracteres inválidos.");
                }

                municipalityDto.DepartmentName = municipalityDto.DepartmentName.Trim();
            }

            try
            {
                await _municipalityService.RegisterMunicipality(municipalityDto);
                watch.Stop();
                _logger.LogInformation("Municipality created successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok("Municipio creado correctamente");
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error creating municipality, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetMunicipalities")]
        public async Task<IActionResult> GetMunicipalities()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("GetMunicipalities method called");

            try
            {
                var municipalities = await _municipalityService.GetMunicipalities();
                watch.Stop();
                _logger.LogInformation("Municipalities retrieved successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok(municipalities);
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error retrieving municipalities, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateMunicipality")]
        public async Task<IActionResult> UpdateMunicipality(UpdateMunicipalityDTO municipalityDto)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("UpdateMunicipality method called");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!Regex.IsMatch(municipalityDto.Name, @"^[a-zA-Z\s]+$")) // Solo permite letras y espacios
            {
                return BadRequest("El nombre contiene caracteres inválidos.");
            }
            if (!string.IsNullOrEmpty(municipalityDto.Name))
            {
                municipalityDto.Name = municipalityDto.Name.Trim();
            }
            if (!string.IsNullOrEmpty(municipalityDto.DepartmentName))
            {
                if (!Regex.IsMatch(municipalityDto.DepartmentName, @"^[a-zA-Z\s]+$")) // Solo permite letras y espacios
                {
                    return BadRequest("El nombre del departamento contiene caracteres inválidos.");
                }

                municipalityDto.DepartmentName = municipalityDto.DepartmentName.Trim();
            }

            try
            {
                await _municipalityService.UpdateMunicipality(municipalityDto);
                watch.Stop();
                _logger.LogInformation("Municipality updated successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok("Municipio actualizado correctamente");
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error updating municipality, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteMunicipality")]
        public async Task<IActionResult> DeleteMunicipality(int id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("DeleteMunicipality method called");

            try
            {
                await _municipalityService.DeleteMunicipality(id);
                watch.Stop();
                _logger.LogInformation("Municipality deleted successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok("Municipio eliminado correctamente");
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error deleting municipality, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }
    }
}