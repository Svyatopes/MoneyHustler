using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoneyHustler.Models;
using NUnit.Framework;
using System;

namespace MoneyHustlerTests
{
    [TestClass]
    public class CrediyTests
    {
        [SetUp]
        public void Setup()
        {
        }

        public Credit GetTestCredit(int key)
        {
            Credit credit = key switch
            {
                1 => new Credit("Микрокредитыч", 10, null, 10000, new Person() { Name = "Антон" }, new Card("Кошель", 1000000, 5), new DateTime(2021, 11, 21), new DateTime(2023, 11, 21)),
                _ => new Credit()
            };
            return credit;
        }


        [Test]

    }
}
