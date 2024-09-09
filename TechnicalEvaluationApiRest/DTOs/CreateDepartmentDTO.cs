namespace TechnicalEvaluationApiRest.DTOs
{
    public class CreateDepartmentDTO
    {
        public required string Name { get; set; }
        public int? CountryId { get; set; }
        public string? CountryName { get; set; }
    }
}