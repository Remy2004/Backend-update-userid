using Aoun.BLL.DTOs.Zakat;
using System.Net.Http.Json;

namespace Aoun.BLL.Services.Zakat;

public class ZakatService
{
    private readonly HttpClient _http;
    private readonly MetalPriceService _metal;
    public ZakatService(MetalPriceService metal, HttpClient http)

    {
        _metal = metal;
        _http = http;
    }


    public async Task<object> Calculate(ZakatFullDto dto)
    {
        var (goldPrice24, silverPrice) = await _metal.GetPrices();
        decimal goldPrice21 = goldPrice24 * 21 / 24;
        decimal goldPrice18 = goldPrice24 * 18 / 24;
        decimal goldValue = (dto.Gold24 * goldPrice24) +
                        (dto.Gold21 * goldPrice21) +
                        (dto.Gold18 * goldPrice18);
        decimal silverValue = dto.SilverGrams * silverPrice;
        var total = dto.Cash + dto.Bank + dto.Investments + goldValue + silverValue;
        var net = total - dto.Debts;
        var nisab = (85 * goldPrice24);
        bool isEligible = net >= nisab;

        if (net < nisab)
    {
        return new { 
            zakat = 0, 
            net, 
            nisab,
            prices = new
            {
                gold24 = Math.Round(goldPrice24, 2),
                gold21 = Math.Round(goldPrice21, 2),
                gold18 = Math.Round(goldPrice18, 2),
                silver = Math.Round(silverPrice, 2)
            },
            isEligible = false 
        };
    }

    decimal zakat = net * 0.025m;

    return new
    {
        zakat = Math.Round(zakat, 2),
        net,
        nisab,
        prices = new
        {                      
            gold24 = Math.Round(goldPrice24, 2),
            gold21 = Math.Round(goldPrice21, 2),
            gold18 = Math.Round(goldPrice18, 2),
            silver = Math.Round(silverPrice, 2)
        },
        isEligible = true,
        message = "The net amount has not reached the Nisab limit."
    };
    }
}