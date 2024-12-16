using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace proj.Models
{
    public class RecipeModel
    {
        public RecipeModel()
        {
            Comments = new List<CommentModel>(); // Inițializează colecția de comentarii
        }

        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "The Title is mandatory.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "The Article Content is mandatory.")]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public string PhotoPath { get; set; }
        public string VideoPath { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        [NotMapped]
        public IFormFile Video { get; set; }

        [Required(ErrorMessage = "The Category is mandatory.")]
        public int CategoryId { get; set; }
        public virtual CategoryModel Category { get; set; }
        [BindNever]
        public virtual ICollection<CommentModel> Comments { get; set; }
        public int Difficulty { get; set; }
        public string Time { get; set; }
        public int GetNumberOfComments()
        {
            return Comments.Count;
        }
        public int? ContestId { get; set; }
        public virtual ContestModel? Contest { get; set; }
        public string UserName { get; set; }
        public double AverageRating { get; set; }
        public virtual ICollection<RatingModel> Ratings { get; set; }
        public double GetAverageRating()
        {
            return Comments.Any(c => c.Rating.HasValue) ? Comments.Where(c => c.Rating.HasValue).Average(c => c.Rating.Value) : 0;
        }
    }
    
}
