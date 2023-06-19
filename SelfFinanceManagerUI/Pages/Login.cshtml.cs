using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SelfFinanceManagerUI.Pages
{
    public class LoginModel : PageModel
    {
        public async Task<IActionResult> OnGet(string redirectUri)
        {
            await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties() { RedirectUri = redirectUri });
            return new EmptyResult();
        }
    }
}
