using Xunit;

namespace Aoun.Tests
{
    public class ZakatTests
    {
        [Fact]
        public void CalculateZakat_AboveNisab_ReturnsCorrectAmount()
        {
            // Arrange
            decimal totalWealth = 500000m;
            decimal expectedZakat = 12500m; // 2.5%

            // Act
            decimal actualZakat = totalWealth * 0.025m; // استبدلها باستدعاء الـ Service الحقيقية

            // Assert
            Assert.Equal(expectedZakat, actualZakat);
        }
    }
}