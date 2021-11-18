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
            _incomes =  new List<Income>();
            Incomes = _incomes.AsReadOnly();
            _expenses  = new List<Expense>();
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
            if (_balance < expense.Amount)
            {
                throw new ArgumentException("You can't decrease your balance with amount more than current balance.");
            }
            _balance -= expense.Amount;
            expense.Vault = this;
            _expenses.Add(expense);
        }

        //TODO: change default(Person) to some instance of Person
        //TODO: maybe need change expenseType and incomeType to some user specific instance
        public void TransferMoney(MoneyVault vault, decimal amount, DateTime date, string comment, ExpenseType expenseType, IncomeType incomeType)
        {
            if (amount > _balance)
            {
                throw new ArgumentException("You can't transfer more money than you already have on this vault.");
            }

            DecreaseBalance(new Expense(amount, date, default(Person), comment, expenseType));
            vault.IncreaseBalance(new Income(amount, date, default(Person), comment, incomeType));
        }

        public void Remove(Income income)
        {
            if(!_incomes.Contains(income) || income.Vault != this)
            {
                throw new ArgumentException("Not contains in this list. Check your program logic");
            }

            if(income.Amount > _balance)
            {
                throw new ArgumentException("Your income is bigger than your current balance, you cannot remove this income.");
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
    }
}
