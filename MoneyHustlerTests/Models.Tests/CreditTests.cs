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
                2 => new Credit("Микрокредитыч", 12, null, 200000, new Person() { Name = "Слава" }, new Card("Кошель", 1000000, 5), new DateTime(2023, 12, 21), new DateTime(2020, 11, 21)),
                _ => new Credit()
            };
            return credit;
        }


        [TestCase(1, 105508)]
        [TestCase(2, 233770)]

        public void PayMonthlyPaymentTest(int key, decimal expectedValue)
        {
            //arrange
            Credit credit = GetTestCredit(key);
            ExpenseType expenseType = new ExpenseType { Name = "Кредит" };

            //act
            credit.PayMonthlyPayment(expenseType);

            //assert
            Assert.AreEqual(expectedValue, Math.Round((decimal)credit.Amount, 0));
        }

        [TestCase(1, 76026, 20000)]
        [TestCase(2, 165479, 30000)]
        public void PayOneTimePaymentTest(int key, decimal expectedValue,  decimal payValue)
        {
            //arrange
            Credit credit = GetTestCredit(key);
            ExpenseType expenseType = new ExpenseType { Name = "Кредит" };
            //act
            credit.PayOneTimePayment(payValue, expenseType);

            //assert
            Assert.AreEqual(expectedValue, Math.Round((decimal)credit.Amount, 0));
        }

    }
}
