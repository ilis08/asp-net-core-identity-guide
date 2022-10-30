using identity_guide_1.Models;
using identity_guide_2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using NuGet.Common;
using System.Net.Http.Headers;
using Token = identity_guide_1.Models.Token;

namespace identity_guide_1.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory client;

        [BindProperty]
        public List<WeatherForecastDTO>? Forecasts { get; set; }

        public HRManagerModel(IHttpClientFactory _client)
        {
            client = _client;
        }

        public async Task OnGet()
        {
            var httpClient = client.CreateClient("OurWebAPI");

            var result = await httpClient.PostAsJsonAsync("auth", new Credential { UserName = "admin",Password = "admin", RememberMe = true });

            var token = JsonConvert.DeserializeObject<Token>(await result.Content.ReadAsStringAsync());

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            Forecasts = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast");
        }
    }
}
