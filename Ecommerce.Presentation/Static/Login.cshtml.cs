using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Web;

namespace Ecommerce.Presentation.Static
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
            // This method handles GET requests
        }

        public IActionResult OnPost()
        {
            // This method handles POST requests

            // Validate the username and password (replace this with your actual authentication logic)
            if (Username == "yourusername" && Password == "yourpassword")
            {
                // Successful login, redirect to a different page
                return RedirectToPage("/Index");
            }
            else
            {
                // Failed login, return to the login page
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }
    }
}
