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
            //get token from session 

            Token token = null;

            var strTokenObj = HttpContext.Session.GetString("access_token");

            if (string.IsNullOrWhiteSpace(strTokenObj))
            {
                token = await Authenticate();
            }
            else
            {
                token = JsonConvert.DeserializeObject<Token>(strTokenObj);
            }

            if (token == null || string.IsNullOrWhiteSpace(token.AccessToken) || token.ExpiresAt <= DateTime.UtcNow)
            {
                token = await Authenticate();
            }

            var httpClient = client.CreateClient("OurWebAPI");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            Forecasts = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast");
        }

        private async Task<Token> Authenticate()
        {
            //authentication & getting the token
            var httpClient = client.CreateClient("OurWebAPI");

            var result = await httpClient.PostAsJsonAsync("auth", new Credential { UserName = "admin", Password = "admin", RememberMe = true });

            var strJwt = await result.Content.ReadAsStringAsync();

            HttpContext.Session.SetString("access_token", strJwt);

            return JsonConvert.DeserializeObject<Token>(strJwt);
        }
    }
}
