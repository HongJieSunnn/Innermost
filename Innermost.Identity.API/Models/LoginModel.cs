using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Innermost.Identity.API.Models
{
    public class LoginModel
    {
        [Required]
        public string Account { get; set; }
        [MinLength(8,ErrorMessage ="password's length must lengthen than 8")]
        [MaxLength(25,ErrorMessage = "password's length must less than 25")]
        public string PassWord { get; set; }
        [Required]
        [RegularExpression(@"[Email|UserName]",ErrorMessage ="Account type must be Email or UserName")]
        public string AccountType { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}
