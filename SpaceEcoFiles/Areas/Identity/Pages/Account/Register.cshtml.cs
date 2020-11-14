using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SpaceEcoFiles.Controllers;

namespace SpaceEcoFiles.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _sharedLocalizer = sharedLocalizer;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessageResourceType = typeof(Resources.Controllers.SharedResources), ErrorMessageResourceName = "TheFieldIsRequired")]
            [EmailAddress(ErrorMessageResourceType = typeof(Resources.Controllers.SharedResources), ErrorMessageResourceName = "TheFieldIsNotAValidEmailAddress")]
            [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.Controllers.SharedResources), ErrorMessageResourceName = "TheFieldIsRequired")]
            [StringLength(100, ErrorMessageResourceType = typeof(Resources.Controllers.SharedResources), ErrorMessageResourceName = "The0MustBeAtLeast2AndAtMax1CharactersLong", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "ConfirmPassword")]
            [Compare("Password", ErrorMessageResourceType = typeof(Resources.Controllers.SharedResources), ErrorMessageResourceName = "ThePasswordAndConfirmationPasswordDoNotMatch")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    if (error.Description.Contains("is already taken"))
                    {
                        error.Description = String.Format(_sharedLocalizer["UserNameIsAlreadyTaken"], user?.Email);
                    }
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
