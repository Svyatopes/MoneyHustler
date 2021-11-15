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
        //TODO: необходимо продумать движение по закрытию вклада
        public decimal Percent { get; set; }
        public int PaymentDay { get; set; } // день месяца,
        // TODO: число не может быть отрицательным и больше 31, продумать логику set(чтобы не нарушать дату начисления процента) 

        //Никита 14.11
        

        public DateTime OpenDate { get; set; } //дата открытия вклада

        public Deposit(string name, decimal balance, decimal percent, DateTime openDate) //string name) : base(name) нужно сделать конструктор для мани волт,
                                                                                         //чтобы в конструкторе депозита использовать параметры, такие как имя и баланс 
        {
            Name = name;
            _balance = balance;
            Percent = percent;
            OpenDate = openDate;
            PaymentDay = openDate.Day; //день зачисления процентов
        }


        public void EarnIncome() //начисление процентов
        {
            if (DateTime.Today.Day == PaymentDay) 
                //если день даты совпадёт с днём выплаты процентов, то начисляем деньги
            {
                _balance += _balance * (Percent / 100) / 12;
            }
        }


        //TODO: public void DecreaseBalance(decimal value) нужно подумать, зачем переопределять этот метод

    }
}
