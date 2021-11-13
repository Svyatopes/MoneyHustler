using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    public abstract class MoneyVault
    {
        public string Name { get; set; }
        public List<Income> Incomes { get; set; }
        public List<Expense> Expenses { get; set; }


        protected decimal _balance;

        public decimal GetBalance()
        {
            return _balance;
        }

        public void IncreaseBalance(Income income)
        {
            _balance += income.Amount;
            income.Vault = this;
            Incomes.Add(income);
        }

        public void DecreaseBalance(Expense expense)
        {
            if (_balance < expense.Amount)
            {
                throw new ArgumentException("You can't decrease your balance with amount more than current balance.");
            }
            _balance -= expense.Amount;
            expense.Vault = this;
            Expenses.Add(expense);
        }

        //TODO: change default(Person) to some instance of Person
        //TODO: maybe need change expenseType and incomeType to some user specific instance
        public void TransferMoney(MoneyVault vault, decimal amount, DateTime date, string comment, ExpenseType expenseType, IncomeType incomeType)
        {
            if (amount > _balance)
            {
                throw new ArgumentException("You can't transfer more money than you already have on this vault.");
            }

            DecreaseBalance(new Expense(amount, date, default(Person), comment, vault, expenseType));
            vault.IncreaseBalance(new Income(amount, date, default(Person), comment, this, incomeType));
        }
    }
}
