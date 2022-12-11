using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Kurs_Fomicheva
{
    public partial class Form1 : Form
    {
        // Ссылка на контрольные шашки
        private BoardView m_view;

        // Опции меню для настройки уровня игры
        // Необходимо, потому что нам нужно установить / снять галочки
        private MenuItem m_easyOpt;
        private MenuItem m_mediumOpt;
        private MenuItem m_hardOpt;

        //  Создает игру с элементами управления игрой в шашки
        public Form1()
        {
            // Установите заголовок окна
            Text = "Sharp Checkers";

            // Установить размер окна
            ClientSize = new Size(400, 400);

            // Создать меню
            MainMenu menu = new MainMenu();
            MenuItem item = new MenuItem("&Файл");
            menu.MenuItems.Add(item);

            // Добавьте пункты меню в меню "Файл" 
            item.MenuItems.Add(new MenuItem("&Новая игра", new EventHandler(OnNewGame)));
            item.MenuItems.Add(new MenuItem("&Выйти", new EventHandler(OnExit)));

            // Создать новое меню
            item = new MenuItem("&Уровень сложности");
            menu.MenuItems.Add(item);

            // Добавьте пункты меню в меню "Параметры"
            m_easyOpt = new MenuItem("&Лёгкий", new EventHandler(OnEasyOpt));
            m_easyOpt.Checked = true;
            item.MenuItems.Add(m_easyOpt);

            m_mediumOpt = new MenuItem("&Средний", new EventHandler(OnMediumOpt));
            item.MenuItems.Add(m_mediumOpt);

            m_hardOpt = new MenuItem("&Сложный", new EventHandler(OnHardOpt));
            item.MenuItems.Add(m_hardOpt);

            // Прикрепить меню к окну
            Menu = menu;

            // Добавьте элемент управления checkers в форму
            m_view = new BoardView(this);
            m_view.Location = new Point(0, 0);
            m_view.Size = ClientSize;
            Controls.Add(m_view);
        }


        // Обработчик для опции "Новая игра"
        private void OnNewGame(object sender, EventArgs ev)
        {
            // Сохранить текущий уровень сложности
            int level = m_view.depth;
            m_view.newGame();
            m_view.depth = level;
        }

        // Обработчик для опции "Выход"
        private void OnExit(object sender, EventArgs ev)
        {
            Close();
        }

        // Обработчик для параметра "Лёгкий"
        private void OnEasyOpt(object sender, EventArgs ev)
        {
            m_view.depth = 2;
            m_easyOpt.Checked = true;
            m_mediumOpt.Checked = false;
            m_hardOpt.Checked = false;
        }
        // Обработчик для параметра "Средний"
        private void OnMediumOpt(object sender, EventArgs ev)
        {
            m_view.depth = 3;
            m_easyOpt.Checked = false;
            m_mediumOpt.Checked = true;
            m_hardOpt.Checked = false;
        }
        // Обработчик для параметра "Сложный"
        private void OnHardOpt(object sender, EventArgs ev)
        {
            m_view.depth = 5;
            m_easyOpt.Checked = false;
            m_mediumOpt.Checked = false;
            m_hardOpt.Checked = true;
        }

        // Обрабатывает изменение размера окна
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (m_view != null)
            {
                m_view.Size = ClientSize;
                m_view.Invalidate();
            }
        }
    }
}
