using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechnicalEvaluationApiRest.Models
{
    [Table("departments", Schema = "locations")]
    public class Department
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public required string Name { get; set; }
        [Column("country_id")]
        public int CountryId { get; set; }

        public Country? Country { get; set; }
    }
}