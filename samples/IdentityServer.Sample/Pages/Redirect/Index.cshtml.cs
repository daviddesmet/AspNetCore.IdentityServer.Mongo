namespace IdentityServer.Sample.Pages.Redirect
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class IndexModel : PageModel
    {
        public string RedirectUri { get; set; }

        public IActionResult OnGet(string redirectUri)
        {
            if (!Url.IsLocalUrl(redirectUri))
            {
                return RedirectToPage("/Error/Index");
            }

            RedirectUri = redirectUri;
            return Page();
        }
    }
}
