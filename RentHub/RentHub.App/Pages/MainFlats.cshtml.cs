using Microsoft.AspNetCore.Mvc.RazorPages;
using RentHub.Core.Model;

namespace RentHub.App.Pages
{
    public class MainFlatsModel : PageModel
    {
        public List<Flat> Flats { get; set; } = new();
        public void OnGet()
        {
        }
    }
}
