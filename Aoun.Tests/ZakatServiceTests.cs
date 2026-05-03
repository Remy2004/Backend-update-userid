using Xunit;
using Aoun.BLL.Services; // تأكد من استدعاء مسار خدماتك الصحيح
using Aoun.BLL.DTOs;

namespace Aoun.Tests.UnitTests
{
    public class ZakatServiceTests
    {
        // اختبار حساب الزكاة لمبلغ تجاوز النصاب
        [Fact]
        public void CalculateZakat_AboveNisab_ReturnsCorrectZakatAmount()
        {
            // Arrange (تجهيز البيانات)
            // لنفترض أن النصاب 255,000 وأن الممتلكات الصافية 500,000
            decimal cash = 500000m;
            decimal debts = 0m;
            decimal expectedZakat = 12500m; // 2.5% من 500,000

            // Act (تنفيذ العملية)
            // قم باستبدال هذا بالـ Service الحقيقية لديك
            decimal netWealth = cash - debts;
            decimal actualZakat = (netWealth >= 255000m) ? netWealth * 0.025m : 0m;

            // Assert (التحقق من النتيجة)
            Assert.Equal(expectedZakat, actualZakat);
        }

        // اختبار حساب الزكاة لمبلغ أقل من النصاب
        [Fact]
        public void CalculateZakat_BelowNisab_ReturnsZero()
        {
            // Arrange
            decimal cash = 100000m; // أقل من النصاب
            decimal expectedZakat = 0m;

            // Act
            decimal actualZakat = (cash >= 255000m) ? cash * 0.025m : 0m;

            // Assert
            Assert.Equal(expectedZakat, actualZakat);
        }
    }
}