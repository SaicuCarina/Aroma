using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proj.Models
{
    public class WheelModel
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }  // Stochează ID-ul utilizatorului
        public string Prize { get; set; }   // Premiul câștigat
        public DateTime SpinDate { get; set; }  // Data ultimei rotiri
    }
}
