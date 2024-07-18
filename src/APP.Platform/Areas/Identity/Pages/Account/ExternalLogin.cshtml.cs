// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace APP.Platform.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            ILogger<ExternalLoginModel> logger
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ProviderDisplayName { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGet() => RedirectToPage("./Login");

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page(
                "./ExternalLogin",
                pageHandler: "Callback",
                values: new { returnUrl }
            );
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(
                provider,
                redirectUrl
            );
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(
            string returnUrl = null,
            string remoteError = null
        )
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (!string.IsNullOrEmpty(remoteError))
                return RedirectToErrorPage(remoteError, returnUrl);

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToErrorPage("Error loading external login information.", returnUrl);

            var signInResult = await AttemptSignIn(info, returnUrl);
            if (signInResult != null)
                return signInResult;

            ReturnUrl = returnUrl;
            ProviderDisplayName = info.ProviderDisplayName;

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                SetInputModel(info);

            var user = CreateUser();
            return await RegisterUser(user, info, returnUrl);
        }

        private IActionResult RedirectToErrorPage(string errorMessage, string returnUrl)
        {
            ErrorMessage = $"Error from external provider: {errorMessage}";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        private async Task<IActionResult> AttemptSignIn(ExternalLoginInfo info, string returnUrl)
        {
            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true
            );
            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "{Name} logged in with {LoginProvider} provider.",
                    info.Principal.Identity.Name,
                    info.LoginProvider
                );
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
                return RedirectToPage("./Lockout");
            return null;
        }

        private void SetInputModel(ExternalLoginInfo info)
        {
            Input = new InputModel { Email = info.Principal.FindFirstValue(ClaimTypes.Email) };
        }

        private async Task<IActionResult> RegisterUser(
            IdentityUser user,
            ExternalLoginInfo info,
            string returnUrl
        )
        {
            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                return await AddLoginAndConfirmEmail(user, info);
            }

            foreach (var error in result.Errors)
            {
                if (error.Code == "DuplicateUserName")
                {
                    ErrorMessage = "E-mail já cadastrado.";
                    return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
                }
            }
            ErrorMessage = "Erro desconhecido ;/ tente outro provedor";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        private async Task<IActionResult> AddLoginAndConfirmEmail(
            IdentityUser user,
            ExternalLoginInfo info
        )
        {
            var result = await _userManager.AddLoginAsync(user, info);
            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "User created an account using {Name} provider.",
                    info.LoginProvider
                );
                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    var emailConfirmationResult = await SendConfirmationEmail(user);
                    if (!emailConfirmationResult)
                        return RedirectToPage(
                            "./RegisterConfirmation",
                            new { Email = Input.Email }
                        );
                }

                await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                return Redirect("../../Perfil");
            }
            return null;
        }

        private async Task<bool> SendConfirmationEmail(IdentityUser user)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new
                {
                    area = "Identity",
                    userId = userId,
                    code = code
                },
                protocol: Request.Scheme
            );
            // await _emailSender.SendEmailAsync(
            //     Input.Email,
            //     "Confirm your email",
            //     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
            // );
            return _userManager.Options.SignIn.RequireConfirmedAccount;
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation(
                            "User created an account using {Name} provider.",
                            info.LoginProvider
                        );
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        await _signInManager.SignInAsync(
                            user,
                            isPersistent: false,
                            info.LoginProvider
                        );
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"Can't create an instance of '{nameof(IdentityUser)}'. "
                        + $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively "
                        + $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml"
                );
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException(
                    "The default UI requires a user store with email support."
                );
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
