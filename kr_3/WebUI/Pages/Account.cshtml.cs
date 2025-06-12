using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages
{
    public class AccountModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public AccountViewModel? Account { get; set; }
        public string? ErrorMessage { get; set; }

        public AccountModel(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            await LoadAccountAsync();
        }

        public async Task<IActionResult> OnPostCreateAccountAsync()
        {
            try
            {
                var client = _clientFactory.CreateClient("ApiGateway");
                var userId = GetUserId();

                var request = new CreateAccountRequest { UserId = userId };
                var response = await client.PostAsJsonAsync("api/accounts", request);

                if (response.IsSuccessStatusCode)
                {
                    Account = await response.Content.ReadFromJsonAsync<AccountViewModel>();
                    return RedirectToPage();
                }
                else
                {
                    ErrorMessage = await response.Content.ReadAsStringAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Произошла ошибка: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostTopUpAccountAsync(decimal amount)
        {
            try
            {
                if (amount <= 0)
                {
                    ErrorMessage = "Сумма должна быть положительной";
                    await LoadAccountAsync();
                    return Page();
                }

                var client = _clientFactory.CreateClient("ApiGateway");
                var userId = GetUserId();

                Console.WriteLine($"Отправка запроса на пополнение счета: UserId={userId}, Amount={amount}");

                var request = new TopUpAccountRequest { UserId = userId, Amount = amount };

                client.DefaultRequestHeaders.Remove("X-User-Id");
                client.DefaultRequestHeaders.Add("X-User-Id", userId);

                var response = await client.PutAsJsonAsync("api/accounts/topup", request);

                Console.WriteLine($"Ответ: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    Account = await response.Content.ReadFromJsonAsync<AccountViewModel>();
                    return RedirectToPage();
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Ошибка: {content}");
                    ErrorMessage = $"Ошибка: {content}";
                    await LoadAccountAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Произошла ошибка: {ex.Message}";
                await LoadAccountAsync();
                return Page();
            }
        }


        private async Task LoadAccountAsync()
        {
            try
            {
                var client = _clientFactory.CreateClient("ApiGateway");
                var userId = GetUserId();

                var response = await client.GetAsync($"api/accounts/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    Account = await response.Content.ReadFromJsonAsync<AccountViewModel>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Account = null;
                }
                else
                {
                    ErrorMessage = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Произошла ошибка при загрузке данных счета: {ex.Message}";
            }
        }

        private string GetUserId()
        {
            return "user-123";
        }
    }

    public class AccountViewModel
    {
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateAccountRequest
    {
        public string UserId { get; set; }
    }

    public class TopUpAccountRequest
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
    }
}
