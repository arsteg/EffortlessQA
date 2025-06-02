using System.Net.Http.Json;
using System.Text.Json;
using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Client.Services
{
    public class CommonService
    {
        private readonly HttpClient _httpClient;

        public CommonService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
        }

        public async Task<byte[]> GeneratePdfAsync(PdfGenerationDto pdfRequest)
        {
            try
            {
                var url = "common/generate-pdf";
                Console.WriteLine($"Calling PDF API: {url}");

                var response = await _httpClient.PostAsJsonAsync(url, pdfRequest);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine($"Response Content: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<
                        ApiResponse<object>
                    >(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    throw new HttpRequestException(
                        $"PDF generation failed: {errorResponse?.Error?.Code} - {errorResponse?.Error?.Message}",
                        null,
                        response.StatusCode
                    );
                }

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
                throw new Exception(
                    $"Failed to generate PDF: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw new Exception($"Unexpected error during PDF generation: {ex.Message}", ex);
            }
        }
    }
}
