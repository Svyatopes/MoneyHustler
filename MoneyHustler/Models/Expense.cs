using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    public class Expense : MoneyTraffic //наследует свойства класса AbstractMoneyTraffic
    {
        public override decimal Amount
        {
            get => base.Amount;
            set
            {
                if (Vault != null)
                {
                    Vault.ChangeBalance(base.Amount - value);
                }
                
                base.Amount = value;
            }
        }

        public ExpenseType Type { get; set; } // делегирует класс категории расхода

        public Expense(decimal amount, DateTime date, Person person, string comment, ExpenseType type)
                        : base(amount, date, person, comment)
        {
            Type = type;
        }

    }
}
