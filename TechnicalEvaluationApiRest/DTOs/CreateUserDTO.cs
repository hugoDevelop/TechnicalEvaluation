namespace TechnicalEvaluationApiRest.DTOs
{
    public class CreateUserDTO
    {
        public required string Name { get; set; }
        public required string Cellphone { get; set; }
        public required string Address { get; set; }
        public int? CountryId { get; set; }
        public string? CountryName { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? MunicipalityId { get; set; }
        public string? MunicipalityName { get; set; }
    }
}