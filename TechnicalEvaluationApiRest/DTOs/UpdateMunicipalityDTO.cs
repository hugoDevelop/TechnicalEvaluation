namespace TechnicalEvaluationApiRest.DTOs
{
    public class UpdateMunicipalityDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }
}