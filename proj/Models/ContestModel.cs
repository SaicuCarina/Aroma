using System.ComponentModel.DataAnnotations;

namespace proj.Models
{
    public class ContestModel
    {
        [Key]
        public int Id { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Theme { get; set; }
    }
}
