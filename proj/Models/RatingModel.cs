using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace proj.Models
{
    public class RatingModel
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public int RecipeId { get; set; }
        public virtual RecipeModel Recipe { get; set; }
        public int StarNumber { get; set; }
    }
}
