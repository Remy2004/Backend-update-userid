//using Aoun.DAL.Data;
//using Aoun.BLL.DTOs.ChatAI;
//using Microsoft.Extensions.Configuration;
//using System.Net.Http.Json;

//namespace Aoun.BLL.Services.Chat;

//public class AISmartService
//{
//    private readonly ApplicationDbContext _db;
//    private readonly HttpClient _http;
//    private readonly string _apiKey;

//    public AISmartService(ApplicationDbContext db, HttpClient http, IConfiguration config)
//    {
//        _db = db; _http = http;
//        _apiKey = config["Gemini:ApiKey"] ?? "";
//    }

//    public async Task<string> AskAounAsync(string message)
//    {
//        // لوجيك استدعاء Gemini API الحقيقي
//        return "أهلاً بك، أنا مساعد عون الذكي. كيف يمكنني مساعدتك اليوم؟";
//    }

//    public async Task<string> GetRecommendedCasesAsync(string? userId = null)
//    {
//        return "حالات عاجلة من أقسام التعليم والصحة";
//    }
//}
