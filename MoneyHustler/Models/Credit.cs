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
        private decimal _repaid;
        public decimal? Amount { get; set; }

        public decimal InitialAmount { get; set; }

        public string Name { get; set; }
        public double Percent { get; set; }
        public DateTime CloseDate { get; set; }

        public DateTime OpenDate { get; set; }

        public Card BindedCard { get; set; }
        public Person Person { get; set; }
        public decimal MonthlyPayment { get; set; }



        public Credit()
        {

        }
        public Credit(string name, double percent, decimal? amount, decimal initialAmount, Person person, Card card, DateTime dayClose, DateTime dayOpen)
        {
           
            _repaid = 0;
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
            if (Amount < expense.Amount)
            {
                throw new ArgumentException("You can't decrease your credit with amount more than current balance.");
            }
            Amount -= expense.Amount;
        }

        private void SetMonthlyPayment()
        {
            int percentPeriod = GetMounthPeriod();
            double monthPercent = Percent / (100 * 12);
            double persentRate = monthPercent / (1 - Math.Pow((1 + monthPercent), 0 - percentPeriod));
            decimal payment = InitialAmount * (decimal)persentRate;
            MonthlyPayment = payment;
        }

        private int GetMounthPeriod()
        {
            int percentPeriod = (CloseDate.Month - OpenDate.Month) + 12 * (CloseDate.Year - OpenDate.Year);
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
            Expense expense = new Expense(MonthlyPayment, DateTime.Today, Person, "Ежемесячная оплата по кредиту", expenseType);
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
            _repaid += payValue;
            InitialAmount -= _repaid;
            SetMonthlyPayment();
            InitialAmount += _repaid;
        }



    }
}
