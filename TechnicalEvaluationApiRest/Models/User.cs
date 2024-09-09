using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TechnicalEvaluationApiRest.Models
{
    [Table("users", Schema = "users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public required string Name { get; set; }
        [Column("cellphone")]
        public required string Cellphone { get; set; }
        [Column("address")]
        public required string Address { get; set; }
        [Column("municipality_id")]
        public int MunicipalityId { get; set; }
        public Municipality? Municipality { get; set; }
    }
}