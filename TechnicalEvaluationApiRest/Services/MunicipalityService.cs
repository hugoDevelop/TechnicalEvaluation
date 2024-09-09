using Microsoft.AspNetCore.Mvc;
using TechnicalEvaluationApiRest.DTOs;
using TechnicalEvaluationApiRest.Interfaces;
using TechnicalEvaluationApiRest.Models;

namespace TechnicalEvaluationApiRest.Services
{
    public class MunicipalityService : ControllerBase
    {
        private readonly IMunicipalityRepository _municipalityRepository;

        public MunicipalityService(IMunicipalityRepository municipalityRepository)
        {
            _municipalityRepository = municipalityRepository;
        }

        public async Task RegisterMunicipality(CreateMunicipalityDTO municipality)
        {
            try
            {
                await _municipalityRepository.AddAsync(municipality);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar el municipio. " + ex.Message);
            }
        }

        public async Task<List<Municipality>> GetMunicipalities()
        {
            try
            {
                var municipalities = await _municipalityRepository.GetAllAsync();
                return municipalities;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los municipios. " + ex.Message);
            }
        }

        public async Task UpdateMunicipality(UpdateMunicipalityDTO municipality)
        {
            try
            {
                await _municipalityRepository.UpdateAsync(municipality);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el municipio. " + ex.Message);
            }
        }

        public async Task DeleteMunicipality(int id)
        {
            try
            {
                await _municipalityRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el municipio. " + ex.Message);
            }
        }
    }
}