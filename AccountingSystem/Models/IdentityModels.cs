using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace AccountingSystem.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {


        [Required] 
        [MaxLength(200)] 
        [Display(Name = "E-mail")] 
        public string Email { get; set; }


        [Required]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Mobile Number Not Valid")]
        public string Mobile { get; set; }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("mycon")
        {
        }
    }
}