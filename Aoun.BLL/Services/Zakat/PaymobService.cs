using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Aoun.BLL.Services.Zakat
{
    public class PaymobService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public PaymobService(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<string> CreateCheckoutSession(decimal amount, string userId)
        {
            var apiKey = _config["Paymob:ApiKey"];
            var integrationId = _config["Paymob:IntegrationId"];
            var iframeId = _config["Paymob:IframeId"];
            var amountCents = (int)(amount * 100); // تحويل المبلغ للقروش

            // 1. Authentication: الحصول على توكن الوصول
            var authRes = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/auth/tokens", new { api_key = apiKey });
            var authData = await authRes.Content.ReadFromJsonAsync<JsonElement>();
            var token = authData.GetProperty("token").GetString();

            // 2. Order Registration: تسجيل الطلب
            var orderRes = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/ecommerce/orders", new
            {
                auth_token = token,
                delivery_needed = "false",
                amount_cents = amountCents,
                currency = "EGP",
                merchant_order_id = Guid.NewGuid().ToString(), // رقم تعريفي فريد للطلب
                items = new List<object>()
            });
            var orderData = await orderRes.Content.ReadFromJsonAsync<JsonElement>();
            var orderId = orderData.GetProperty("id").GetInt32();

            // 3. Payment Key Generation: مفتاح الدفع النهائي
            var paymentKeyRes = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/acceptance/payment_keys", new
            {
                auth_token = token,
                amount_cents = amountCents,
                expiration = 3600,
                order_id = orderId,
                billing_data = new
                {
                    // Paymob يطلب هذه البيانات إجبارياً حتى لو كانت وهمية
                    apartment = "NA",
                    email = "donor@aoun.com",
                    floor = "NA",
                    first_name = "Aoun",
                    street = "NA",
                    building = "NA",
                    phone_number = "01000000000",
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "Cairo",
                    country = "EG",
                    last_name = "User",
                    state = "NA"
                },
                currency = "EGP",
                integration_id = int.Parse(integrationId!)
            });
            var paymentKeyData = await paymentKeyRes.Content.ReadFromJsonAsync<JsonElement>();
            var paymentToken = paymentKeyData.GetProperty("token").GetString();

            // إرجاع الرابط النهائي الذي سيتم تحويل المستخدم إليه (أو فتحه في Iframe)
            return $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentToken}";
        }
    }
}
