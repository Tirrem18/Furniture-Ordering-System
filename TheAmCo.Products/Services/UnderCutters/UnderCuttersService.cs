namespace TheAmCo.Products.Services.UnderCutters
{
    public class UnderCuttersService : IUnderCuttersService
    {
        private readonly HttpClient _client;

        public UnderCuttersService(HttpClient client, IConfiguration configuration)
        {
            var baseUrl = configuration["WebServices:UnderCutters:BaseURL"] ?? "";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30); // Set a higher timeout to accommodate retries
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = client;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var uri = "api/product";
            int maxRetries = 10;
            double delayFactor = 2; // Exponential backoff multiplier

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // Make the HTTP GET request
                    var response = await _client.GetAsync(uri);
                    response.EnsureSuccessStatusCode();

                    // Deserialize the response content into ProductDto
                    var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
                    return products ?? Enumerable.Empty<ProductDto>();
                }
                catch (HttpRequestException ex) when (attempt < maxRetries)
                {
                    Console.WriteLine($"Retry attempt {attempt} failed: {ex.Message}");
                    var delay = TimeSpan.FromSeconds(Math.Pow(delayFactor, attempt));
                    Console.WriteLine($"Waiting {delay.TotalSeconds} seconds before retry...");
                    await Task.Delay(delay);
                }
            }

            // Throw an exception if all retry attempts fail
            throw new HttpRequestException($"Failed to fetch products from UnderCutters API after {maxRetries} attempts.");
        }
    }
}
