using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SelfFinanceManagerUI.Pages
{
    public class LogoutModel : PageModel
    {
        [Authorize]
        public async Task OnGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("Auth0", new AuthenticationProperties { RedirectUri = "/" });
        }
    }
}
