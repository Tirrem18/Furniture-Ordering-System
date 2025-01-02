namespace TheAmCo.Products.Services.DodgeyDealers;
    public class DodgyDealersService : IDodgyDealersService
    {
        private readonly HttpClient _client;

       public DodgyDealersService(HttpClient client, IConfiguration configuration)
        {
            var baseUrl = configuration["WebServices:DodgeyDealers:BaseURL"] ?? "";
            client.BaseAddress = new System.Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = client;
        }


        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var uri = "api/product";
            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            
            var products = await response.Content.ReadAsAsync<IEnumerable<ProductDto>>();
            return products;
        }

    }
