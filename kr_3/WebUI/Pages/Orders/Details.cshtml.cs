using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public OrderViewModel? Order { get; set; }
        public string? ErrorMessage { get; set; }

        public DetailsModel(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task OnGetAsync(string orderId)
        {
            try
            {
                var client = _clientFactory.CreateClient("ApiGateway");

                var response = await client.GetAsync($"api/orders/{orderId}");

                if (response.IsSuccessStatusCode)
                {
                    Order = await response.Content.ReadFromJsonAsync<OrderViewModel>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ErrorMessage = "Заказ не найден";
                }
                else
                {
                    ErrorMessage = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Произошла ошибка: {ex.Message}";
            }
        }
    }
}
