using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustler
{
    abstract class AbstractMoneyTraffic //абстрактный класс с информацией о финансовом обороте
    {
        public decimal Amount { get; set; } //сумма
        public DateTime Date { get; set; } //дата
        public string Comment { get; set; } //комментарий
        public Person Person { get; set; } //кто совершил

        public AbstractMoneyTraffic( 
            decimal amount, 
            DateTime date, 
            Person person, 
            string comment
            )
        {
            Amount = amount;
            Date = date;
            Person = person;
            Comment = comment;

        }
    }
}
