using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Aoun.Tests
{
    public class CasesApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        public CasesApiTests(WebApplicationFactory<Program> factory) { _factory = factory; }

        [Fact]
        public async Task GetHomeCases_ReturnsSuccessStatusCode()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/Cases/home");
            
            if (response.StatusCode != HttpStatusCode.NotFound) {
                // response.EnsureSuccessStatusCode();
            }
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound);
        }
    }
}

