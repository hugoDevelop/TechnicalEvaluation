using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using TechnicalEvaluationApiRest.DTOs;
using TechnicalEvaluationApiRest.Services;

namespace TechnicalEvaluationApiRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly CountryService _countryService;
        private readonly ILogger<CountryController> _logger;

        public CountryController(CountryService countryService, ILogger<CountryController> logger)
        {
            _countryService = countryService;
            _logger = logger;
        }

        [HttpPost("CreateCountry")]
        public async Task<IActionResult> CreateCountry(CreateCountryDTO countryDto)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("CreateUser method called");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!Regex.IsMatch(countryDto.Name, @"^[a-zA-Z\s]+$")) // Solo permite letras y espacios
            {
                return BadRequest("El nombre contiene caracteres inválidos.");
            }

            try
            {
                await _countryService.RegisterCountry(countryDto);
                watch.Stop();
                _logger.LogInformation("Country created successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok("País creado correctamente");
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error creating country, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCountries")]
        public async Task<IActionResult> GetCountries()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("GetCountries method called");

            try
            {
                var countries = await _countryService.GetCountries();
                watch.Stop();
                _logger.LogInformation("Countries retrieved successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok(countries);
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error retrieving countries, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateCountry")]
        public async Task<IActionResult> UpdateCountry(UpdateCountryDTO updateCountryDTO)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("UpdateCountry method called");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _countryService.UpdateCountry(updateCountryDTO);
                watch.Stop();
                _logger.LogInformation("Country updated successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok("País actualizado correctamente");
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error updating country, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteCountry")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("DeleteCountry method called");

            try
            {
                await _countryService.DeleteCountry(id);
                watch.Stop();
                _logger.LogInformation("Country deleted successfully, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return Ok("País eliminado correctamente");
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError("Error deleting country, elapsed time: {0} ms", watch.ElapsedMilliseconds);
                return BadRequest(ex.Message);
            }
        }
    }
}