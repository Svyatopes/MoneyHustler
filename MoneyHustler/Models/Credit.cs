using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    public class Credit
    {
        private Storage _storageInstance = Storage.GetInstance();
        public decimal? Value { get; set; }

        public decimal ValuewithoutPercent { get; set; }

        public string Name { get; set; }
        public double Percent { get; set; }
        public DateTime DayClose { get; set; }

        public DateTime DayOpen { get; set; }

        public Card BindedCard { get; set; }
        public Person Person { get; set; }
        public decimal AnnuentPayValue { get; set; }



        public Credit()
        {

        }
        public Credit(string name, double percent, decimal? value, decimal valueInitial,Person person,Card card, DateTime dayClose, DateTime dayOpen)
        {

            Name = name;
            Percent = percent;
            ValuewithoutPercent = valueInitial;
            BindedCard = card;
            DayClose = dayClose;
            DayOpen = dayOpen;
            Person = person;
            SetMonthlyPayment();
            if (Value == null)
            {
                Value = AnnuentPayValue * GetMounthPeriod();
            }
            else 
            {
                Value = value; 
            }
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

        public void SetMonthlyPayment()
        {
            int percentPeriod = GetMounthPeriod();
            double monthPercent = Percent / (100 * 12);
            double persentRate = monthPercent / (1 - Math.Pow((1 + monthPercent), 0 - percentPeriod));
            decimal payment = ValuewithoutPercent * (decimal)persentRate;
            AnnuentPayValue = payment;
        }

        public int GetMounthPeriod()
        {
            int percentPeriod = (DayClose.Month - DayOpen.Month) + 12 * (DayClose.Year - DayOpen.Year);
            return percentPeriod;
        }

        public void PayMonthlyPayment()
        {
            var expenseType = _storageInstance.ExpenseTypes.FirstOrDefault(item => item.Name == "Кредит");
            if (expenseType == null)
            {
                expenseType = new ExpenseType() { Name = "Кредит" };
                _storageInstance.ExpenseTypes.Add(expenseType);
            }
            Expense expense = new Expense(AnnuentPayValue, DateTime.Today, Person, "Ежемесячная оплата по кредиту", expenseType);
            BindedCard.DecreaseBalance(expense);
            DecreaseValue(expense);

        }

        public void PayOneTimePayment(decimal payValue)
        {
            var expenseType = _storageInstance.ExpenseTypes.FirstOrDefault(item => item.Name == "Кредит");
            if (expenseType == null)
            {
                expenseType = new ExpenseType() { Name = "Кредит" };
            }
            Expense expense = new Expense(payValue, DateTime.Today, Person, "Единовременный платеж по кредиту", expenseType);
            BindedCard.DecreaseBalance(expense);
            DecreaseValue(expense);
            ValuewithoutPercent -= payValue;
            SetMonthlyPayment();

        }



    }
}
