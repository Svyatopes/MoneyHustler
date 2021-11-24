using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    public class Income : MoneyTraffic //наследует свойства класса AbstractMoneyTraffic
    {
        public override decimal Amount { 
            get => base.Amount;
            set 
            {
                if (Vault != null)
                {
                    Vault.ChangeBalance(value - base.Amount);
                }
                
                base.Amount = value;
            }
        }
        public IncomeType Type { get; set; } // делегирует класс категории

        public Income(decimal amount, DateTime date, Person person, string comment, IncomeType type)
                        : base(amount, date, person, comment)
        {
            Type = type;
        }

    }
}
