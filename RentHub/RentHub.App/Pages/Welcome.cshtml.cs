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
            BaseAddress = new Uri("http://94.183.186.221:5000/")
        };
        public IActionResult OnGet()
        {
            string? token = Request.Cookies["jwt"];

            if (!string.IsNullOrEmpty(token))
            {
                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    Secure = true,
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

                var response = await client.PostAsync($"Auth/email_exists", formData);

                response.EnsureSuccessStatusCode();

                var json2 = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<EmailExistsResponse>(json2);

                TempData["Email"] = Email;
                TempData["exists"] = result.exists ? "true" : "false";
                return RedirectToPage("/RegisLogIn");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка проверки email.");
                return Page();
            }
        }
    }
}
