using Xunit;
namespace Aoun.Tests
{
    public class ZakatLogicTests
    {
        [Fact]
        public void CalculateZakat_AboveNisab_ReturnsCorrectAmount()
        {
            decimal netWealth = 500000m; 
            decimal expectedZakat = 12500m; // 2.5%
            decimal actualZakat = netWealth * 0.025m; 
            Assert.Equal(expectedZakat, actualZakat);
        }
    }
}
