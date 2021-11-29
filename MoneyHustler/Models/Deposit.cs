using System;

namespace MoneyHustler.Models
{

    public class Deposit : MoneyVault //наследуется от мани волт
    {
        //данный вклад является копилкой и не имеет даты закрытия
        public decimal Percent { get; set; }
        public int PaymentDay { get; set; }

        public DateTime OpenDate { get; set; } //дата открытия вклада

        public Deposit()
        {

        }

        public Deposit(string name, decimal percent, DateTime openDate)
        {
            Name = name;
            Percent = percent;
            OpenDate = openDate;
            PaymentDay = openDate.Day; //день зачисления процентов
        }
    }
}
