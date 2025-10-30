
namespace MyApi.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var a = somma(1, 2);
            Assert.Equal(3, a);
        }

        private static int somma(int v1, int v2)
        {
            return v1 + v2;
        }
    }
}