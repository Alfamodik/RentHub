using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace RentHub.App.Pages
{

    public class EmailExistsResponse
    {
        public bool exists { get; set; }
    }
    public class WelcomeModel : PageModel
    {
        HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:5188/")
        };

        public IActionResult OnGet()
        {
            string? token = Request.Cookies["jwt"];

            if (!string.IsNullOrEmpty(token))
            {
                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(60)
                });

                return RedirectToPage("/MainFlats");
            }

            return Page();
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        public async Task<IActionResult> OnPost()
        {
            try
            {
                var loginData = new
                {
                    email = Email,

                };

                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(Email), "email");

                var response = await client.PostAsync($"Authentication/email_exists", formData);

                response.EnsureSuccessStatusCode();

                var json2 = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<EmailExistsResponse>(json2);

                TempData["Email"] = Email;
                TempData["exists"] = result.exists ? "true" : "false";
                return RedirectToPage("/RegisLogIn");

            }
            catch
            {
                ModelState.AddModelError("", "Îøèáêà ïðîâåðêè email.");
                return Page();
            }
        }
    }
}
