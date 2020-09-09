using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(maximumLength: 100)]
        public string FirstName { get; set; }

        [StringLength(maximumLength: 100)]
        public string LastName { get; set; }

        [StringLength(maximumLength: 200)]
        public string Nickname { get; set; }

        [StringLength(maximumLength: 50)]
        public string Birthdate { get; set; }

        [StringLength(maximumLength: 10)]
        public string NationalCode { get; set; }

        [StringLength(maximumLength: 10)]
        public string Gender { get; set; }
        
        [StringLength(maximumLength: 20)]
        public string Phone { get; set; }

        public DateTime RegistrationDateTime { get; set; }

        [StringLength(maximumLength: 10)]
        public string VerificationCode { get; set; }

        [StringLength(maximumLength: 200)]
        public string AvatarUrl { get; set; }
        public string Favorites { get; set; }
        public string Address { get; set; }
        public string CartNumber { get; set; }
    }
    
}
