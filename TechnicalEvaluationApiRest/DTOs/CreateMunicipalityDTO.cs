namespace TechnicalEvaluationApiRest.DTOs
{
    public class CreateMunicipalityDTO
    {
        public required string Name { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }
}