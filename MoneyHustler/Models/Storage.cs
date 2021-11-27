using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Newtonsoft.Json;

namespace MoneyHustler.Models
{
    public sealed class Storage
    {

        //Класс используется как хранилище всех данных в программе.
        private static Storage _instance;

        public List<Credit> Credits = new List<Credit>();
        public List<Person> Persons = new List<Person>();
        public List<MoneyVault> Vaults = new List<MoneyVault>();
        public List<IncomeType> IncomeTypes = new List<IncomeType>();
        public List<ExpenseType> ExpenseTypes = new List<ExpenseType>();

        private const string _path = "./storage.json";

        private Storage() { }

        public static Storage GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Storage();
            }
            return _instance;
        }

        public static List<Income> GetAllIncomes()
        {
            List<Income> allIncomes = new List<Income>();

            Storage instance = GetInstance();
            foreach (MoneyVault moneyVault in instance.Vaults)
            {
                allIncomes.AddRange(moneyVault.Incomes);
            }

            return allIncomes;
        }

        public static List<Expense> GetAllExpenses()
        {
            List<Expense> allExpenses = new List<Expense>();

            Storage instance = GetInstance();
            foreach (MoneyVault moneyVault in instance.Vaults)
            {
                allExpenses.AddRange(moneyVault.Expenses);
            }

            return allExpenses;
        }

        public static bool IsIncomeTypeUsedInVaults(IncomeType incomeType)
        {
            return GetAllIncomes().Any(item => item.Type == incomeType);
        }

        public static bool IsExpenseTypeUsedInVaults(ExpenseType expenseType)
        {
            return GetAllExpenses().Any(item => item.Type == expenseType);
        }

        public static void Save()
        {
            var storageInstance = GetInstance();

            var _jsonSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ObjectCreationHandling = ObjectCreationHandling.Auto
            };

            string jsonString = JsonConvert.SerializeObject(storageInstance, Formatting.Indented, _jsonSettings);
            File.WriteAllText(_path, jsonString);
        }

        public static void Load()
        {
            if (!File.Exists(_path))
            {
                return;
            }

            string jsonString = File.ReadAllText(_path);

            var _jsonSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ObjectCreationHandling = ObjectCreationHandling.Auto
            };

            _instance = JsonConvert.DeserializeObject<Storage>(jsonString, _jsonSettings);
        }
    }
}
