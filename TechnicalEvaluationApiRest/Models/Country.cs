using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechnicalEvaluationApiRest.Models
{
    [Table("countries", Schema = "locations")]
    public class Country
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public required string Name { get; set; }
    }
}