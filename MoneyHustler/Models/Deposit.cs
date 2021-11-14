using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler.Models
{
    //Никита
    //Убрал абстрактный класс у депозита, чтобы его можно было использовать как WithdrawDeposit
    public class Deposit : MoneyVault //наследуется от мани волт
    {
        public decimal Percent { get; set; }
        public int PaymentDay { get; set; } // день месяца, число не может быть отрицательным и больше 31, продумать логику set(чтобы не нарушать дату начисления процента) 

        //Никита 14.11
        public decimal MoneyBox { get; set; }

        public DateTime OpenDate { get; set; }

        public Deposit(string name, decimal balance, decimal percent, DateTime openDate) //string name) : base(name) нужно сделать конструктор для мани волт,
                                                                                         //чтобы в конструкторе депозита использовать параметры, такие как имя и баланс 
        {
            Name = name;
            _balance = balance;
            Percent = percent;
            OpenDate = openDate;
            PaymentDay = openDate.Day;
            MoneyBox = 0;
        }


        public void EarnIncome()
        {
            if (DateTime.Today.Day == PaymentDay)
            {
                MoneyBox = _balance * (Percent / 100) / 12;
                _balance += MoneyBox;
            }
        }


        //public void DecreaseBalance(decimal value) нужно подумать, зачем переопределять этот метод

    }
}
