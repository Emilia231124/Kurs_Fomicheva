using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Xml.Serialization;

//Основной класс контроллера шашек
namespace Kurs_Fomicheva
{
    public class BoardView : Control
    {
        // Сохраняет информацию о текущем состоянии платы
        private CheckersBoard board;

        // Верхний левый угол доски
        internal int startX;
        internal int startY;

        // Размер квадратов досок
        internal int cellWidth;

        // Выбранные квадраты в игровом ходе. Первый элемент - это первое движение
        internal List selected;

        // Наш противник, компьютер
        internal Computer computer;

        // Размер частей
        private static int SIZE = 0;

        // Форма, содержащая элемент управления
        private Form parent;


        //  Строит контрольные шашки.
        // Форма, в которой содержится элемент управления.
        public BoardView(Form parentComponent)
        {
            selected = new List();
            board = new CheckersBoard();
            parent = parentComponent;
            computer = new Computer(board);
            reset();
        }

        //   Позволяет пользователю изменять максимальную глубину дерева min-max,
        //   используемого в компьютерных вычислениях
        public int depth
        {
            get
            {
                return computer.depth;
            }
            set
            {
                computer.depth = value;
            }
        }

        //  Начинается новая игра. Человек первым играет черными фигурами.
        public void newGame()
        {
            board.clearBoard();
            selected.clear();
            Invalidate();
            reset();
            ChangeTitle();
        }

        // Изменяет название формы, чтобы отразить текущего игрока
        public void ChangeTitle()
        {
            if (board.getCurrentPlayer() == CheckersBoard.WHITE)
                parent.Text = "Checkers - White";
            else
                parent.Text = "Checkers - Black";
        }

        //  Сохраняет текущую игру в файловом потоке
        /*        public void saveBoard(Stream file)
                {
                    try
                    {
                        IFormatter formatter = (IFormatter)new BinaryFormatter();

                        // Сериализуйте граф объектов для потоковой передачи
                        formatter.Serialize(file, board);
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка при сохранении", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }*/

        //  Загружает сохраненную игру из потока файлов  
        /*        public void loadBoard(Stream file)
                {
                    try
                    {
                        IFormatter formatter = (IFormatter)new BinaryFormatter();

                        // Очистите выбранные ходы, на всякий случай
                        selected.clear();
                        Invalidate();
                        reset();

                        // Десериализует граф объектов для потоковой передачи
                        board = (CheckersBoard)formatter.Deserialize(file);

                        // Создайте новый экземпляр компьютера для этой платы
                        computer = new Computer(board);
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка при загрузке", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }*/

        // Рисует элемент управления шашками в ответ на событие рисования.
        // Рисует шашечное поле 
        protected override void OnPaint(PaintEventArgs ev)
        {
            Graphics g = ev.Graphics;
            Size d = ClientSize;
            int marginX;
            int marginY;
            int incValue;

            // Вычисляет приращения, чтобы мы могли получить квадратную доску
            if (d.Width < d.Height)
            {
                marginX = 0;
                marginY = (d.Height - d.Width) / 2;

                incValue = d.Width / 8;
            }
            else
            {
                marginX = (d.Width - d.Height) / 2;
                marginY = 0;

                incValue = d.Height / 8;
            }

            startX = marginX;
            startY = marginY;
            cellWidth = incValue;

            drawBoard(g, marginX, marginY, incValue);
            drawPieces(g, marginX, marginY, incValue);
        }

        // Рисует фон доски
        private void drawBoard(Graphics g, int marginX, int marginY, int incValue)
        {
            int pos;
            Brush cellColor;

            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                {
                    if ((x + y) % 2 == 0)
                        cellColor = new SolidBrush(Color.AntiqueWhite);
                    else
                    {
                        pos = y * 4 + (x + ((y % 2 == 0) ? -1 : 0)) / 2;

                        if (selected.has(pos))
                            cellColor = new SolidBrush(Color.Yellow);
                        else
                            cellColor = new SolidBrush(Color.SaddleBrown);
                    }


                    g.FillRectangle(cellColor, marginX + x * incValue, marginY + y * incValue, incValue - 1, incValue - 1);
                }
        }

        // Поле для рисования королевских фигур
        private static int KING_SIZE = 3;


        private void drawPieces(Graphics g, int marginX, int marginY, int incValue)
        {
            int x, y;
            Brush pieceColor;

            for (int i = 0; i < 32; i++)
                    if (board.getPiece(i) != CheckersBoard.EMPTY)
                    {
                        if (board.getPiece(i) == CheckersBoard.BLACK ||
                            board.getPiece(i) == CheckersBoard.BLACK_KING)
                            pieceColor = new SolidBrush(Color.Black);
                        else
                            pieceColor = new SolidBrush(Color.GhostWhite);

                        y = i / 4;
                        x = (i % 4) * 2 + (y % 2 == 0 ? 1 : 0);
                        g.FillEllipse(pieceColor, SIZE + marginX + x * incValue, SIZE + marginY + y * incValue,
                                    incValue - 1 - 2 * SIZE, incValue - 1 - 2 * SIZE);

                        if (board.getPiece(i) == CheckersBoard.WHITE_KING)
                        {
                            pieceColor = new SolidBrush(Color.Black);
                            g.DrawEllipse(new Pen(pieceColor), KING_SIZE + marginX + x * incValue, KING_SIZE + marginY + y * incValue,
                                        incValue - 1 - 2 * KING_SIZE, incValue - 1 - 2 * KING_SIZE);
                        }
                        else if (board.getPiece(i) == CheckersBoard.BLACK_KING)
                        {
                            pieceColor = new SolidBrush(Color.White);
                            g.DrawEllipse(new Pen(pieceColor), KING_SIZE + marginX + x * incValue, KING_SIZE + marginY + y * incValue,
                                        incValue - 1 - 2 * KING_SIZE, incValue - 1 - 2 * KING_SIZE);
                        }
                    }
        }


        Stack boards;

        //  Обрабатывает сообщение пользователя, нажимающего кнопку мыши.
        //  Если она нажата над фигурой, и это фигура игрока, 
        //  то она становится выбранной / невыбранной.
        protected override void OnMouseDown(MouseEventArgs e)
        {
            int pos;

            pos = getPiecePos(e.X, e.Y);
            if (pos != -1)
            {
                int piece = board.getPiece(pos);

                if (piece != CheckersBoard.EMPTY &&
                    (((piece == CheckersBoard.WHITE || piece == CheckersBoard.WHITE_KING) &&
                      board.getCurrentPlayer() == CheckersBoard.WHITE) ||
                      ((piece == CheckersBoard.BLACK || piece == CheckersBoard.BLACK_KING) &&
                      board.getCurrentPlayer() == CheckersBoard.BLACK)))
                {
                    if (selected.isEmpty())
                        selected.push_back(pos);
                    else
                    {
                        int temp = (int)selected.peek_tail();

                        if (temp == pos)
                            selected.pop_back();
                        else
                        {
                            MessageBox.Show("Не выбран", "Ошибка");
                        }
                    }
                    Invalidate();
                    Update();
                    return;
                }
                else
                {
                    bool good = false;
                    CheckersBoard tempBoard;

                    if (!selected.isEmpty())
                    {
                        if (boards.Count == 0)
                        {
                            tempBoard = (CheckersBoard)board.clone();
                            boards.Push(tempBoard);
                        }
                        else
                            tempBoard = (CheckersBoard)boards.Peek();


                        int from = (int)selected.peek_tail();
                        if (tempBoard.isValidMove(from, pos))
                        {
                            tempBoard = (CheckersBoard)tempBoard.clone();

                            /*bool isAttacking = tempBoard.mustAttack();*/
                            bool isAttacking = tempBoard.mayAttack(from);

                            tempBoard.move(from, pos); //Ключевой элемент

                            if (isAttacking && tempBoard.mayAttack(pos))
                            {
                                selected.push_back(pos);
                                boards.Push(tempBoard);

                            }
                            else
                            {
                                selected.push_back(pos);
                                makeMoves(selected, board);
                                boards = new Stack();
                            }

                            good = true;
                        }
                        else if (from == pos)
                        {
                            selected.pop_back();
                            boards.Pop();

                            good = true;
                        }
                    }

                    if (!good)
                    {
                        MessageBox.Show("Недопустимый ход", "Ошибка");
                    }
                    else
                    {
                        Invalidate();
                        Update();
                    }
                }
            }
        }

        //  Очищает временные сооружения, используемые во время каждого перемещения
        public void reset()
        {
            boards = new Stack();
        }

        // Применяет ходы игрока к доске и после этого сообщает компьютеру играть.
        private void makeMoves(List moves, CheckersBoard board)
        {
            List moveList = new List();
            int from, to = 0;

            from = (int)moves.pop_front();
            while (!moves.isEmpty())
            {
                to = (int)moves.pop_front();
                moveList.push_back(new Move(from, to));
                from = to;
            }

            board.move(moveList);
            Invalidate();
            Update();
            selected.clear();
            reset();


            if (!gameEnded())
            {
                Thread.Sleep(500); // Время хода компьютера
                ChangeTitle();
                computer.play();
                Invalidate();
                Update();

                if (!gameEnded())
                    ChangeTitle();
            }
        }


        //  Значение индекса от 0 до 31, если в заданном местоположении есть фрагмент.
        //  В противном случае он возвращает значение -1.
        private int getPiecePos(int currentX, int currentY)
        {
            for (int i = 0; i < 32; i++)
            {
                int x, y;

                y = i / 4;
                x = (i % 4) * 2 + (y % 2 == 0 ? 1 : 0);
                if (startX + x * cellWidth < currentX &&
                    currentX < startX + (x + 1) * cellWidth &&
                    startY + y * cellWidth < currentY &&
                    currentY < startY + (y + 1) * cellWidth)
                    return i;
            }

            return -1;
        }

        //  true, если игра закончилась, false в противном случае.
        private bool gameEnded()
        {
            bool result;

            int white = board.getWhitePieces();
            int black = board.getBlackPieces();
            if (board.hasEnded())
            {
                if (board.winner() == CheckersBoard.BLACK)
                    MessageBox.Show("Чёрные выиграли", "Игра окончена");
                else
                    MessageBox.Show("Белые выиграли", "Игра окончена");
                result = true;
            }
            else
                result = false;

            return result;
        }
    }
}
