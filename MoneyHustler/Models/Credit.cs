using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    public class Credit
    {
       
        public decimal? Amount { get; set; }

        public decimal InitialAmount { get; set; }

        public string Name { get; set; }
        public decimal Percent { get; set; }
        public DateTime CloseDate { get; set; }

        public DateTime OpenDate { get; set; }

        public Card BindedCard { get; set; }
        public Person Person { get; set; }
        public decimal MonthlyPayment { get; set; }



        public Credit()
        {

        }
        public Credit(string name, decimal percent, decimal? amount, decimal initialAmount, Person person, Card card, DateTime dayClose, DateTime dayOpen)
        {
            Name = name;
            Percent = percent;
            InitialAmount = initialAmount;
            BindedCard = card;
            CloseDate = dayClose;
            OpenDate = dayOpen;
            Person = person;
            SetMonthlyPayment();
            if (Amount == null)
            {
                Amount = MonthlyPayment * GetMounthPeriod();
            }
            else
            {
                Amount = amount;
            }
        }

        private void DecreaseValue(Expense expense)
        {
            if ((Amount + (decimal)0.01) < expense.Amount)
            {
                throw new ArgumentException("You can't decrease your credit with amount more than current balance.");
            }
            Amount -= expense.Amount;
        }

        private void SetMonthlyPayment()
        {
            int percentPeriod = GetMounthPeriod();
            decimal monthPercent = Percent / (100 * 12);
            decimal persentRate = (decimal)((double)monthPercent / (1 - Math.Pow((1 + (double)monthPercent), 0 - percentPeriod)));
            decimal payment = InitialAmount * (decimal)persentRate;
            MonthlyPayment = payment;
        }

        private int GetMounthPeriod()
        {
            int percentPeriod = (CloseDate.Month - OpenDate.Month) + 12 * (CloseDate.Year - OpenDate.Year);
            return percentPeriod;
        }

        public void PayMonthlyPayment(ExpenseType expenseType)
        {
            Expense expense = new Expense(MonthlyPayment, DateTime.Today, Person, "Ежемесячная оплата по кредиту", expenseType);
            BindedCard.DecreaseBalance(expense);
            DecreaseValue(expense);

        }

        public void PayOneTimePayment(decimal payValue, ExpenseType expenseType)
        {
            Expense expense = new Expense(payValue, DateTime.Today, Person, "Единовременный платеж по кредиту", expenseType);

            BindedCard.DecreaseBalance(expense);
            decimal tmp = InitialAmount;

            InitialAmount = (decimal)Amount;
            InitialAmount -= payValue;

            SetMonthlyPayment();
            Amount = MonthlyPayment * GetMounthPeriod();
            InitialAmount = tmp;
        }



    }
}
