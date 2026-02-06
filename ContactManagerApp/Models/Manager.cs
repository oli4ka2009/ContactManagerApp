using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ContactManagerApp.ValidationAttributes;

namespace ContactManagerApp.Models
{
    public class Manager
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [BirthDate]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public bool IsMarried { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 10000000, ErrorMessage = "Salary must be positive")]
        public decimal? Salary { get; set; }
    }
}
