using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
        public string? ErrorMessage { get; set; }

        public IndexModel(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            await LoadOrdersAsync();
        }

        public async Task<IActionResult> OnPostAsync(decimal Amount, string Description)
        {
            try
            {
                if (Amount <= 0)
                {
                    ErrorMessage = "Сумма должна быть положительной";
                    await LoadOrdersAsync();
                    return Page();
                }

                var client = _clientFactory.CreateClient("ApiGateway");
                var userId = GetUserId();

                var request = new CreateOrderRequest
                {
                    UserId = userId,
                    Amount = Amount,
                    Description = Description
                };

                var response = await client.PostAsJsonAsync("api/orders", request);

                if (response.IsSuccessStatusCode)
                {
                    var newOrder = await response.Content.ReadFromJsonAsync<OrderViewModel>();
                    return RedirectToPage();
                }
                else
                {
                    ErrorMessage = await response.Content.ReadAsStringAsync();
                    await LoadOrdersAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Произошла ошибка: {ex.Message}";
                await LoadOrdersAsync();
                return Page();
            }
        }

        private async Task LoadOrdersAsync()
        {
            try
            {
                var client = _clientFactory.CreateClient("ApiGateway");
                var userId = GetUserId();
                client.DefaultRequestHeaders.Remove("X-User-Id");
                client.DefaultRequestHeaders.Add("X-User-Id", userId);
                var response = await client.GetAsync($"api/orders/user/{userId}");

                Console.WriteLine($"Ответ сервера: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Содержимое: {content}");

                    try
                    {
                        Orders = System.Text.Json.JsonSerializer.Deserialize<List<OrderViewModel>>(content,
                            new System.Text.Json.JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            }) ?? new List<OrderViewModel>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка десериализации: {ex.Message}");
                        ErrorMessage = $"Ошибка десериализации: {ex.Message}";
                        Orders = new List<OrderViewModel>();
                    }
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Ошибка: {content}";
                    Console.WriteLine($"Ошибка: {content}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Произошла ошибка при загрузке заказов: {ex.Message}";
                Console.WriteLine($"Исключение: {ex.Message}");
            }
        }


        private string GetUserId()
        {
            return "user-123";
        }
    }

    public class OrderViewModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }



    public class CreateOrderRequest
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
