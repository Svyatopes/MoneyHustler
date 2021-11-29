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
        public void DecreaseBalanseTest(decimal cardBalance, decimal expenseAmount, decimal expectedBalance)
        {
            //arrange
            Card card = new Card("Card", cardBalance, 0);
            Expense expense = new Expense();
            expense.Amount = expenseAmount;

            //act          
            card.DecreaseBalance(expense);

            //assert
            Assert.AreEqual(card.GetBalance(), expectedBalance);
        }

        [TestCase(100, 50, 150)]

        public void IncreaseBalanceTest(decimal cardBalance, decimal incomeAmount, decimal expectedBalance)
        {
            //arrange
            Card card = new Card("Card", cardBalance, 0);
            Income income = new Income();
            income.Amount = incomeAmount;

            //act          
            card.IncreaseBalance(income);

            //assert
            Assert.AreEqual(card.GetBalance(), expectedBalance);
        }
    }
}