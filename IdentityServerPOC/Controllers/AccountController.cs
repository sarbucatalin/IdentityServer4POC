using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServerPOC.Infrastructure;
using IdentityServerPOC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace IdentityServerPOC.Controllers
{

    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEventService _events;
        private const string _invalidCredentialsErrorMessage = "Invalid username or password";
        private const string _userLockedOutMessage = "user locked out";
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(IIdentityServerInteractionService interaction, UserManager<AppUser> userManager, IEventService events, SignInManager<AppUser> signInManager)
        {
            _interaction = interaction;
            _userManager = userManager;
            _events = events;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            var vm = new Models.LoginInputModel { ReturnUrl = returnUrl, Username = context?.LoginHint };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberLogin, lockoutOnFailure: true);

                if (user != null && signInResult.Succeeded)
                {
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.Name));

                    if (context != null)
                    {
                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(model.ReturnUrl);
                    }
                    // request for a local page
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }

                if (signInResult.IsLockedOut)
                {
                    await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, _userLockedOutMessage));
                    ModelState.AddModelError(string.Empty, _userLockedOutMessage);
                }
                else
                {
                    await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));
                    ModelState.AddModelError(string.Empty, _invalidCredentialsErrorMessage);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();
            var context = await _interaction.GetLogoutContextAsync(logoutId);
            return Redirect(context.PostLogoutRedirectUri);
        }

    }
}
