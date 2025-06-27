using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace EffortlessQA.UI.Services
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthTokenHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Optionally clear token and redirect to login
                await _localStorage.RemoveItemAsync("authToken");
                // You can use NavigationManager in a Blazor component, not here; handle in the calling component
                throw new HttpRequestException(
                    "Unauthorized. Please log in again.",
                    null,
                    System.Net.HttpStatusCode.Unauthorized
                );
            }

            return response;
        }
    }
}
