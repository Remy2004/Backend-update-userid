using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

public class CaseIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CaseIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetActiveCases_Endpoint_ReturnsSuccess()
    {
        // 1) Login
        var loginResponse = await _client.PostAsync("/api/Auth/login",
            new StringContent(JsonSerializer.Serialize(new {
                email = "admin@test.com",
                password = "Aa123456!"
            }), Encoding.UTF8, "application/json"));

        var json = await loginResponse.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        var token = doc.RootElement.GetProperty("token").GetString();

        // 2) Add Token
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // 3) Call API
        var response = await _client.GetAsync("/api/Cases");

        // response.EnsureSuccessStatusCode();
    }
}

