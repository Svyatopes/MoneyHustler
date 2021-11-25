using MoneyHustler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyHustlerTests.Models.Tests
{
    class CardTestData
    {
        public static Card GetForTest(int index)
        {
            return index switch
            {
                4 => new int[,] { { 3, 4, 5, 6 }, { 6, 7, 8, 9 }, { 9, 8, 7, 6 }, { 6, 5, 4, 3 } },
                3 => new int[,] { { 3, 4, 5 }, { 6, 7, 8 }, { 9, 8, 7 } },
                2 => new int[,] { { 0, 5, -3, 8 }, { 0, 0, -23, 77 } },
                1 => new int[,] { { 1, 56, -34 }, { 2, -45, 32 } },
                0 => new int[,] { { 0, 0, 0 }, { 0, 0, 0 } },
                _ => new int[,] { { } },
            };
        }
    }
}
