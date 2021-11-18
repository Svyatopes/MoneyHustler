using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Newtonsoft.Json;

namespace MoneyHustler.Models
{
    public static class Storage
    {

        //Класс используется как хранилище всех данных в программе.

        public static List<Person> Persons = new List<Person>();
        public static List<MoneyVault> Vaults = new List<MoneyVault>();
        public static List<IncomeType> IncomeTypes = new List<IncomeType>();
        public static List<ExpenseType> ExpenseTypes = new List<ExpenseType>();
        public static List<Credit> Credits;

        private const string _path = "./storage.json";

        public static List<Income> GetAllIncomes()
        {
            List<Income> allIncomes = new List<Income>();

            foreach (MoneyVault moneyVault in Vaults)
            {
                allIncomes.AddRange(moneyVault.Incomes);
            }

            return allIncomes;
        }

        public static List<Expense> GetAllExpences()
        {
            List<Expense> allExpences = new List<Expense>();

            foreach (MoneyVault moneyVault in Vaults)
            {
                allExpences.AddRange(moneyVault.Expenses);
            }

            return allExpences;
        }

        public static void Save()
        {
            var storageInstance = new StorageInstance(true);

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

            var storageInstance = JsonConvert.DeserializeObject<StorageInstance>(jsonString, _jsonSettings);

            Storage.Vaults = storageInstance.Vaults;
            Storage.Persons = storageInstance.Persons;
            Storage.ExpenseTypes = storageInstance.ExpenseTypes;
            Storage.IncomeTypes = storageInstance.IncomeTypes;
        }
    }
}
