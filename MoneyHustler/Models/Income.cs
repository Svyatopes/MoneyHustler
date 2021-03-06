using System;

namespace MoneyHustler.Models
{
    public class Income : MoneyTraffic //наследует свойства класса AbstractMoneyTraffic
    {
        public IncomeType Type { get; set; } // делегирует класс категории

        public Income()
        {

        }

        public Income(decimal amount, DateTime date, Person person, string comment, IncomeType type)
                        : base(amount, date, person, comment)
        {
            Type = type;
        }



    }
}
