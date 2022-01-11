namespace IdentityClientPrototype.Infrastructure
{
    public class CustomBackchannelHttpHandler : HttpMessageHandler
    {
        private const string AUTHORIZATION_KEY = "Authorization";

        private readonly IServiceCollection _services;

        public CustomBackchannelHttpHandler(IServiceCollection services)
        {
            _services = services;
        }

        private string GetToken()
        {
            var httpContextAccessor = _services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>();

            return httpContextAccessor.HttpContext?.Request.Headers.ContainsKey(AUTHORIZATION_KEY) ?? false
               ? httpContextAccessor.HttpContext.Request.Headers[AUTHORIZATION_KEY].ToString()
               : string.Empty;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var token = GetToken();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.BadRequest };

            var client = new HttpClient(new HttpClientHandler(), disposeHandler: false);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add(AUTHORIZATION_KEY, token);

            //NOTE: request.RequestUri = "https://localhost:7100/.well-known/openid-configuration"
            //var result = await client.GetAsync(request.RequestUri, ct);

            var result = await client.PostAsync("https://localhost:7100/api/v1/validate-token", request.Content, ct);
            
            return result;
        }
    }
}
