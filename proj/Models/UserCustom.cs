using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace proj.Models
{
    public class UserCustom : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool isWinner { get; set; } = false;

        public virtual ICollection<CommentModel>? Comments { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

        public bool IsPostingRevoked { get; set; } = false;
        public DateTime RegistrationDate { get; set; }
        public bool hasSpunWheel { get; set; } = false;
        public DateTime LastSpinDate { get; set; }
    }
}
