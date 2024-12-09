using System.ComponentModel.DataAnnotations;

namespace proj.Models
{
    public class CommentModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int RecipeId { get; set; }
        public virtual RecipeModel Recipe { get; set; }
        [Required]
        public string UserName { get; set; }
        public string Content { get; set; }
        public int? Rating { get; set; }
        public DateTime Date { get; set; }
    }
}
