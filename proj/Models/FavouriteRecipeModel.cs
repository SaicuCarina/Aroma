using System.ComponentModel.DataAnnotations;

namespace proj.Models
{
    public class FavouriteRecipe
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; } // Numele utilizatorului care a adăugat în favorite
        public int RecipeId { get; set; } // ID-ul rețetei
        public virtual RecipeModel Recipe { get; set; }
    }

}
