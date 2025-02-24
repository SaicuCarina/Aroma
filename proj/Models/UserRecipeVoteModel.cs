using proj.Models;

public class UserRecipeVoteModel
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public int RecipeId { get; set; }
    public virtual RecipeModel Recipe { get; set; }
}
