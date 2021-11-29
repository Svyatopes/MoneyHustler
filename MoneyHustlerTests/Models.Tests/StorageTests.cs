using NUnit.Framework;
using MoneyHustler.Models;
using System.Collections.Generic;

namespace MoneyHustlerTests.Models.Tests
{
    class StorageTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetAllIncomesTest()
        {
            //arrange
            Income firstIncome = new Income();
            Income secIncome = new Income();
            List<Income> list = new List<Income>();

            Card cardForIncomes = new Card();
            Storage instance = Storage.GetInstance();
            instance.Vaults.Clear();
            instance.Vaults.Add(cardForIncomes);

            //act 
            list.Add(firstIncome);
            list.Add(secIncome);

            cardForIncomes.IncreaseBalance(firstIncome);
            cardForIncomes.IncreaseBalance(secIncome);

            //assert
            Assert.AreEqual(list, Storage.GetAllIncomes());
        }

        [Test]
        public void GetAllExpensesTest()
        {
            //arrange
            Expense firstExpense = new Expense();
            Expense secExpense = new Expense();
            List<Expense> list = new List<Expense>();

            Card cardForExpenses = new Card();
            Storage instance = Storage.GetInstance();
            instance.Vaults.Add(cardForExpenses);

            //act 
            list.Add(firstExpense);
            list.Add(secExpense);

            cardForExpenses.DecreaseBalance(firstExpense);
            cardForExpenses.DecreaseBalance(secExpense);

            //assert
            Assert.AreEqual(list, Storage.GetAllExpenses());
        }


        [TestCase("MyIncomeType", true)]
        public void IsIncomeTypeUsedInVaultsTest(string typeName, bool expected)
        {
            //arrange
            Income income = new Income();
            IncomeType type = new IncomeType();
            type.Name = typeName;
            income.Type = type;

            Card cardForIncomeTypes = new Card();
            Storage instance = Storage.GetInstance();
            instance.Vaults.Add(cardForIncomeTypes);

            //act 
            cardForIncomeTypes.IncreaseBalance(income);

            //assert
            Assert.AreEqual(expected, Storage.IsIncomeTypeUsedInVaults(type));
        }

        [TestCase("MyExpenseType", true)]
        public void IsExpenseTypeUsedInVaultsTest(string typeName, bool expected)
        {
            //arrange
            Expense expense = new Expense();
            ExpenseType type = new ExpenseType();
            type.Name = typeName;
            expense.Type = type;

            Card cardForExpenseTypes = new Card();
            Storage instance = Storage.GetInstance();
            instance.Vaults.Add(cardForExpenseTypes);

            //act 
            cardForExpenseTypes.DecreaseBalance(expense);

            //assert
            Assert.AreEqual(expected, Storage.IsExpenseTypeUsedInVaults(type));
        }

        [TestCase("MyPersonName", true)]
        public void IsPersonUsedInVaultsExpensesTest(string name, bool expected)
        {
            //arrange
            Person person = new Person();
            person.Name = name;

            Card cardForExpenseTypes = new Card();
            Expense expense = new Expense();
            expense.Person = person;

            Storage instance = Storage.GetInstance();
            instance.Vaults.Clear();
            instance.Vaults.Add(cardForExpenseTypes);

            //act 
            cardForExpenseTypes.DecreaseBalance(expense);

            //assert
            Assert.AreEqual(expected, Storage.IsPersonUsedInVaults(person));
        }

        [TestCase("MyPersonName", true)]
        public void IsPersonUsedInVaultsIncomesTest(string name, bool expected)
        {
            //arrange
            Person person = new Person();
            person.Name = name;

            Card cardForIncomeTypes = new Card();
            Income income = new Income();
            income.Person = person;

            Storage instance = Storage.GetInstance();
            instance.Vaults.Clear();
            instance.Vaults.Add(cardForIncomeTypes);

            //act 
            cardForIncomeTypes.IncreaseBalance(income);

            //assert
            Assert.AreEqual(expected, Storage.IsPersonUsedInVaults(person));
        }

        [TestCase("MyName", true)]
        [TestCase("MyNameAnother", false)]
        public void GetOrCreatePersonByNameTest(string name, bool toAdd)
        {
            //arrange
            Person person = new Person();
            person.Name = name;

            Storage instance = Storage.GetInstance();
            instance.Persons.Clear();
            if (toAdd) instance.Persons.Add(person);

            //act, assert
            if (!toAdd) Assert.IsNotNull(Storage.GetOrCreatePersonByName(name));
            else Assert.AreEqual(person, Storage.GetOrCreatePersonByName(name));
        }


        [TestCase("MyName", true)]
        [TestCase("MyNameAnother", false)]
        public void GetOrCreateExpenseTypeByNameTest(string name, bool toAdd)
        {
            //arrange
            ExpenseType expenseType = new ExpenseType();
            expenseType.Name = name;

            Storage instance = Storage.GetInstance();
            if (toAdd) instance.ExpenseTypes.Add(expenseType);

            //act, assert
            if (!toAdd) Assert.IsNotNull(Storage.GetOrCreateExpenseTypeByName(name));
            else Assert.AreEqual(expenseType, Storage.GetOrCreateExpenseTypeByName(name));
        }

        [TestCase("MyName", true)]
        [TestCase("MyNameAnother", false)]
        public void GetOrCreateIncomeTypeByNameTest(string name, bool toAdd)
        {
            //arrange
            IncomeType incomeType = new IncomeType();
            incomeType.Name = name;
            Storage.Load();

            Storage instance = Storage.GetInstance();
            if (toAdd) instance.IncomeTypes.Add(incomeType);

            //act, assert
            if (!toAdd) Assert.IsNotNull(Storage.GetOrCreateIncomeTypeByName(name));
            else Assert.AreEqual(incomeType, Storage.GetOrCreateIncomeTypeByName(name));
        }

        
        [TestCase("MyName", true)]
        [TestCase("MyNameAnother", false)]
        public void CheckIfPersonExistTest(string name, bool expected)
        {
            //arrange
            Person person = new Person();
            person.Name = name;

            Storage instance = Storage.GetInstance();
            instance.Persons.Clear();
            if (expected) instance.Persons.Add(person);

            //act, assert
            Assert.AreEqual(expected, Storage.CheckIfPersonExist(name));
        }
    }
}
