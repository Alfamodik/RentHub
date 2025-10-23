using Microsoft.Playwright;

namespace RentHub.API.Services
{
    public static class SutochnoParser
    {
        public async static Task<string> Get()
        {
            var url = "https://sutochno.ru/front/searchapp/detail/1966081";
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            var page = await browser.NewPageAsync();
            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            var html = await page.ContentAsync();
            return html;
        }
    }
}
