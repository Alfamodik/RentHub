using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text;
using System.Text.Json;

namespace RentHub.App.Pages
{
    public class RegisLogInModel : PageModel
    {
        private readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://94.183.186.221:5000/")
        };

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public bool emailExists { get; set; }

        public void OnGet()
        {
            // восстанавливаем данные из TempData
            if (TempData.TryGetValue("Email", out var e))
                Email = e?.ToString() ?? string.Empty;

            if (TempData.TryGetValue("exists", out var ex))
                emailExists = string.Equals(ex?.ToString(), "true", StringComparison.OrdinalIgnoreCase);

            //TempData["Message"] = $"{emailExists}";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                HttpResponseMessage response;

                if (emailExists)
                {
                    // 🔹 Существует → логин
                    var loginData = new
                    {
                        email = Email,
                        password = Password
                    };

                    string json = JsonSerializer.Serialize(loginData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    response = await client.PostAsync("Auth/login", content);
                }
                else
                {
                    // 🔹 Не существует → регистрация
                    var regData = new
                    {
                        email = Email,
                        password = Password
                    };

                    string json = JsonSerializer.Serialize(regData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    response = await client.PostAsync("Auth/Register", content);
                }

                string body = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    TempData["Message"] = "Этот email уже зарегистрирован.";
                    return Page();
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["Message"] = "Неверный email или пароль.";
                    return Page();
                }
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    TempData["Message"] = "Введите корректные данные.";
                    return Page();
                }
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Message"] = $"Ошибка API: {response.StatusCode}. {body}";
                    return Page();
                }

                string? jwt = JsonSerializer.Deserialize<JsonElement>(body).GetProperty("token").GetString();
                Response.Cookies.Append("jwt", jwt, new CookieOptions
                {
                    Secure = true,
                    Expires = DateTime.Now.AddMinutes(60)
                });
                TempData["Message"] = emailExists ? "Успешный вход!" : "Регистрация успешна!";
                return RedirectToPage("/MainFlats");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Ошибка: " + ex.Message;
                return Page();
            }
        }
    }
}
