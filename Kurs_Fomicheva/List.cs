using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurs_Fomicheva
{
    internal interface Enumeration
    {

        //   Используется для проверки, есть ли еще элементы для итерации.
        //  true, если есть больше элементов для итерации, false в противном случае

        bool hasMoreElements();


        //  Возвращает текущий элемент и продвигает итератор.
        object nextElement();
    }
    internal class NoSuchElementException : Exception
    {
    }
    // Список узлов
    internal class ListNode
    {
        internal ListNode prev, next;
        internal object value;
        
        public ListNode(object elem, ListNode prevNode, ListNode nextNode)
        {
            value = elem;
            prev = prevNode;
            next = nextNode;
        }
    }
    internal class List
    {
        private ListNode head;
        private ListNode tail;
        private int count;

        public List()
        {
            count = 0;
        }


        //  Добавляет элемент в начало списка
        public void push_front(object elem)
        {
            ListNode node = new ListNode(elem, null, head);

            if (head != null)
                head.prev = node;
            else
                tail = node;

            head = node;
            count++;
        }

        //  Добавляет элемент в конец списка

        // elem Элемент, который будет добавлен

        public void push_back(object elem)
        {
            ListNode node = new ListNode(elem, tail, null);

            if (tail != null)
                tail.next = node;
            else
                head = node;

            tail = node;
            count++;
        }


        //  Удаляет элемент из заголовка списка
        //  Удаленный элемент

        public object pop_front()
        {
            if (head == null)
                return null;

            ListNode node = head;
            head = head.next;

            if (head != null)
                head.prev = null;
            else
                tail = null;

            count--;
            return node.value;
        }

        //  Удаляет элемент из хвоста списка
        //  Удкалённый элемент

        public object pop_back()
        {
            if (tail == null)
                return null;

            ListNode node = tail;
            tail = tail.prev;

            if (tail != null)
                tail.next = null;
            else
                head = null;

            count--;
            return node.value;
        }


        //  Проверяет, является ли список пустым
        public bool isEmpty()
        {
            return head == null;
        }


        //  Возвращает количество элементов списка

        public int length()
        {
            return count;
        }


        //  Добавляет еще один список в конец списка
        //  other список, который будет добавлен

        public void append(List other)
        {
            ListNode node = other.head;

            while (node != null)
            {
                push_back(node.value);
                node = node.next;
            }
        }

        public void clear()
        {
            head = tail = null;
        }


        // Возвращает элемент из заголовка списка, не удаляя его.
        //  Глава списка

        public object peek_head()
        {
            if (head != null)
                return head.value;
            else
                return null;
        }


        // Возвращает элемент из конца списка, не удаляя его.
        // Хвост списка

        public object peek_tail()
        {
            if (tail != null)
                return tail.value;
            else
                return null;
        }



        //  Проверяет, существует ли данный элемент в списке.
        //  elem Элемент для поиска в списке true, если элемент был найден, false в противном случае

        public bool has(object elem)
        {
            ListNode node = head;

            while (node != null && !node.value.Equals(elem))
                node = node.next;

            return node != null;
        }


        //  Клонирует список в мелкой копии
        //  Клон экземпляра, в котором был вызван метод

        public object clone()
        {
            List temp = new List();
            ListNode node = head;

            while (node != null)
            {
                temp.push_back(node.value);
                node = node.next;
            }

            return temp;
        }


        //  Создает строковое представление списка. В виде [элемент1, элемент2, ... ]
/*        public override string ToString()
        {
            string temp = "[";
            ListNode node = head;

            while (node != null)
            {
                temp += node.value.ToString();
                node = node.next;
                if (node != null)
                    temp += ", ";
            }
            temp += "]";

            return temp;
        }*/

        //  Итератор списка
        class Enum : Enumeration
        {
            // Текущий элемент
            private ListNode node;

            internal Enum(ListNode start)
            {
                node = start;
            }


            //  Используется для проверки, есть ли еще элементы для итерации.
            public bool hasMoreElements()
            {
                return node != null;
            }


            //  Возвращает текущий элемент и продвигает итератор.

            public object nextElement()
            {
                Object temp;

                if (node == null)
                    throw new NoSuchElementException();

                temp = node.value;
                node = node.next;

                return temp;
            }
        }


        // Возвращает итератор списка.
        //  Итератор списка
        public Enumeration elements()
        {
            return new Enum(head);
        }
    }
}
