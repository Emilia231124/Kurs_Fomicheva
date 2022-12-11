using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace Kurs_Fomicheva
{
    internal class Computer
    {
        private CheckersBoard currentBoard;

        // Цвет деталей компьютера
        private int color;

        // Максимальная глубина, используемая в алгоритме Min-Max
        private int maxDepth = 1;

        // Веса, используемые для доски
        private int[] tableWeight = { 4, 4, 4, 4,
                                 4, 3, 3, 3,
                                 3, 2, 2, 4,
                                 4, 2, 1, 3,
                                 3, 1, 2, 4,
                                 4, 2, 2, 3,
                                 3, 3, 3, 4,
                                 4, 4, 4, 4};


        public Computer(CheckersBoard gameBoard)
        {
            currentBoard = gameBoard;
            color = CheckersBoard.BLACK;
        }


        //  Позволяет пользователю изменять максимальную глубину дерева min-max
        public int depth
        {
            get
            {
                return maxDepth;
            }
            set
            {
                maxDepth = value;
            }
        }


        //  Заставляет компьютер сделать ход на шахматной доске, которую он держит
        public void play()
        {
                List moves = minimax(currentBoard);

                if (!moves.isEmpty())
                    currentBoard.move(moves);
        }

        //   Изменяет шахматную доску, удерживаемую компьютером.

        public void setBoard(CheckersBoard board)
        {
            currentBoard = board;
        }

        //   Говорит, является ли ход игры действительным
        //  Список движений фигур для игрового хода.

        private bool mayPlay(List moves)
        {
            return !moves.isEmpty() && !((List)moves.peek_head()).isEmpty();
        }


        //  Реализует алгоритм Min-Max для выбора перемещения компьютера

        //   Доска, которая будет использоваться в качестве отправной точки
        //   для создания игровых движений

        private List minimax(CheckersBoard board)
        {
            List sucessors;
            List move, bestMove = null;
            CheckersBoard nextBoard;
            int value, maxValue = Int32.MinValue;

            sucessors = board.legalMoves();
            while (mayPlay(sucessors))
            {
                move = (List)sucessors.pop_front();
                nextBoard = (CheckersBoard)board.clone();

                Debug.WriteLine("******************************************************************");
                nextBoard.move(move);
                value = minMove(nextBoard, 1, maxValue, Int32.MaxValue);

                if (value > maxValue)
                {
                    Debug.WriteLine("Max value : " + value + " at depth : 0");
                    maxValue = value;
                    bestMove = move;
                }
            }
            Debug.WriteLine("Move value selected : " + maxValue + " at depth : 0");

            return bestMove;
        }

        //   Реализует оценку хода игры c точки зрения максимального игрока

        // board Доска, которая будет использоваться в качестве
        // отправной точки для создания игровых движений

        //  depth Текущая глубина в дереве Min-Max


        //  alpha Cтекущее альфа-значение для альфа-бета-отсечки
        //  beta Текущее бета-значение для альфа-бета-отсечки

        private int maxMove(CheckersBoard board, int depth, int alpha, int beta)
        {
            if (cutOffTest(board, depth))
                return eval(board);


            List sucessors;
            List move;
            CheckersBoard nextBoard;
            int value;

            Debug.WriteLine("Max node at depth : " + depth + " with alpha : " + alpha + " beta : " + beta);

            sucessors = board.legalMoves();
            while (mayPlay(sucessors))
            {
                move = (List)sucessors.pop_front();
                nextBoard = (CheckersBoard)board.clone();
                nextBoard.move(move);
                value = minMove(nextBoard, depth + 1, alpha, beta);

                if (value > alpha)
                {
                    alpha = value;
                    Debug.WriteLine("Max value : " + value + " at depth : " + depth);
                }

                if (alpha > beta)
                {
                    Debug.WriteLine("Max value with prunning : " + beta + " at depth : " + depth);
                    Debug.WriteLine(sucessors.length() + " sucessors left");
                    return beta;
                }

            }
            Debug.WriteLine("Max value selected : " + alpha + " at depth : " + depth);
            return alpha;
        }

        //   Реализует оценку хода игры с точки зрения МИНИМАЛЬНОГО игрока


        //   board Доска, которая будет использоваться в качестве отправной точки
        //   для создания игровых движений

        //   depth Текущая глубина в дереве Min-Max

        //  alpha Текущее альфа-значение для альфа-бета-отсечки

        // beta Текущее бета-значение для альфа-бета-отсечки


        private int minMove(CheckersBoard board, int depth, int alpha, int beta)
        {
            if (cutOffTest(board, depth))
                return eval(board);


            List sucessors;
            List move;
            CheckersBoard nextBoard;
            int value;

            Debug.WriteLine("Min node at depth : " + depth + " with alpha : " + alpha +
                                " beta : " + beta);

            sucessors = (List)board.legalMoves();
            while (mayPlay(sucessors))
            {
                move = (List)sucessors.pop_front();
                nextBoard = (CheckersBoard)board.clone();
                nextBoard.move(move);
                value = maxMove(nextBoard, depth + 1, alpha, beta);

                if (value < beta)
                {
                    beta = value;
                    Debug.WriteLine("Min value : " + value + " at depth : " + depth);
                }

                if (beta < alpha)
                {
                    Debug.WriteLine("Min value with prunning : " + alpha + " at depth : " + depth);
                    Debug.WriteLine(sucessors.length() + " sucessors left");
                    return alpha;
                }
            }

            Debug.WriteLine("Min value selected : " + beta + " at depth : " + depth);
            return beta;
        }

        //   Оценивает силу текущего игрока
        //   board Доска, на которой будет оцениваться текущая позиция игрока.
        private int eval(CheckersBoard board)
        {
            int colorKing;
            int colorForce = 0;
            int enemyForce = 0;
            int piece;

            if (color == CheckersBoard.WHITE)
                colorKing = CheckersBoard.WHITE_KING;
            else
                colorKing = CheckersBoard.BLACK_KING;

                for (int i = 0; i < 32; i++)
                {
                    piece = board.getPiece(i);

                    if (piece != CheckersBoard.EMPTY)
                        if (piece == color || piece == colorKing)
                            colorForce += calculateValue(piece, i);
                        else
                            enemyForce += calculateValue(piece, i);
                }


            return colorForce - enemyForce;
        }

        //   piece Тип фигуры
        //   pos Позиия фигуры
        private int calculateValue(int piece, int pos)
        {
            int value;

            if (piece == CheckersBoard.WHITE) //Простая фигура
                if (pos >= 4 && pos <= 7)
                    value = 7;
                else
                    value = 5;
            else if (piece != CheckersBoard.BLACK) //Простая фигура
                if (pos >= 24 && pos <= 27)
                    value = 7;
                else
                    value = 5;
            else // Королевская фигура
                value = 10;

            return value * tableWeight[pos];
        }


        //   Проверяет, можно ли обрезать игровое дерево
        //   board Правлению для оценки
        //   depth Текущая глубина игрового дерева
        //  верно, если дерево можно обрезать

        private bool cutOffTest(CheckersBoard board, int depth)
        {
            return depth > maxDepth || board.hasEnded();
        }
    }
}
