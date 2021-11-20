using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    public class Credit
    {
        public decimal Value { get; set; }
        public string Name { get; set; }
        public double Percent { get; set; }
        public DateTime DayClose { get; set; }

        public DateTime DayOpen { get; set; }

        public Card BindedCard { get; set; }

        public Credit(string name, double percent, decimal value, Card card, DateTime dayClose, DateTime dayOpen)
        {
            if (value < 0)
            {
                Value = Math.Abs(value);
            }
            Name = name;
            Percent = percent;
            Value = value;
            BindedCard = card;
            DayClose = dayClose;
            DayOpen = dayOpen;
        }
        private void IncreaseValue(decimal valueIncrease)
        {
            Value += valueIncrease;
        }

        public void DecreaseValue(Expense expense)
        {
            if (Value < expense.Amount)
            {
                throw new ArgumentException("You can't decrease your credit with amount more than current balance.");
            }
            Value -= expense.Amount;
        }

        public decimal GetMonthlyPayment(int percentPeriod)
        {
            double monthPercent = Percent / (100 * 12);
            var persentRate = monthPercent / (1 - Math.Pow((1 + monthPercent), 0 - percentPeriod - 1));
            decimal payment = Value * (decimal)persentRate;
            return payment;
        }

        public int GetMounthPeriod()
        {
            int percentPeriod = (DayClose.Month - DayOpen.Month) + 12 * (DayClose.Year - DayOpen.Year);
            return percentPeriod;
        }

        public void PayMonthlyPayment(Expense expense)
        {   
            BindedCard.DecreaseBalance(expense);
            DecreaseValue(expense);
        }




    }
}
