using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurs_Fomicheva
{
    internal class Move
    {
        private int from;
        private int to;

        //  Инициализирует класс
        public Move(int moveFrom, int moveTo)
        {
            from = moveFrom;
            to = moveTo;
        }

        //  от собственност
        public int getFrom()
        {
            return from;
        }
        // к собственности
        //  Целое число, задающее номер фигуры, используемого в игровом ходе.
        public int getTo()
        {
            return to;
        }
    }
}
