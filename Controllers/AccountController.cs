using BulkyBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Controllers
{
    public class AccountController : Controller
    {
        private readonly BulkyBookDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        
        public AccountController(UserManager<IdentityUser> userManager, IEmailSender emailSender, BulkyBookDbContext context)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _context = context;
        }
        
        public async Task<IActionResult> VerifyEmail()
        {
            int loginId = Convert.ToInt32(HttpContext.Session.GetInt32("LoginId"));
            RegisterModel regItem = _context.RegisterModels.Find(loginId);
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = regItem.Email,
                    Email = regItem.Email
                };
                string email=regItem.Email;
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmLink = Url.Action("ConfirmEmail", "Account", new { userId = loginId, token = token },Request.Scheme);
                string message = $"Please confirm your account by<a href='{confirmLink}'> Click here";
                await _emailSender.SendEmailAsync(email, "Confirm Your Email", message);
            }
            return RedirectToAction("Cart", "Home");
        }

        [HttpGet]
        public ActionResult ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Error", "Home");
            }
            int id = Convert.ToInt32(userId);
            RegisterModel user = _context.RegisterModels.Find(id);
            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                user.EmailConfirmed = true;
                //_context.SaveChanges();
                TempData["ConformMessage"] = "Email Verification Sucessfull...";
                return RedirectToAction("Cart", "Home");
            }            
        }
    }
}
