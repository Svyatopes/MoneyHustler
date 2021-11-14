using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    static class Storage
    {

        //Класс используется как хранилище всех данных в программе.

        public static List<Person> Persons;
        public static List<MoneyVault> Vaults;
        public static List<IncomeType> IncomeTypes;
        public static List<ExpenseType> ExpenseTypes;

        public static List<Income> GetAllIncomes()
        {
            List<Income> allIncomes = new List<Income>();

            foreach (MoneyVault moneyVault in Vaults)
            {
                if (!allIncomes.Any())
                {
                    allIncomes = moneyVault.Incomes;
                }
                else
                {
                    allIncomes.AddRange(moneyVault.Incomes);
                }
            }

            return allIncomes;
        } 

        public static List<Expense> GetAllExpences()
        {
            List<Expense> allExpences = new List<Expense>();

            foreach (MoneyVault moneyVault in Vaults)
            {
                if (!allExpences.Any())
                {
                    allExpences = moneyVault.Expenses;
                }
                else
                {
                    allExpences.AddRange(moneyVault.Expenses);
                }
            }

            return allExpences;
        }
    }
}
