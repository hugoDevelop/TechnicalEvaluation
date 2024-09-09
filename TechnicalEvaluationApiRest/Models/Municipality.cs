using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechnicalEvaluationApiRest.Models
{
    [Table("municipalities", Schema = "locations")]
    public class Municipality
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public required string Name { get; set; }
        [Column("department_id")]
        public int DepartmentId { get; set; }

        public Department? Department { get; set; }
    }
}