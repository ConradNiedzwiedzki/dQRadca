using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using dQRadca.Data;

namespace dQRadca.Pages.Account.Manage
{
    public class ExternalLoginsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public ExternalLoginsModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationScheme> OtherLogins { get; set; }

        public bool ShowRemoveButton { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(Resources.ApplicationTexts.UnableToLoadUserWithId, userManager.GetUserId(User)));
            }

            CurrentLogins = await userManager.GetLoginsAsync(user);
            OtherLogins = (await signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            ShowRemoveButton = user.PasswordHash != null || CurrentLogins.Count > 1;
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(Resources.ApplicationTexts.UnableToLoadUserWithId, userManager.GetUserId(User)));
            }

            var result = await userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                throw new ApplicationException(string.Format(Resources.ApplicationTexts.UnexpectedErrorWhileRemovingExternalLogin, user.Id));
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = Resources.ApplicationTexts.ExternalLoginRemoved;

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userManager.GetUserId(User));

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(Resources.ApplicationTexts.UnableToLoadUserWithId, userManager.GetUserId(User)));
            }

            var info = await signInManager.GetExternalLoginInfoAsync(await userManager.GetUserIdAsync(user));
            if (info == null)
            {
                throw new ApplicationException(string.Format(Resources.ApplicationTexts.UnexpectedErrorWhileLoadingExternalLogin, userManager.GetUserId(User)));
            }

            var result = await userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new ApplicationException(string.Format(Resources.ApplicationTexts.UnexpectedErrorWhileAddingExternalLogin, userManager.GetUserId(User)));
            }

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = Resources.ApplicationTexts.ExternalLoginAdded;
            return RedirectToPage();
        }
    }
}
