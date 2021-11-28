using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    public abstract class MoneyVault
    {
        public string Name { get; set; }

        [JsonIgnore]
        public ReadOnlyCollection<Income> Incomes { get; set; }

        [JsonIgnore]
        public ReadOnlyCollection<Expense> Expenses { get; set; }

        [JsonProperty]
        protected List<Income> _incomes;
        [JsonProperty]
        protected List<Expense> _expenses;
        [JsonProperty]
        protected decimal _balance;

        public MoneyVault()
        {
            _incomes = new List<Income>();
            Incomes = _incomes.AsReadOnly();
            _expenses = new List<Expense>();
            Expenses = _expenses.AsReadOnly();
        }

        public decimal GetBalance()
        {
            return _balance;
        }

        public void IncreaseBalance(Income income)
        {
            _balance += income.Amount;
            income.Vault = this;
            _incomes.Add(income);
        }

        public void DecreaseBalance(Expense expense)
        {
            _balance -= expense.Amount;
            expense.Vault = this;
            _expenses.Add(expense);
        }

        public void Remove(Income income)
        {
            if (!_incomes.Contains(income) || income.Vault != this)
            {
                throw new ArgumentException("Not contains in this list. Check your program logic");
            }

            _incomes.Remove(income);
            _balance -= income.Amount;

        }

        public void Remove(Expense expense)
        {
            if (!_expenses.Contains(expense) || expense.Vault != this)
            {
                throw new ArgumentException("Not contains in this list. Check your program logic");
            }

            _expenses.Remove(expense);
            _balance += expense.Amount;
        }

        public decimal GetBalanceOnDate(DateTime day)//ready
        {
            decimal balanceOnSelectedDay;

            balanceOnSelectedDay = this._balance;
            foreach (Expense item in this.Expenses)
            {
                if (item.Date >= day)
                {
                    balanceOnSelectedDay += item.Amount;
                }
            }
            foreach (Income item in this.Incomes)
            {
                if (item.Date >= day)
                {
                    balanceOnSelectedDay -= item.Amount;
                }
            }


            return balanceOnSelectedDay;
        }

        public bool IsHaveIncomesOrExpenses()
        {
            return Incomes.Any() || Expenses.Any();
        }

        public void ChangedAmountInIncomeOrExpense(MoneyTraffic moneyTraffic, decimal newAmount)
        {
            switch (moneyTraffic)
            {
                case Income:
                    _balance += newAmount - moneyTraffic.Amount;
                    break;
                case Expense:
                    _balance += moneyTraffic.Amount - newAmount;
                    break;
            }
        }

        public decimal ReturnBalanceWithDifferenceAmount(decimal deltaAmount)
        {
            return _balance - deltaAmount;
        }

    }
}
