using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Innermost.Identity.API.Models
{
    /// <summary>
    /// User model in Innermost
    /// </summary>
    public class InnermostUser:IdentityUser
    {
        [Range(1,130,ErrorMessage ="Age must between 1 and 130")]
        public int Age { get; set; }

        [RegularExpression(@"^MALE|FEMALE$", ErrorMessage = "Error gender.Gender just only can be MALE or FEMALE")]
        [Required,Column(TypeName = "VARCHAR(8)")]
        public string Gender { get; set; }

        [Required]
        [StringLength(maximumLength:18,MinimumLength =1)]
        public string NickName { get; set; }

        [StringLength(maximumLength:80)]
        public string School { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        [StringLength(maximumLength:150)]
        public string SelfDescription { get; set; }

        [RegularExpression(@"(19|20)\d{2}-(1[0-2]|0[1-9])-(0[1-9]|[1-2][0-9]|3[0-1])",ErrorMessage = "Birthday pattern error.It must like yyyy-mm-dd")]//Pattern is yyyy-mm-dd (if month less than 10 it will be 0m)
        public string Birthday { get; set; }

        [Required,Column(TypeName = "DATETIME")]
        public DateTime CreateTime { get; set; }

        [Column(TypeName ="DATETIME")]
        public DateTime UpdateTime { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime DeleteTime { get; set; }
    }
}
