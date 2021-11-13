using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler
{
    class Expense : AbstractMoneyTraffic //наследует свойства класса AbstractMoneyTraffic
    {
        public ExpenseType Type { get; set; } // делегирует класс категории расхода

        public Expense(
            decimal amount,
            DateTime date,
            Person person,
            string comment,
            ExpenseType type
            )
        : base(amount, date, person, comment)
        {
            Type = type;
        }

    }
}
