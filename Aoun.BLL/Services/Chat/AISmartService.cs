using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Aoun.BLL.Services.Chat
{
    public class AISmartService
    {
        private readonly HttpClient _http;
        private readonly string _geminiKey;
        private readonly string _openAiKey;
        private readonly string _deepSeekKey;
        private readonly string _groqKey;
        private string _workingModelName = string.Empty;


        public AISmartService(HttpClient http, IConfiguration config)
        {
            _http = http;

            _geminiKey = "";
            _openAiKey = "";
            _deepSeekKey = "";
            _groqKey = "";
        }

        private async Task<string> AskAIWithFallbackAsync(string systemPrompt, string userPrompt)
        {
            string fullPrompt = $"{systemPrompt}\n\n{userPrompt}";
            List<string> errors = new List<string>();

            // 1. Groq
            if (!string.IsNullOrEmpty(_groqKey))
            {
                try
                {
                    Console.WriteLine("🤖 [Routing] Trying Groq (Llama 70B)...");
                    return await AskOpenAICompatibleAsync(_groqKey, "https://api.groq.com/openai/v1/chat/completions", "llama-3.3-70b-versatile", systemPrompt, userPrompt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Groq Failed: {ex.Message}");
                    errors.Add($"Groq: {ex.Message}");
                }
            }

            // 2. Gemini
            if (string.IsNullOrEmpty(_workingModelName))
            {
                try
                {
                    string listModelsUrl = $"https://generativelanguage.googleapis.com/v1beta/models?key={_geminiKey}";
                    var listResponse = await _http.GetAsync(listModelsUrl);

                    if (!listResponse.IsSuccessStatusCode)
                        throw new Exception("فشل الاتصال بجوجل لجلب النماذج!");

                    var listJson = await listResponse.Content.ReadFromJsonAsync<JsonElement>();

                    foreach (var model in listJson.GetProperty("models").EnumerateArray())
                    {
                        var name = model.GetProperty("name").GetString();
                        var methods = model.GetProperty("supportedGenerationMethods").EnumerateArray().Select(m => m.GetString()).ToList();

                        if (methods.Contains("generateContent") && name != null && name.StartsWith("models/gemini"))
                        {
                            _workingModelName = name;
                            Console.WriteLine($"✅ Auto-Discovered Working Model: {_workingModelName}");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrEmpty(_workingModelName))
                        throw new Exception("مفتاحك لا يملك صلاحية لأي نموذج من نماذج جيميناي!");
                }

                // 3. OpenAI
                if (!string.IsNullOrEmpty(_openAiKey))
                {
                    try
                    {
                        Console.WriteLine("🤖 [Routing] Trying OpenAI (ChatGPT)...");
                        return await AskOpenAICompatibleAsync(_openAiKey, "https://api.openai.com/v1/chat/completions", "gpt-4o-mini", systemPrompt, userPrompt);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ OpenAI Failed: {ex.Message}");
                        errors.Add($"OpenAI: {ex.Message}");
                    }
                }

                // 4. DeepSeek
                if (!string.IsNullOrEmpty(_deepSeekKey))
                {
                    try
                    {
                        Console.WriteLine("🤖 [Routing] Trying DeepSeek...");
                        return await AskOpenAICompatibleAsync(_deepSeekKey, "https://api.deepseek.com/chat/completions", "deepseek-chat", systemPrompt, userPrompt);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ DeepSeek Failed: {ex.Message}");
                        errors.Add($"DeepSeek: {ex.Message}");
                    }
                }

                throw new Exception($"❌ All AI providers failed! Errors log: {string.Join(" | ", errors)}");
            }

            // ✅ الحل الوحيد للمشكلة
            throw new Exception("Unexpected error: no AI provider executed.");
        }

        private async Task<string> AskGeminiAsync(string prompt)
        {
            string requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_geminiKey}";
            var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) throw new Exception(await response.Content.ReadAsStringAsync());

            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            return jsonResponse.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "";
        }

        private async Task<string> AskOpenAICompatibleAsync(string apiKey, string url, string modelName, string sysPrompt, string usrPrompt)
        {
            string strictSystemPrompt = sysPrompt + "\nتعليمات صارمة: لا تذكر أبداً أنك ذكاء اصطناعي أو روبوت أو نموذج لغوي...";

            var requestBody = new
            {
                model = modelName,
                messages = new[] {
                    new { role = "system", content = strictSystemPrompt },
                    new { role = "user", content = usrPrompt }
                },
                temperature = 0.3,
                max_tokens = 500
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) throw new Exception(await response.Content.ReadAsStringAsync());

            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            return jsonResponse.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
        }

        public async Task<string> GetRecommendationsAsync(string userHistory, string availableCases)
        {
            string sysPrompt = "أنت موظف بشري ودود في خدمة عملاء منصة 'عون'...";
            string usrPrompt = $"تاريخ المتبرع: {userHistory}\nالحالات المتاحة حالياً: {availableCases}";
            return await AskAIWithFallbackAsync(sysPrompt, usrPrompt);
        }

        public async Task<string> ChatWithRAGAsync(string userQuestion, string dbContext)
        {
            string sysPrompt = $"أنت إنسان طبيعي تعمل في منصة (عون)... {dbContext}";
            string usrPrompt = $"السؤال: {userQuestion}";
            return await AskAIWithFallbackAsync(sysPrompt, usrPrompt);
        }

        public async Task<string> GenerateCaseDescriptionAsync(string title, string category, decimal requiredAmount)
        {
            string sysPrompt = "اكتب وصفاً تسويقياً...";
            string usrPrompt = $"العنوان: {title} | التصنيف: {category} | المبلغ: {requiredAmount}";
            return await AskAIWithFallbackAsync(sysPrompt, usrPrompt);
        }
    }
}