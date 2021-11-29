using System;

namespace MoneyHustler.Models
{
    public class Expense : MoneyTraffic //наследует свойства класса AbstractMoneyTraffic
    {
        public ExpenseType Type { get; set; } // делегирует класс категории расхода

        public Expense()
        {

        }

        public Expense(decimal amount, DateTime date, Person person, string comment, ExpenseType type)
                        : base(amount, date, person, comment)
        {
            Type = type;
        }


    }
}
