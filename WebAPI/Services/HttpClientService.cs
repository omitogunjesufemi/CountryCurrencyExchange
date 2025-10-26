namespace WebAPI.Services
{
    public class HttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetCountryAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("CountryDetails");
                var httpResponse = await httpClient.GetAsync("");
                httpResponse.EnsureSuccessStatusCode();
                var content = await httpResponse.Content.ReadAsStringAsync();
                return content;
            }
            catch (HttpRequestException hEx)
            {
                return $"Error fetching country details: {hEx.Message}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex}";
            }
        }

        public async Task<string> GetExchangeRatesAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("ExchangeRates");
                var httpResponse = await httpClient.GetAsync($"");
                httpResponse.EnsureSuccessStatusCode();
                var content = await httpResponse.Content.ReadAsStringAsync();
                return content;
            }
            catch (HttpRequestException hEx)
            {
                return $"Error fetching exchange rate: {hEx.Message}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex}";
            }
        }
     }
}
