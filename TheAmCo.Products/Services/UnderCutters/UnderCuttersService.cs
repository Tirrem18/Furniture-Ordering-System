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

        public async Task<IEnumerable<TheAmCo.Products.Data.Products.Product>> GetProductsAsync()
        {
            var uri = "api/product";
            int maxRetries = 10;
            double delayFactor = 2; 

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // Make the HTTP GET request
                    var response = await _client.GetAsync(uri);
                    response.EnsureSuccessStatusCode();

                    // Deserialize the response content into ProductDto
                    var products = await response.Content.ReadFromJsonAsync<IEnumerable<TheAmCo.Products.Data.Products.Product>>();
                    return products ?? Enumerable.Empty<TheAmCo.Products.Data.Products.Product>();
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
            
            return Enumerable.Empty<TheAmCo.Products.Data.Products.Product>();
        }
    }
}
