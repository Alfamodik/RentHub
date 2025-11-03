using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RentHub.App.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public string Email { get; set; } = string.Empty;

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page(); // Показать ошибки
        }
        bool isExist = false;
        TempData["Email"] = Email; 
        return RedirectToPage("/Pages/RegisLogIn");
    }
}
