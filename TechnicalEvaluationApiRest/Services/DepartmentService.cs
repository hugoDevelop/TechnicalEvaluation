using TechnicalEvaluationApiRest.DTOs;
using TechnicalEvaluationApiRest.Interfaces;
using TechnicalEvaluationApiRest.Models;

namespace TechnicalEvaluationApiRest.Services
{
    public class DepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task RegisterDepartment(CreateDepartmentDTO departmentDto)
        {
            try
            {
                await _departmentRepository.AddAsync(departmentDto);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar el departamento. " + ex.Message);
            }
        }

        public async Task<List<Department>> GetDepartments()
        {
            try
            {
                var departments = await _departmentRepository.GetAllAsync();
                return departments;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los departamentos. " + ex.Message);
            }
        }

        public async Task UpdateDepartment(UpdateDepartmentDTO departmentDto)
        {
            try
            {
                await _departmentRepository.UpdateAsync(departmentDto);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el departamento. " + ex.Message);
            }
        }

        public async Task DeleteDepartment(int id)
        {
            try
            {
                await _departmentRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el departamento. " + ex.Message);
            }
        }
    }
}