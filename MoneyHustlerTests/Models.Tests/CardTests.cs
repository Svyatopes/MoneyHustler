using NUnit.Framework;
using MoneyHustler.Models;

namespace MoneyHustlerTests.Models.Tests
{
    public class CardTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(100, 50, 50)] //был баланс 100
        public void DecreaseBalanseTest(int cardBalance, int expense, int expectedBalance)
        {
            //arrange
            Card card = new Card("Card", cardBalance, 0);

            //act          
            card.DecreaseBalanse(expense);

            //assert
            Assert.AreEqual(card.GetBalance, expectedBalance);
        }
    }
}