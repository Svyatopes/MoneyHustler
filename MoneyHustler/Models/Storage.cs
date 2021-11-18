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

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
            };

            string jsonString = JsonConvert.SerializeObject(storageInstance, settings);
            File.WriteAllText(_path, jsonString);
        }

        public static void Load()
        {
            if (!File.Exists(_path))
            {
                return;
            }

            string jsonString = File.ReadAllText(_path);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
            var storageInstance = (StorageInstance)JsonConvert.DeserializeObject(jsonString, settings);

            Storage.Vaults = storageInstance.Vaults;
            Storage.Persons = storageInstance.Persons;
            Storage.ExpenseTypes = storageInstance.ExpenseTypes;
            Storage.IncomeTypes = storageInstance.IncomeTypes;

            foreach(var vault in Storage.Vaults)
            {
                vault.AfterDeserialize();
            }

            int stop = 1;
        }
    }
}
