using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    public class StorageInstance
    {
        public List<Person> Persons { get; set; }
        public List<MoneyVault> Vaults { get; set; }
        public List<IncomeType> IncomeTypes { get; set; }
        public List<ExpenseType> ExpenseTypes { get; set; }


        public StorageInstance()
        {

        }

        public StorageInstance(bool fromStorage)
        {
            if (fromStorage)
            {
                this.Persons = Storage.Persons;
                this.Vaults = Storage.Vaults;
                this.IncomeTypes = Storage.IncomeTypes;
                this.ExpenseTypes = Storage.ExpenseTypes;
            }
            else
            {
                Persons = new List<Person>();
                Vaults = new List<MoneyVault>();
                IncomeTypes = new List<IncomeType>();
                ExpenseTypes = new List<ExpenseType>();
            }

        }

    }
}
