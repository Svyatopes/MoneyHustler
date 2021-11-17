using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    class Card: MoneyVault
    {
        public decimal CashBack { get; set; } //Вводиться в ввиде процента

        public Card(string name, decimal balance, decimal cashback)
        {
            Name = name;
            _balance = balance;
            CashBack = cashback;
        }

        public new void DecreaseBalance(Expense expense)
        {
            if (_balance < expense.Amount)
            {
                throw new ArgumentException("You can't decrease your balance with amount more than current balance.");
            }
            _balance -= expense.Amount;
            expense.Vault = this;
            _expenses.Add(expense);
            EarnCashBack(expense);
        }

        private void EarnCashBack(Expense expense)
        {
            //Создание нового Income для начисления кэшбэка с категорией дохода "CashBack"
            IncomeType incomeTypeCashBack = new IncomeType { Name = "CashBack" };

            //Информация по категории "CashBack", зависит от расхода по this Card 
            Income incomeCashback = new Income(expense.Amount * CashBack / 100, 
                expense.Date, expense.Person, expense.Comment, 
                incomeTypeCashBack); //Создаем новый доход который является кэшбеком изходя из expense
            IncreaseBalance(incomeCashback);
        }

    }
}
