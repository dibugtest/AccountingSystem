using AccountingSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AccountingSystem.Startup))]
namespace AccountingSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createUsers();
        }

        // In this method we will create default User roles and Admin user for login
        private void createUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            //  var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating  a default Admin User 
            var user = new ApplicationUser();
            user.UserName = "SuperAdmin";
            user.Email = "superAdmin@gmail.com";
            user.Mobile = "9801010101";
            string userPWD = "admin123";


            var chkUser = UserManager.Create(user, userPWD);


        }
    }
}

