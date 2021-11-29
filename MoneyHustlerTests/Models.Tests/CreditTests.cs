using MoneyHustler.Models;
using NUnit.Framework;
using System;

namespace MoneyHustlerTests.Models.Tests
{

    public class CreditTests
    {
        [SetUp]
        public void Setup()
        {
        }

        public Credit GetTestCredit(int key)
        {
            Credit credit = key switch
            {
                1 => new Credit("Микрокредитыч", 10, null, 100000, new Person() { Name = "Антон" }, new Card("Кошель", 1000000, 5), new DateTime(2023, 11, 21), new DateTime(2021, 12, 21)),
                2 => new Credit("Микрокредитыч", 12, null, 200000, new Person() { Name = "Антон" }, new Card("Кошель", 1000000, 5), new DateTime(2023, 12, 21), new DateTime(2020, 11, 21)),
                _ => new Credit()
            };
            return credit;
        }


        [TestCase(1, 100304.10, 10000)]
        [TestCase(2, 235263.56, 5000)]


        public void DecreaseValueTest(int key, decimal expectedValue, decimal expenseAmount)
        {
            //arrange
            Credit credit = GetTestCredit(key);
            Expense expense = new Expense();
            expense.Amount = expenseAmount;

            //act
            credit.DecreaseValue(expense);

            //assert
            Assert.AreEqual(expectedValue, Math.Round((decimal)credit.Value, 2));

        }


        [TestCase(1, 23)]
        [TestCase(2, 37)]

        public void GetMounthPeriodTest(int key, int expectedPeriod)
        {
            //arrange
            Credit credit = GetTestCredit(key);

            //act
            int period = credit.GetMounthPeriod();

            //assert
            Assert.AreEqual(expectedPeriod, period);

        }

        [TestCase(1, 4796)]
        [TestCase(2, 6494)]
        public void SetMonthlyPaymentTest(int key, decimal expectedPayValue)
        {
            //arrange
            Credit credit = GetTestCredit(key);

            //act
            credit.SetMonthlyPayment();

            //assert
            Assert.AreEqual(expectedPayValue, Math.Round((decimal)credit.AnnuentPayValue, 0));
        }

        [TestCase(1, 105508)]
        [TestCase(2, 233770)]

        public void PayMonthlyPaymentTest(int key, decimal expectedValue)
        {
            //arrange
            Credit credit = GetTestCredit(key);

            //act
            credit.PayMonthlyPayment();

            //assert
            Assert.AreEqual(expectedValue, Math.Round((decimal)credit.Value, 0));
        }

        [TestCase(1, 90304, 20000)]
        [TestCase(2, 230264, 10000)]
        public void PayOneTimePaymentTest(int key, decimal expectedValue,  decimal payValue)
        {
            //arrange
            Credit credit = GetTestCredit(key);
            Expense expense = new Expense();
            expense.Amount = payValue;
            //act
            credit.PayOneTimePayment(payValue);

            //assert
            Assert.AreEqual(expectedValue, Math.Round((decimal)credit.Value, 0));
        }

    }
}
