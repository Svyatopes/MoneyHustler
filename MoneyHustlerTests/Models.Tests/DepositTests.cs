using NUnit.Framework;
using MoneyHustler.Models;
using System;

namespace MoneyHustlerTests.Models.Tests
{
    class DepositTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [TestCase("Namename", 2, "2020-06-29")] //был баланс 100
        public void MainDepositTest(string name, decimal percent, DateTime openDate)
        {
            //arrange
            Deposit deposit1 = new Deposit(name, percent, openDate);
            Deposit deposit2 = new Deposit();
            deposit2.Name = name;
            deposit2.Percent = percent;
            deposit2.OpenDate = openDate;
            deposit2.PaymentDay = deposit1.PaymentDay;

            Deposit deposit3 = new Deposit();
            deposit3.Name = deposit2.Name;
            deposit3.Percent = deposit2.Percent;
            deposit3.OpenDate = deposit2.OpenDate;

            //act, assert          
            Assert.AreEqual(deposit2.Name, deposit1.Name);
            Assert.AreEqual(deposit1.Name, deposit3.Name);
        }
    }
}
