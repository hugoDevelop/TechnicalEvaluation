namespace TechnicalEvaluationApiRest.DTOs
{
    public class UpdateDepartmentDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? CountryId { get; set; }
        public string? CountryName { get; set; }
    }
}