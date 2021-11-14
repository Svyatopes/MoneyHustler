using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    public abstract class Deposit : MoneyVault //наследуется от мани волт
    {
        public decimal Percent { get; set; }
        public int PaymentDay { get; set; } // день месяца, число не может быть отрицательным и больше 31, продумать логику set(чтобы не нарушать дату начисления процента) 

        public Deposit(decimal percent, int paymentDay) //string name) : base(name) нужно сделать конструктор для мани волт,
                                                        //чтобы в конструкторе депозита использовать параметры, такие как имя и баланс 
        {
            Percent = percent;
            PaymentDay = paymentDay;
        }

        //public void DecreaseBalance(decimal value) нужно подумать, зачем переопределять этот метод
        
    }
}
