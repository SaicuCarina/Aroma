using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proj.Models
{
    public class AdvertisementModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string PhotoPath { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}