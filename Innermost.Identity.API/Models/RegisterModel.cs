using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Innermost.Identity.API.Models
{
    public record RegisterModel
    {
        [Required]
        [RegularExpression(@"[a-zA-Z0-9_-]{4,16}",ErrorMessage = "UserName only can contain letters,numbers,- and _")]
        public string UserName { get; init; }

        [Required]
        [EmailAddress]
        public string Email { get; init; }

        [Required]
        [StringLength(30,ErrorMessage = "The {0} must be at least {2} and at max {1} characters long",MinimumLength =8)]
        public string Password { get; init; }

        [Required]
        [Compare(nameof(Password),ErrorMessage = "The password and confirmation password do not match")]
        public string ComfirmPassword { get; init; }

        public InnermostUser User { get; init; }
    }
}
