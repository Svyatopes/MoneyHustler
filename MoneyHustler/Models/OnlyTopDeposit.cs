using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    public class OnlyTopDeposit : Deposit
    {
        
        public DateTime DayOfCloseDeposit { get; set; } //день закрытия вклада

        public OnlyTopDeposit(string name, decimal balance, decimal percent, DateTime openDate, DateTime dayOfCloseDeposit): base(name, balance, percent, openDate)
        {
            DayOfCloseDeposit = dayOfCloseDeposit;
        }

        public new void DecreaseBalance(Expense expense)
        {
            if (DateTime.Today > DayOfCloseDeposit)
            {
                if (_balance < expense.Amount)
                {
                    throw new ArgumentException("You can't decrease your balance with amount more than current balance.");
                }
                _balance -= expense.Amount;
                expense.Vault = this;
                Expenses.Add(expense);
            }
            else
            {
                if (MoneyBox < expense.Amount)
                {
                    throw new ArgumentException("You can't decrease your balance with amount more than current balance.");
                }
                MoneyBox -= expense.Amount;
                _balance -= expense.Amount;
                expense.Vault = this;
                Expenses.Add(expense);
            }
            //Если дата закрытия не настала, то мы можем сниять только накопленные деньги,
            //а после даты закрытия мы можем сниять весь баланс
        }

        public new void EarnIncome()
        {
            if (DateTime.Today > DayOfCloseDeposit)
            {
                throw new ArgumentException("Deposit is closed"); //Знатоки англиского исправите
            }
            if (DateTime.Today.Day == PaymentDay)
            {
                MoneyBox = _balance * (Percent / 100)/12;
                _balance += MoneyBox;
            }
        }

    }
}
