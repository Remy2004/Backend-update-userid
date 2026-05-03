using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Aoun.Tests.IntegrationTests
{
    // نستخدم WebApplicationFactory لعمل محاكاة (In-Memory Server) للـ API الخاص بك
    public class CasesEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CasesEndpointTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetHomeCases_ReturnsSuccessStatusCode()
        {
            // Arrange (تجهيز عميل الـ HTTP الوهمي)
            var client = _factory.CreateClient();

            // Act (إرسال طلب GET لمسار الحالات في الصفحة الرئيسية)
            var response = await client.GetAsync("/api/Cases/home");

            // Assert (التحقق من أن الـ API رد بـ 200 OK ولم يضرب 500)
            // response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetNonExistentCase_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act (البحث عن حالة برقم مستحيل)
            var response = await client.GetAsync("/api/Cases/999999");

            // Assert (التأكد من أن السيرفر رد بـ 404 أو 204 كما هو مبرمج لديك وليس بـ 500)
            Assert.True(response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.NoContent);
        }
    }
}
