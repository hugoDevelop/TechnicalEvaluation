namespace TechnicalEvaluationApiRest.DTOs
{
    public class UpdateUserDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Cellphone { get; set; }
        public required string Address { get; set; }
        public int? MunicipalityId { get; set; }
        public string? MunicipalityName { get; set; }
    }
}