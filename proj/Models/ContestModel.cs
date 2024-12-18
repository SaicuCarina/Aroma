using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proj.Models
{
    public class ContestModel
    {
        [Key]
        public int Id { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Theme { get; set; }
        public string PhotoPath { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
