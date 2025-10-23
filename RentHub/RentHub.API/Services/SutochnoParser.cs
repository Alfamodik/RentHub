using Microsoft.Playwright;
using RentHub.Core.Model;
using System.Globalization;

namespace RentHub.API.Services
{
    public static class SutochnoParser
    {
        public async static Task<List<Reservation>?> GetReservations(string url)
        {
            using IPlaywright playwright = await Playwright.CreateAsync();

            await using IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            IBrowserContext context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118 Safari/537.36"
            });

            IPage page = await context.NewPageAsync();

            try
            {
                await page.GotoAsync(url, new() { WaitUntil = WaitUntilState.DOMContentLoaded });
            }
            catch
            {
                return null;
            }

            IElementHandle? checkInCheckOut = await page.WaitForSelectorAsync(".aside__dates.dates");

            if (checkInCheckOut == null)
                return null;

            IElementHandle? checkIn = await checkInCheckOut.WaitForSelectorAsync(".dates__item.dates__item_in");

            if (checkIn == null)
                return null;

            await checkIn.ClickAsync();

            IElementHandle? calendar = await checkInCheckOut.WaitForSelectorAsync(".sc-datepickerext-wrapper.desktop.sc-datepickerext");

            if (calendar == null)
                return null;

            List<DateState> dateStates = [];
            bool nextButtonEmpty = false;

            do
            {
                IReadOnlyList<IElementHandle> months = await calendar.QuerySelectorAllAsync(".sc-datepickerext-month.privet");

                foreach (IElementHandle month in months)
                {
                    if (month == null)
                        return null;

                    IReadOnlyList<IElementHandle> dayCells = await month.QuerySelectorAllAsync("td[data-cy='sc-datepickerext-day']");
                    await ParseDateStates(dayCells, dateStates);
                }

                IElementHandle? nextButton = await calendar.WaitForSelectorAsync(".sc-datepickerext-wrapper-next");

                if (nextButton == null)
                    return null;

                nextButtonEmpty = await nextButton.EvaluateAsync<bool>("el => el.classList.contains('empty')");

                if (!nextButtonEmpty)
                    await nextButton.ClickAsync();
            }
            while (!nextButtonEmpty);
            
            return CreateReservations(dateStates);
        }

        private static List<Reservation> CreateReservations(List<DateState> dateStates)
        {
            List<Reservation> reservations = [];
            Reservation? reservation = null;

            foreach (DateState dateState in dateStates)
            {
                if (dateState.State == DateState.DateStates.CheckIn)
                {
                    reservation = new()
                    {
                        DateOfStartReservation = dateState.Date
                    };

                    reservations.Add(reservation);
                }
                else if (dateState.State == DateState.DateStates.CheckOut && reservation != null)
                {
                    reservation.DateOfEndReservation = dateState.Date;
                }
            }

            return reservations;
        }

        private static async Task ParseDateStates(IEnumerable<IElementHandle> elementHandles, List<DateState> dateStates)
        {
            foreach (IElementHandle cell in elementHandles)
            {
                string? cellDate = await cell.GetAttributeAsync("data-cy-date");

                if (string.IsNullOrEmpty(cellDate))
                    continue;

                if (!TryParseDate(cellDate, out DateOnly cellDateOnly))
                    continue;

                string classes = await cell.GetAttributeAsync("class") ?? "";

                if (classes.Contains("sc-datepickerext-disabled"))
                {
                    if (cellDateOnly < DateOnly.FromDateTime(DateTime.Now))
                        dateStates.Add(new DateState(cellDateOnly, DateState.DateStates.Past));
                    else
                        dateStates.Add(new DateState(cellDateOnly, DateState.DateStates.NotAvailable));
                }

                //else if (classes.Contains("sc-datepickerext-current-date")) { }

                else if (classes.Contains("sc-datepickerext-day-employment__right"))
                    dateStates.Add(new DateState(cellDateOnly, DateState.DateStates.CheckIn));

                else if (classes.Contains("sc-datepickerext-day-employment__both"))
                    dateStates.Add(new DateState(cellDateOnly, DateState.DateStates.Stay));

                else if (classes.Contains("sc-datepickerext-day-employment__left"))
                    dateStates.Add(new DateState(cellDateOnly, DateState.DateStates.CheckOut));

                else
                    dateStates.Add(new DateState(cellDateOnly, DateState.DateStates.Free));
            }
        }

        private static bool TryParseDate(string date, out DateOnly result)
        {
            if (DateTime.TryParseExact(date, "dd.MM.yyyy", null, DateTimeStyles.None, out var dateTime))
            {
                result = DateOnly.FromDateTime(dateTime);
                return true;
            }

            result = default;
            return false;
        }

        private class DateState(DateOnly date, DateState.DateStates state)
        {
            public DateOnly Date = date;
            public DateStates State = state;

            public enum DateStates
            {
                Past,
                Free,
                CheckIn,
                Stay,
                CheckOut,
                NotAvailable
            }
        }
    }
}
