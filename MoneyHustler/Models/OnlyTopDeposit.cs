using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    public class OnlyTopDeposit : Deposit
    {
        public decimal CurrentMoneyBox { get; set; } //начисленная сумма с процентов
        public DateTime DayOfCloseDeposit { get; set; } //день закрытия вклада
        private Storage _storageInstance = Storage.GetInstance();//ANTON

        public List<decimal> MoneyBoxes;

        public OnlyTopDeposit()
        {

        }

        public OnlyTopDeposit(string name, decimal percent, DateTime openDate, DateTime dayOfCloseDeposit): base(name, percent, openDate)
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
                _expenses.Add(expense);
            }
            else
            {
                if (CurrentMoneyBox < expense.Amount)
                {
                    throw new ArgumentException("You can't decrease your balance with amount more than current balance.");
                }
                CurrentMoneyBox -= expense.Amount;
                _balance -= expense.Amount;
                expense.Vault = this;
                _expenses.Add(expense);
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
                CurrentMoneyBox = _balance * (Percent / 100)/12;
                _balance += CurrentMoneyBox;
            }
        }

        public new void StartInitializeIncomesOfDeposit()
        {
            if (OpenDate == DateTime.Today)
            {
                return;
            }
            //создание переменной для перечисления с момента открытия вклада:

            DateTime dateCounterFromOpenDate = new(OpenDate.Year, OpenDate.Month, OpenDate.Day);
            //поиск категории по начислению процентов (она будет, если вклад не первый)
            var incomeTypePercentOfDeposit = _storageInstance.IncomeTypes.FirstOrDefault(item => item.Name == "Проценты по вкладу");
            if (incomeTypePercentOfDeposit == null)
            {
                incomeTypePercentOfDeposit = new IncomeType { Name = "Проценты по вкладу" };
                _storageInstance.IncomeTypes.Add(incomeTypePercentOfDeposit);
            }
            //MinBalanceMonth.Add(new(_balance, _incomes[0], dateCounterFromOpenDate));
            //если депозит открыт задним числом, то добавляем к нему баланс
            while (DateTime.Today > dateCounterFromOpenDate.AddMonths(1))
            {
                dateCounterFromOpenDate = dateCounterFromOpenDate.AddMonths(1);
                Income incomeDeposit = new Income
                    (_balance * (Percent / 100) / 12,
                     dateCounterFromOpenDate,
                     null,
                     "Начисление процентов по вкладу",
                     incomeTypePercentOfDeposit);
                _earnIncomes.Add(incomeDeposit); //сохраняем в списке доходов чисто по вкладу
                this.IncreaseBalance(incomeDeposit); //добавляем доход в этот депозит
                incomeDeposit.Vault = this;
                MoneyBoxes.Add(incomeDeposit.Amount);


                //MinBalanceMonth.Add(new BalanceOfMonth(_balance, incomeDeposit, dateCounterFromOpenDate));
            }
        }

    }
}
