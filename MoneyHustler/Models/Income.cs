using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    public class Income : MoneyTraffic //наследует свойства класса AbstractMoneyTraffic
    {
        public IncomeType Type { get; set; } // делегирует класс категории

        public Income(decimal amount, DateTime date, Person person, string comment, MoneyVault vault, IncomeType type)
                        : base(amount, date, person, comment, vault)
        {
            Type = type;
        }

    }
}
