using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Kurs_Fomicheva
{
    internal class CheckersBoard
    {
        private byte[] pieces;

        // Фигуры, используемые в игре
        public const byte EMPTY = 0;
        public const byte WHITE = 2;
        public const byte WHITE_KING = 3;
        public const byte BLACK = 4;
        public const byte BLACK_KING = 5;

        // Позволяет игре знать, какие фигуры продвигаются королю
        private const byte KING = 1;

        // Фишки для каждой стороны
        private int whitePieces;
        private int blackPieces;


        // Текущий игрок
        private int currentPlayer;

        // Конструктор
        public CheckersBoard()
        {
            pieces = new byte[32];
            clearBoard();
        }

        //  Конструктор сериализации
/*        public CheckersBoard(SerializationInfo info, StreamingContext context)
        {
            byte[] tempPieces = new byte[32];

            pieces = (byte[])info.GetValue("pieces", tempPieces.GetType());

            System.Int32 temp = new System.Int32();
            whitePieces = (int)info.GetValue("whitePieces", temp.GetType());
            blackPieces = (int)info.GetValue("blackPieces", temp.GetType());
            currentPlayer = (int)info.GetValue("currentPlayer", temp.GetType());
        }*/

        //  Возвращает текущего игрока 
        public int getCurrentPlayer()
        {
            return currentPlayer;
        }

        //   Возвращает количество белых фигур
        public int getWhitePieces()
        {
            return whitePieces;
        }

        //   Возвращает количество черных фигур  
        public int getBlackPieces()
        {
            return blackPieces;
        }

        //  Глубокое клонирование экземпляра класса 
        public object clone()
        {
            CheckersBoard board = new CheckersBoard();

            board.currentPlayer = currentPlayer;
            board.whitePieces = whitePieces;
            board.blackPieces = blackPieces;
            for (int i = 0; i < 32; i++)
                board.pieces[i] = pieces[i];

            return board;
        }

        //   Записывает компоненты в поток сериализации
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("pieces", pieces);
            info.AddValue("whitePieces", whitePieces);
            info.AddValue("blackPieces", blackPieces);
            info.AddValue("currentPlayer", currentPlayer);
        }

        //   Находит все допустимые ходы для текущего игрока на текущей доске.  
        public List legalMoves()
        {
            int color;
            int enemy;

            color = currentPlayer;
            if (color == WHITE)
                enemy = BLACK;
            else
                enemy = WHITE;

            if (mustAttack())
                return generateAttackMoves(color, enemy);
            else
                return generateMoves(color, enemy);
        }

        //   Находит все разрешенные приемы атаки для текущего игрока
        private List generateAttackMoves(int color, int enemy)
        {
            List moves = new List();
            List tempMoves;


            for (int k = 0; k < 32; k++)
                if ((pieces[k] & ~KING) == currentPlayer)
                {
                    if ((pieces[k] & KING) == 0)
                        tempMoves = simpleAttack(k, color, enemy);
                    else
                    { // Это королевская фигура
                        List lastPos = new List();

                        lastPos.push_back(k);

                        tempMoves = kingAttack(lastPos, k, NONE, color, enemy);
                    }
                    if (notNull(tempMoves))
                        moves.append(tempMoves);
                }
            return moves;
        }

        //   Находит все разрешенные атакующие ходы для текущего игрока с помощью простых фигур
        private List simpleAttack(int pos, int color, int enemy)
        {
            int x = posToCol(pos);
            int y = posToLine(pos);
            int i;
            List moves = new List();
            List tempMoves;
            int enemyPos, nextPos;

            i = (color == WHITE) ? -1 : 1;
            // диагонали /^  \v
            if (x < 6 && y + i > 0 && y + i < 7)
            {
                enemyPos = colLineToPos(x + 1, y + i);
                nextPos = colLineToPos(x + 2, y + 2 * i);

                if ((pieces[enemyPos] & ~KING) == enemy && pieces[nextPos] == EMPTY)
                {
                    tempMoves = simpleAttack(nextPos, color, enemy);
                    moves.append(addMove(new Move(pos, nextPos), tempMoves));
                }
            }
            // Диагонали v/  ^\
            if (x > 1 && y + i > 0 && y + i < 7)
            {
                enemyPos = colLineToPos(x - 1, y + i);
                nextPos = colLineToPos(x - 2, y + 2 * i);

                if ((pieces[enemyPos] & ~KING) == enemy && pieces[nextPos] == EMPTY)
                {
                    tempMoves = simpleAttack(nextPos, color, enemy);
                    moves.append(addMove(new Move(pos, nextPos), tempMoves));
                }
            }

            if (moves.isEmpty())
                moves.push_back(new List());

            return moves;
        }

        // Соответствует последнему направлению, использованному в исследовании
        private const int NONE = 0;        // Начало
        private const int LEFT_BELOW = 1; // Диагональ v/
        private const int LEFT_ABOVE = 2; // диагональ ^\
        private const int RIGHT_BELOW = 3; // Диагональ \v
        private const int RIGHT_ABOVE = 4; // Диагональ /^


        //   Находит все разрешенные приемы атаки для текущего игрока с фигурами короля
        private List kingAttack(List lastPos, int pos, int dir, int color, int enemy)
        {
            List tempMoves, moves = new List();

            if (dir != RIGHT_BELOW)
            {
                tempMoves = kingDiagAttack(lastPos, pos, color, enemy, 1, 1);

                if (notNull(tempMoves))
                    moves.append(tempMoves);
            }

            if (dir != LEFT_ABOVE)
            {
                tempMoves = kingDiagAttack(lastPos, pos, color, enemy, -1, -1);

                if (notNull(tempMoves))
                    moves.append(tempMoves);
            }


            if (dir != RIGHT_ABOVE)
            {
                tempMoves = kingDiagAttack(lastPos, pos, color, enemy, 1, -1);

                if (notNull(tempMoves))
                    moves.append(tempMoves);
            }

            if (dir != LEFT_BELOW)
            {
                tempMoves = kingDiagAttack(lastPos, pos, color, enemy, -1, 1);

                if (notNull(tempMoves))
                    moves.append(tempMoves);
            }
            return moves;
        }

        //   Находит все разрешенные ходы атаки для текущего игрока 
        //   с фигурами короля на одну диагональ
        private List kingDiagAttack(List lastPos, int pos, int color, int enemy, int incX, int incY)
        {
            int x = posToCol(pos);
            int y = posToLine(pos);
            int i, j;
            List moves = new List();
            List tempMoves, tempPos;


            int startPos = (int)lastPos.peek_head();

            i = x + incX;
            j = y + incY;

            // Находит врага
            while (i > 0 && i < 7 && j > 0 && j < 7 && (pieces[colLineToPos(i, j)] == EMPTY || colLineToPos(i, j) == startPos))
            {
                i += incX;
                j += incY;
            }

            if (i > 0 && i < 7 && j > 0 && j < 7 && (pieces[colLineToPos(i, j)] & ~KING) == enemy && !lastPos.has(colLineToPos(i, j)))
            {

                lastPos.push_back(colLineToPos(i, j));

                i += incX;
                j += incY;

                int saveI = i;
                int saveJ = j;
                while (i >= 0 && i <= 7 && j >= 0 && j <= 7 &&
                     (pieces[colLineToPos(i, j)] == EMPTY || colLineToPos(i, j) == startPos))
                {

                    int dir;

                    if (incX == 1 && incY == 1)
                        dir = LEFT_ABOVE;
                    else if (incX == -1 && incY == -1)
                        dir = RIGHT_BELOW;
                    else if (incX == -1 && incY == 1)
                        dir = RIGHT_ABOVE;
                    else
                        dir = LEFT_BELOW;


                    tempPos = (List)lastPos.clone();
                    tempMoves = kingAttack(tempPos, colLineToPos(i, j), dir, color, enemy);

                    if (notNull(tempMoves))
                        moves.append(addMove(new Move(pos, colLineToPos(i, j)), tempMoves));

                    i += incX;
                    j += incY;
                }

                lastPos.pop_back();

                if (moves.isEmpty())
                {
                    i = saveI;
                    j = saveJ;

                    while (i >= 0 && i <= 7 && j >= 0 && j <= 7 &&
                           (pieces[colLineToPos(i, j)] == EMPTY || colLineToPos(i, j) == startPos))
                    {

                        tempMoves = new List();
                        tempMoves.push_back(new Move(pos, colLineToPos(i, j)));
                        moves.push_back(tempMoves);

                        i += incX;
                        j += incY;
                    }
                }
            }

            return moves;
        }

        //  Проверяет, что список списков не равен нулю.
        private bool notNull(List moves)
        {
            return !moves.isEmpty() && !((List)moves.peek_head()).isEmpty();
        }

        //   Добавляет новое игровое движение в начало всех списков
        private List addMove(Move move, List moves)
        {
            if (move == null)
                return moves;

            List current, temp = new List();
            while (!moves.isEmpty())
            {
                current = (List)moves.pop_front();
                current.push_front(move);
                temp.push_back(current);
            }

            return temp;
        }

        private List generateMoves(int color, int enemy)
        {
            List moves = new List();
            List tempMove;


            for (int k = 0; k < 32; k++)
                if ((pieces[k] & ~KING) == currentPlayer)
                {
                    int x = posToCol(k);
                    int y = posToLine(k);
                    int i, j;

                    if ((pieces[k] & KING) == 0)
                    {
                        i = (color == WHITE) ? -1 : 1;

                        // Диагональ /^ e \v
                        if (x < 7 && y + i >= 0 && y + i <= 7 &&
                            pieces[colLineToPos(x + 1, y + i)] == EMPTY)
                        {
                            tempMove = new List();
                            tempMove.push_back(new Move(k, colLineToPos(x + 1, y + i)));
                            moves.push_back(tempMove);
                        }


                        // Диагональ ^\ e v/
                        if (x > 0 && y + i >= 0 && y + i <= 7 &&
                            pieces[colLineToPos(x - 1, y + i)] == EMPTY)
                        {
                            tempMove = new List();
                            tempMove.push_back(new Move(k, colLineToPos(x - 1, y + i)));
                            moves.push_back(tempMove);
                        };
                    }
                    else
                    { // Королевская фигура
                      // Диагональ \v
                        i = x + 1;
                        j = y + 1;

                        while (i <= 7 && j <= 7 && pieces[colLineToPos(i, j)] == EMPTY)
                        {
                            tempMove = new List();
                            tempMove.push_back(new Move(k, colLineToPos(i, j)));
                            moves.push_back(tempMove);

                            i++;
                            j++;
                        }


                        // Диагональ ^\
                        i = x - 1;
                        j = y - 1;
                        while (i >= 0 && j >= 0 && pieces[colLineToPos(i, j)] == EMPTY)
                        {
                            tempMove = new List();
                            tempMove.push_back(new Move(k, colLineToPos(i, j)));
                            moves.push_back(tempMove);

                            i--;
                            j--;
                        }

                        // Диагональ /^
                        i = x + 1;
                        j = y - 1;
                        while (i <= 7 && j >= 0 && pieces[colLineToPos(i, j)] == EMPTY)
                        {
                            tempMove = new List();
                            tempMove.push_back(new Move(k, colLineToPos(i, j)));
                            moves.push_back(tempMove);

                            i++;
                            j--;
                        }

                        // Диагональ v/
                        i = x - 1;
                        j = y + 1;
                        while (i >= 0 && j <= 7 && pieces[colLineToPos(i, j)] == EMPTY)
                        {
                            tempMove = new List();
                            tempMove.push_back(new Move(k, colLineToPos(i, j)));
                            moves.push_back(tempMove);

                            i--;
                            j++;
                        }
                    }
                }
            return moves;
        }

        //   Проверяет ход игры
        public bool isValidMove(int from, int to)
        {
            // Если аргументы недействительны то же самое относится и к игровому движению
            if (from < 0 || from > 32 || to < 0 || to > 32)
                return false;

            // Если фигуры от или до не пусты ход игры недействителен
            if (pieces[from] == EMPTY || pieces[to] != EMPTY)
                return false;

            if ((pieces[from] & ~KING) != currentPlayer)
                return false;


            int color;
            int enemy;
            color = pieces[from] & ~KING;
            if (color == WHITE)
                enemy = BLACK;
            else
                enemy = WHITE;


            int fromLine = posToLine(from);
            int fromCol = posToCol(from);
            int toLine = posToLine(to);
            int toCol = posToCol(to);

            int incX, incY;

            if (fromCol > toCol)
                incX = -1;
            else
                incX = 1;


            if (fromLine > toLine)
                incY = -1;
            else
                incY = 1;

            int x = fromCol + incX;
            int y = fromLine + incY;


            if ((pieces[from] & KING) == 0)
            { // Простая фигура
                bool goodDir;

                if ((incY == -1 && color == WHITE) || (incY == 1 && color == BLACK))
                    goodDir = true;
                else
                    goodDir = false;

                if (x == toCol && y == toLine) // Простой ход
                    return goodDir/* && mustAttack()*/;

                // Если это был не простой ход, то это может быть только атакующий ход
                return goodDir && x + incX == toCol && y + incY == toLine && (pieces[colLineToPos(x, y)] & ~KING) == enemy;
            }
            else
            {
                bool goodDir2;
                // Королевская фигура
                while (x != toCol && y != toLine && pieces[colLineToPos(x, y)] == EMPTY)
                {
                    x += incX;
                    y += incY;
                }

                // Простой ход королевской фигурой
                if (x == toCol && y == toLine)
                {
                    goodDir2 = true;
                    return goodDir2/*!mustAttack()*/;
                }


                if ((pieces[colLineToPos(x, y)] & ~KING) == enemy)
                {
                    x += incX;
                    y += incY;

                    while (x != toCol && y != toLine && pieces[colLineToPos(x, y)] == EMPTY)
                    {
                        x += incX;
                        y += incY;
                    }

                    if (x == toCol && y == toLine)
                        return true;
                }
            }

            return false;
        }

        // Проверяет, должен ли текущий игрок атаковать
        // верно, если есть вражеские фигуры для атаки
        public bool mustAttack()
        {
            for (int i = 0; i < 32; i++)
                if ((pieces[i] & ~KING) == currentPlayer && mayAttack(i))
                    return true;

            return false;
        }

        //   Проверяет, атакует ли данная фигура какую-либо позицию
        //  верно, если на позиции есть фигура, и она атакует вражеские фигуры.
        public bool mayAttack(int pos)
        {
            if (pieces[pos] == EMPTY)
                return false;

            int color;
            int enemy;

            color = pieces[pos] & ~KING;
            if (color == WHITE)
                enemy = BLACK;
            else
                enemy = WHITE;

            int x = posToCol(pos);
            int y = posToLine(pos);

            if ((pieces[pos] & KING) == 0)
            { // Простая фигура
                int i;

                i = (color == WHITE) ? -1 : 1;

                // Диагональ /^  \v
                if (x < 6 && y + i > 0 && y + i < 7 && (pieces[colLineToPos(x + 1, y + i)] & ~KING) == enemy && pieces[colLineToPos(x + 2, y + 2 * i)] == EMPTY)
                    return true;
                // Диагональ ^\  v/
                if (x > 1 && y + i > 0 && y + i < 7 && (pieces[colLineToPos(x - 1, y + i)] & ~KING) == enemy && pieces[colLineToPos(x - 2, y + 2 * i)] == EMPTY)
                    return true;

            }
            else
            { // Королевская фигура
                int i, j;
                // Диагональ \v
                i = x + 1;
                j = y + 1;
                while (i < 6 && j < 6 && pieces[colLineToPos(i, j)] == EMPTY)
                {
                    i++;
                    j++;
                }

                if (i < 7 && j < 7 && (pieces[colLineToPos(i, j)] & ~KING) == enemy)
                {
                    i++;
                    j++;

                    if (i <= 7 && j <= 7 && pieces[colLineToPos(i, j)] == EMPTY)
                        return true;
                }

                // Диагональ ^\
                i = x - 1;
                j = y - 1;
                while (i > 1 && j > 1 && pieces[colLineToPos(i, j)] == EMPTY)
                {
                    i--;
                    j--;
                }

                if (i > 0 && j > 0 && (pieces[colLineToPos(i, j)] & ~KING) == enemy)
                {
                    i--;
                    j--;

                    if (i >= 0 && j >= 0 && pieces[colLineToPos(i, j)] == EMPTY)
                        return true;
                }

                // Диагональ /^
                i = x + 1;
                j = y - 1;
                while (i < 6 && j > 1 && pieces[colLineToPos(i, j)] == EMPTY)
                {
                    i++;
                    j--;
                }

                if (i < 7 && j > 0 && (pieces[colLineToPos(i, j)] & ~KING) == enemy)
                {
                    i++;
                    j--;

                    if (i <= 7 && j >= 0 && pieces[colLineToPos(i, j)] == EMPTY)
                        return true;
                }

                // Диагональ v/
                i = x - 1;
                j = y + 1;
                while (i > 1 && j < 6 && pieces[colLineToPos(i, j)] == EMPTY)
                {
                    i--;
                    j++;
                }

                if (i > 0 && j < 7 && (pieces[colLineToPos(i, j)] & ~KING) == enemy)
                {
                    i--;
                    j++;

                    if (i >= 0 && j <= 7 && pieces[colLineToPos(i, j)] == EMPTY)
                        return true;
                }
            }
            return false;
        }

        //   Выполняет многократное игровое движение
        public void move(int from, int to)
        {
            bool haveToAttack = mustAttack();

            applyMove(from, to);

            if (!haveToAttack)
                changeSide();
            else
              if (!mayAttack(to))
                changeSide();
        }

        //   Выполняет одно  игровое движение
        public void move(List moves)
        {
            Move move;
            Enumeration iter = moves.elements();

            while (iter.hasMoreElements())
            {
                move = (Move)iter.nextElement();
                applyMove(move.getFrom(), move.getTo());
            }
            changeSide();
        }

        //  Изменяет текущего игрока
        private void changeSide()
        {
            if (currentPlayer == WHITE)
                currentPlayer = BLACK;
            else
                currentPlayer = WHITE;
        }

        //   Изменяет информацию о доске, применяя игровой ход
        private void applyMove(int from, int to)
        {

            clearPiece(from, to);
            // выполняет движение
            if (to < 4 && pieces[from] == WHITE)
                pieces[to] = WHITE_KING;
            else if (to > 27 && pieces[from] == BLACK)
                pieces[to] = BLACK_KING;
            else
                pieces[to] = pieces[from];

            pieces[from] = EMPTY;
        }

        //  Возвращает фигуру в заданной позиции
        public byte getPiece(int pos)
        {
            return pieces[pos];
        }

        //   Проверяет, закончилась ли игра
        public bool hasEnded()
        {
            return whitePieces == 0 || blackPieces == 0 || !notNull(legalMoves());
        }

        //   Указывает, какая сторона владеет игрой
        public int winner()
        {
            if (currentPlayer == WHITE)
                if (notNull(legalMoves()))
                    return WHITE;
                else
                    return BLACK;
            else if (notNull(legalMoves()))
                return BLACK;
            else
                return WHITE;
        }

        //  Удаляет фигуру с доски между от и до
        private void clearPiece(int from, int to)
        {
            int fromLine = posToLine(from);
            int fromCol = posToCol(from);
            int toLine = posToLine(to);
            int toCol = posToCol(to);

            int i, j;

            if (fromCol > toCol)
                i = -1;
            else
                i = 1;


            if (fromLine > toLine)
                j = -1;
            else
                j = 1;

            fromCol += i;
            fromLine += j;

            while (fromLine != toLine && fromCol != toCol)
            {
                int pos = colLineToPos(fromCol, fromLine);
                int piece = pieces[pos];

                if ((piece & ~KING) == WHITE)
                    whitePieces--;
                else if ((piece & ~KING) == BLACK)
                    blackPieces--;

                pieces[pos] = EMPTY;
                fromCol += i;
                fromLine += j;
            }
        }
        //   Подготавливает доску к новой игре
        public void clearBoard()
        {
            int i;


            whitePieces = 12;
            blackPieces = 12;

            currentPlayer = BLACK;

            for (i = 0; i < 12; i++)
                pieces[i] = BLACK;

            for (i = 12; i < 20; i++)
                pieces[i] = EMPTY;

            for (i = 20; i < 32; i++)
                pieces[i] = WHITE;
        }
        //   Проверяет, является ли аргумент четным
        //   true, если значение равно четному
        private bool isEven(int value)
        {
            return value % 2 == 0;
        }
        //   Преобразует столбец/строку в позицию шашек

        // col Колонка шахматной доски (0-7)
        //   line Линия шахматной доски (0-7)
        private int colLineToPos(int col, int line)
        {
            if (isEven(line))
                return line * 4 + (col - 1) / 2;
            else
                return line * 4 + col / 2;
        }
        //   Преобразует позицию в строку
        //  value Положение шашечной доски
        private int posToLine(int value)
        {
            return value / 4;
        }
        //   Преобразует позицию в столбец
        private int posToCol(int value)
        {
            return (value % 4) * 2 + ((value / 4) % 2 == 0 ? 1 : 0);
        }
    }
}
