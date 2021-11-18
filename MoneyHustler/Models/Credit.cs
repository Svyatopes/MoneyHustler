using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    public class Credit
    {
        private decimal _value;
        public string Name { get; set; }
        public decimal Percent { get; set; }
        public Card BindedCard { get; set; }

        public Credit(string name, decimal percent, decimal Value, Card card)
        {
            if (Value < 0)
            {
                Value = Math.Abs(Value);
            }
            Name = name;
            Percent = percent;
            _value = Value;
            BindedCard = card;
        }

        public decimal GetValue()
        {
            return _value;
        }
        private void IncreaseValue(decimal valueIncrease)
        {
            _value += valueIncrease;
        }

        public void DecreaseValue(Expense expense)
        {
            if (_value < expense.Amount)
            {
                throw new ArgumentException("You can't decrease your credit with amount more than current balance.");
            }
            _value -= expense.Amount;
        }


    }
}
