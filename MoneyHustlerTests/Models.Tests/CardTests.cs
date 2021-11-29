using NUnit.Framework;
using MoneyHustler.Models;
using System;
using System.Linq;

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

        [TestCase(50, 100, "You can't decrease your balance with amount more than current balance.")]
        public void DecreaseBalanseNegativeTest(decimal cardBalance, decimal expenseAmount, string expectedMessage)
        {
            //arrange
            Card card = new Card("Card", cardBalance, 0);
            Expense expense = new Expense();
            expense.Amount = expenseAmount;

            //act, assert

            Exception ex = Assert.Throws(typeof(ArgumentException), () => card.DecreaseBalance(expense));
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [TestCase(50, 1000)]

        public void IncreaseBalanceTest(decimal incomeAmount, decimal expectedBalance)
        {
            //arrange
            Card card1 = new Card();
            Card card = new Card("Card", 1000, 0);
            Income income = new Income();
            income.Amount = 100;
            income.Vault = card;
            income.Amount = incomeAmount;

            //act          
            card.IncreaseBalance(income);

            //assert
            Assert.AreEqual(card.GetBalance(), expectedBalance);
        }



        [TestCase(100, 50)]
        public void DecreaseBalanceWithCategoryTest(decimal cardBalance, decimal expenseAmount)
        {
            //arrange
            Card card = new Card("Card", cardBalance, 0);
            Expense expense = new Expense();
            expense.Amount = expenseAmount;

            //act          
            card.DecreaseBalance(expense);

            Storage _instance = Storage.GetInstance();
            var incomeTypeCashBack = _instance.IncomeTypes.FirstOrDefault(item => item.Name == "CashBack");


            //assert
            Assert.AreEqual(incomeTypeCashBack.Name, "CashBack");
        }


    }
}