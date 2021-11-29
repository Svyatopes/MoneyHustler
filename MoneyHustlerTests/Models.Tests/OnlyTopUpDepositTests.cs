using NUnit.Framework;
using MoneyHustler.Models;
using System;

namespace MoneyHustlerTests.Models.Tests
{
    class OnlyTopUpDepositTests
    {
        [TestCase("Namename", 2, "2020-06-29", "2021-06-29")] //был баланс 100
        public void MainDepositTest(string name, decimal percent, DateTime openDate, DateTime dayOfCloseDeposit)
        {
            //arrange
            OnlyTopDeposit deposit1 = new OnlyTopDeposit(name, percent, openDate, dayOfCloseDeposit);
            OnlyTopDeposit deposit2 = new OnlyTopDeposit();
            deposit2.Name = name;
            deposit2.Percent = percent;
            deposit2.OpenDate = openDate;
            deposit2.PaymentDay = deposit1.PaymentDay;
            deposit2.MoneyBox = deposit1.MoneyBox;
            deposit2.DayOfCloseDeposit = deposit1.DayOfCloseDeposit;

            OnlyTopDeposit deposit3 = new OnlyTopDeposit();
            deposit3.Name = deposit2.Name;
            deposit3.Percent = deposit2.Percent;
            deposit3.OpenDate = deposit2.OpenDate;

            //act, assert          
            Assert.AreEqual(deposit2.Name, deposit1.Name);
            Assert.AreEqual(deposit1.Name, deposit3.Name);
        }
    }
}
