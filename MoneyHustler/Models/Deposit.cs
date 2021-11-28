using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{

    public class Deposit : MoneyVault //наследуется от мани волт
    {
        //данный вклад является копилкой и не имеет даты закрытия
        public decimal Percent { get; set; }
        public int PaymentDay { get; set; }
        public DateTime OpenDate { get; set; } //дата открытия вклада

        private Storage _storageInstance = Storage.GetInstance();//ANTON
        public List<BalanceOfMonth> MinBalanceMonth = new List<BalanceOfMonth>(); //ANTON: лист минимальных остатоков за месяц
        protected List<Income> _earnIncomes = new List<Income>(); //доходы чисто по вкладу
        public Deposit()
        {

        }

        public Deposit(string name, decimal percent, DateTime openDate)
        {
            Name = name;
            Percent = percent;
            OpenDate = openDate;
            PaymentDay = openDate.Day; 

            StartInitializeIncomesOfDeposit(); //ANTON: при создании вклада вызывается метод...
        }

        public void EarnIncome() 
        {
            if (DateTime.Today.Day == PaymentDay)
            {
                _balance += _balance * (Percent / 100) / 12;
            }
        }

        public void EarnIncomeAndOpenMonthlyPeriod()
        {
            DateTime lastDayPeriodMinBalance = MinBalanceMonth.Max(item => item._closePeriod);
            BalanceOfMonth lastBalanceOfMonth = MinBalanceMonth.FirstOrDefault(item => item._closePeriod == lastDayPeriodMinBalance);

            if (DateTime.Now >= lastDayPeriodMinBalance)
            {
                var incomeTypePercentOfDeposit = _storageInstance.IncomeTypes.FirstOrDefault(item => item.Name == "Проценты по вкладу");
                if (incomeTypePercentOfDeposit == null)
                {
                    incomeTypePercentOfDeposit = new IncomeType { Name = "Проценты по вкладу" };
                    _storageInstance.IncomeTypes.Add(incomeTypePercentOfDeposit);
                }

                Income incomeDeposit = new Income
                    (_balance * (Percent / 100) / 12,
                     lastDayPeriodMinBalance,
                     null,
                     "Начисление процентов по вкладу",
                     incomeTypePercentOfDeposit);
                _earnIncomes.Add(incomeDeposit); //сохраняем в списке доходов чисто по вкладу
                this.IncreaseBalance(incomeDeposit); //добавляем доход в этот депозит
                incomeDeposit.Vault = this;
                _earnIncomes.Add(incomeDeposit);
                MinBalanceMonth.Add(new BalanceOfMonth(_balance, incomeDeposit, lastDayPeriodMinBalance.AddDays(1)));
            }
        }

        public new void DecreaseBalance(Expense expense)
        {
            _balance -= expense.Amount;
            expense.Vault = this;
            _expenses.Add(expense);
            RecalculationEarnIncomes(expense);
        }


        public void StartInitializeIncomesOfDeposit()
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
            MinBalanceMonth.Add(new(_balance, _incomes[0], dateCounterFromOpenDate));
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
                

                MinBalanceMonth.Add(new BalanceOfMonth(_balance, incomeDeposit, dateCounterFromOpenDate));
            }
        }
        

        public void RecalculationEarnIncomes(MoneyTraffic financialTurnover)
        {
            //поиск дохода процентов по вкладу
            var incomeOfDeposit = _earnIncomes.FirstOrDefault(item
                => item.Date.Month == financialTurnover.Date.Month && item.Date.Year == financialTurnover.Date.Year);

            //поиск минимального баланса того месяца по доходу от процентов
            var minBalanceMonth = MinBalanceMonth.FirstOrDefault(item => item.EarnIncome == incomeOfDeposit);

            //установка минимального баланса в зависимости от фин оборота
            minBalanceMonth.SetMinBalanceOnPeriod(financialTurnover);
            
            //пересчёт суммы дохода
            incomeOfDeposit.Amount = minBalanceMonth.MinBalance * (Percent / 100) / 12;

            //поиск следующих минимальных балансов
            var allMinBalances = MinBalanceMonth.Where(item => item._startPeriod > minBalanceMonth._closePeriod);
            
            //запомним текущий баланс месяца, в котором был финансовый оборот
            decimal curBal = minBalanceMonth.CurrentBalance;

            //побежали по минимальным балансам в следующие месяцы
            foreach (var item in allMinBalances)
            {
                item.MinBalance = curBal 
                + curBal * (Percent / 100) / 12; //первому из следующих присваиваем минимальный с процентами

                item.CurrentBalance = item.MinBalance;//текущему присваиваем тоже самое
                item.EarnIncome.Amount = curBal * (Percent / 100) / 12; //изменяем сумму его дохода

                curBal = item.CurrentBalance; // кидаем новое значение счётчику
            }
            _balance = curBal;
        }


    }
}
