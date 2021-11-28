using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    public class BalanceOfMonth
    {
        public decimal MinBalance { get; set; }

        public Income EarnIncome { get; set; }

        public decimal CurrentBalance { get; set; }

        public DateTime _startPeriod;

        public DateTime _closePeriod;

        public BalanceOfMonth(decimal minBalance, Income earnIncome, DateTime startPeriodDate)
        {
            MinBalance = minBalance;
            EarnIncome = earnIncome;
            _startPeriod = startPeriodDate .AddDays(1);
            _closePeriod = startPeriodDate.AddMonths(1);
            CurrentBalance = minBalance;
        }

        public void SetMinBalanceOnPeriod(MoneyTraffic financialTurnover)
        {
            switch (financialTurnover)
            {
                case Expense:
                    CurrentBalance -= financialTurnover.Amount;
                    break;
                case Income:
                    CurrentBalance += financialTurnover.Amount;
                    break;
                default:
                    break;
            };
            if (CurrentBalance < MinBalance)
            {
                MinBalance = CurrentBalance;
            }

        }
    }
}
