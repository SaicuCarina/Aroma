using System.ComponentModel.DataAnnotations;

namespace proj.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public virtual ICollection<RecipeModel> Recipes { get; set; }
    }
}
