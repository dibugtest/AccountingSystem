using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using AccountingSystem.Models;
using AccountingSystem.ViewModel;
using AccountingSystem.DAL;
using System.Text.RegularExpressions;

namespace AccountingSystem.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private AccountContext db = new AccountContext();
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            //return RedirectToAction("Index","Home");
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {

            //ViewData["ReturnUrl"] = returnUrl;
    //        if (model.UserName.IndexOf('@') > -1)
    //        {
    //            //Validate email format
    //            string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
    //                                   @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
    //                                      @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
    //            Regex re = new Regex(emailRegex);
    //            if (!re.IsMatch(model.UserName))
    //            {
    //                ModelState.AddModelError("Email", "Email is not valid");
    //            }
    //        }
    //        else
    //        {
    //            //validate Username format
    //            string emailRegex = @"^[a-zA-Z0-9]*$";
    //            Regex re = new Regex(emailRegex);
    //            if (!re.IsMatch(model.UserName))s
    //            {
    //                ModelState.AddModelError("Email", "Email is not valid");
    //            }
    //        }
 
    //if (ModelState.IsValid)
    //{
    //    var userName = model.UserName;
    //    if (userName.IndexOf('@') > -1)
    //    {
    //        var user =  await UserManager.FindByEmailAsync(model.UserName);
    //        if (user == null)
    //        {
    //            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
    //            return View(model);
    //        }
    //        else
    //        {
    //            userName = user.UserName;
    //        }
    //    }
    //    var result = await _signInManager.PasswordSignInAsync(userName, model.Password, model.RememberMe, lockoutOnFailure: false);
           
            
            
            var appUser = new ApplicationDbContext();

              
            if (ModelState.IsValid)
            {
                string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                      @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                         @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                string phoneRegx = @"^\d{10}$";
                Regex re = new Regex(emailRegex);
                Regex rex = new Regex(phoneRegx);
                if ((!re.IsMatch(model.UserName)) && (!rex.IsMatch(model.UserName)))
                {
                    ModelState.AddModelError("", "Invalid Email/Phone and Password.");
                    return View(model);

                }
                var getUser = appUser.Users.SingleOrDefault(m => (m.Mobile == model.UserName) || (m.Email == model.UserName));
                if (getUser != null)
                {
                    var user = await UserManager.FindAsync(getUser.UserName, model.Password);
                    if (user != null)
                    {
                        await SignInAsync(user, model.RememberMe);
                        var fiscalYear = db.fisYears.FirstOrDefault(m => m.status == true);
                        Session["fiscalYear"] = fiscalYear.nepFy;
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid Email/Phone or Password.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Email/Phone and Password.");
                }

            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        //[AllowAnonymous]
        public ActionResult Register()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                Response.Write("<script>alert('Please Login to Register new Admin user.');</script>");
                return View();
            }
        }

        //
        // POST: /Account/Register
        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            if (ModelState.IsValid)
            {
                var objUser = context.Users.FirstOrDefault(m => m.Email == model.Email || m.Mobile == model.Mobile);
                if (objUser == null)
                {
                    var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email, Mobile = model.Mobile };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "User with Email or Mobile already exists.Please try with different mobile orEmail.";
                    return View(model);
                }

                
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        public ActionResult AccountSettings(bool? savedSuccesfully)
        {
            //fist, we need to find the logged in user data
            var user = UserManager.FindById(User.Identity.GetUserId());

            ViewBag.ErrorMessage = "";

            //then create AccountSettingsViewModel with data filled in.
            //notice that we user user object initialized before
            AccountSettingsViewModel model = new AccountSettingsViewModel { Email = user.Email,UserName=user.UserName,Mobile=user.Mobile };


            //the section below defines what happens on the form
            ViewBag.showForm = true;
            if (savedSuccesfully != null && savedSuccesfully == true)
            {
                ViewBag.showForm = false;
                ViewBag.StatusMessage = "Data saved sucessfully";
            }
            else if (savedSuccesfully != null && savedSuccesfully == false)
            {
                ViewBag.showForm = false;
                ViewBag.ErrorMessage = "Something went wrong!";

            }

            return View(model);
        }



        //
        // POST: /Account/AccountSettings
        [HttpPost]
        [ValidateAntiForgeryToken]
        //this action is called when user attempts to save data
        //therefore it receives AccountSettingsViewModel object
        public async Task<ActionResult> AccountSettings(AccountSettingsViewModel model)
        {
            ViewBag.ReturnUrl = Url.Action("AccountSettings");

            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.Mobile = model.Mobile;

                IdentityResult result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    //if everything is validated and saved successfully,
                    //we will be redirected to AccountSettings GET action
                    //where success message will be displayed, but form will be hidden
                    return RedirectToAction("AccountSettings", new { savedSuccesfully = true });
                }

            }

            // If we got this far, something failed, redisplay form
            ViewBag.showForm = true;
            return View(model);
        }


        [Authorize(Users="SuperAdmin")]
        public ActionResult listAppUser()
        {
            ApplicationDbContext context2 = new ApplicationDbContext();
            var allusers = context2.Users.ToList();
            var userVM = allusers.Select(user => new UserViewModel { userId = user.Id, UserName = user.UserName,Email=user.Email,phone=user.Mobile, Role ="Admin" }).ToList();
            return View(userVM);
        }



        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}