using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab10for12;
using Lab12;

namespace Lab13
{
    class Program
    {
        static Random rnd = new Random();
        static void Main(string[] args)
        {
            MyNewCollection<Trial> newCol1 = new MyNewCollection<Trial>();
            MyNewCollection<Trial> newCol2 = new MyNewCollection<Trial>();
            newCol1.Name = "First";
            newCol2.Name = "Second";
            Journal joun1 = new Journal();
            Journal joun2 = new Journal();

            //подписка на события = присоединение обработчика к event-объекту
            newCol1.CollectionCountChanged += new CollectionHandler(joun1.CollectionCountChanged);         
            newCol1.CollectionReferenceChanged += new CollectionHandler(joun2.CollectionReferenceChanged);
            newCol2.CollectionCountChanged += new CollectionHandler(joun1.CollectionCountChanged);
            newCol2.CollectionReferenceChanged += new CollectionHandler(joun2.CollectionReferenceChanged);

            newCol1.Add();
            newCol1.Add();
            newCol1.Add();
            newCol2.Add();
            newCol2.Add();
            newCol2.Add();
            newCol1.Delete();
            newCol2.ChangeValue(1);
            newCol1.ChangeValue(0);
            Console.WriteLine();
            Console.WriteLine("Проверка");
            Console.WriteLine();
            Console.WriteLine("Вывод первой коллекции:");
            newCol1.Show();
            Console.WriteLine();
            Console.WriteLine("Вывод второй коллекции:");
            newCol2.Show();
            Console.WriteLine();
            Console.WriteLine("Вывод первого журнала:");
            joun1.ToString();
            Console.WriteLine();
            Console.WriteLine("Вывод второго журнала:");
            joun2.ToString();
        }
    }

    public class MyCollection<T> : List<Trial>
    {
        static Random rnd = new Random();
        public List<Trial> myList;

        public MyCollection() //предназначен для создания пустой коллекции.
        {
            myList = new List<Trial>();
        }
        public MyCollection(int capacity) // создает пустую коллекцию с начальной емкостью, заданной параметром capacity.
        {
            myList = new List<Trial>(capacity);
        }
        public MyCollection(MyCollection<T> c) //служит для создания коллекции, которая инициализируется элементами
                                               //и емкостью коллекции, заданной параметром с.
        {
            List<Trial> newList = c.myList;
        }

        public MyCollection<T> Add(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Trial item = new Trial(rnd);
                this.myList.Add(item);
            }
            return this;
        }

        public void Show()
        {
            foreach (Trial item in myList)
            {
                Console.WriteLine(item);
            }
        }

        public MyCollection<T> Delete(int num)
        {
            for (int i = 0; i < num; i++)
            {
                this.myList.RemoveAt(i);
            }
            return this;
        }

        public void Sort(MyCollection<Trial> first, out MyCollection<Trial> sorted)
        {
            int max = -1;
            Trial maxTr = new Trial();
            foreach (Trial item in first.myList)
            {
                if (item.Mark >= max)
                {
                    max = item.Mark;
                }
            } //находим испытание с максиимальной оценкой
            sorted = new MyCollection<Trial>();
            //первый элемент - найденный (т.к. впоследствии окажется последним)
            foreach (Trial item in first.myList)
            {
                if (item.Mark == max)
                {
                    sorted.myList.Add(item);//добавляем элементы с оценками = максимуму в начало
                }
            }
            max--;
            do
            {
                foreach (Trial item in first.myList)
                {
                    if (item.Mark == max)
                    {
                        sorted.myList.Add(item); // добавляем все элементы с оценками на 1 ниже
                    }
                }
                max--;
            } while (max != -1);
        }

        public new MyCollection<T> Clear()
        {
            myList.Clear();
            Console.Clear();
            Console.WriteLine("Коллекция удалена");
            return this;
        }

        public int Length()
        {
            int count = 0;
            foreach (Trial item in this)
            {
                count++;
            }
            return count;
        }
    }

    public delegate void CollectionHandler(object source, CollectionHandlerEventArgs args);

    public class MyNewCollection<T> : MyCollection<T> //класс создающий события
    {
        public string Name { get; set; }
        static Random rnd = new Random();
        public MyNewCollection() //предназначен для создания пустой коллекции.
        {
            myList = new List<Trial>();
        }
        public MyNewCollection(int capacity) // создает пустую коллекцию с начальной емкостью, заданной параметром capacity.
        {
            myList = new List<Trial>(capacity);
        }
        public MyNewCollection(MyNewCollection<T> c) //служит для создания коллекции, которая инициализируется элементами
                                               //и емкостью коллекции, заданной параметром с.
        {
            List<Trial> newList = c.myList;
        }

        public MyNewCollection<T> Add()
        {
            Trial t = new Trial(rnd);
            this.myList.Add(t);
            OnCollectionCountChanged(this, new CollectionHandlerEventArgs(this.Name, "Add", myList[myList.IndexOf(t)]));
            Console.WriteLine(new CollectionHandlerEventArgs(this.Name, "add", myList[myList.IndexOf(t)]));
            return this;
        }

        public MyNewCollection<T> Delete()
        {
            OnCollectionCountChanged(this, new CollectionHandlerEventArgs(this.Name, "Delete", myList[0]));
            Console.WriteLine(new CollectionHandlerEventArgs(this.Name, "delete", myList[0]));
            this.myList.RemoveAt(0);
            return this;
        }

        public void ChangeValue(int index)
        {
            if (myList.Count < index)
            {
                //ничего не делаем, такого элемента нет
            }
            else
            {
                Trial t = new Trial(rnd);
                OnCollectionReferenceChanged(this, new CollectionHandlerEventArgs(this.Name, "changed", myList[index]));
                Console.WriteLine(new CollectionHandlerEventArgs(this.Name, "change", myList[index]));
                myList[index].Day = t.Day;
                myList[index].Month = t.Month;
                myList[index].Mark = t.Mark;
                myList[index].data = t.data;
            }
        }
        public bool Remove(int j)
        {
            if (myList.Count < j)
                return false;
            else
            {
                this.myList.RemoveAt(j);
                return true;
            }
        }

        public new Trial this[int index]
        {
            get
            {
                return myList[index];
            }
            set
            {
                OnCollectionReferenceChanged(this, new CollectionHandlerEventArgs(this.Name, "changed", myList[index]));
                myList[index] = value;
            }
        }

        //СОБЫТИЕ, происходит при добавлении нового элемента или при удалении элемента из коллекции
        public event CollectionHandler CollectionCountChanged;
        //СОБЫТИЕ, происходит при присвоении нового значения объекту коллекции   
        public event CollectionHandler CollectionReferenceChanged;
        //обработчик события CollectionCountChanged
        public virtual void OnCollectionCountChanged(object source, CollectionHandlerEventArgs args)
        {
            if (CollectionCountChanged != null)
                CollectionCountChanged(source, args);
        }
        //обработчик события OnCollectionReferenceChanged
        public virtual void OnCollectionReferenceChanged(object source, CollectionHandlerEventArgs args)
        {
            if (CollectionReferenceChanged != null)
                CollectionReferenceChanged(source, args);
        }
    }

    public class CollectionHandlerEventArgs : System.EventArgs
    {
        public string NameCollection { get; set; }
        public string ChangeCollection { get; set; }
        public object Obj { get; set; }

        public CollectionHandlerEventArgs(string name, string type, object obj)
        {
            NameCollection = name;
            ChangeCollection = type;
            Obj = obj;
        }
        public override string ToString()
        {
            return "Имя коллекции: " + this.NameCollection + ". Тип изменения: " + this.ChangeCollection + ". Объект: " + this.Obj;
        }
    }

    public class Journal //класс получатель, содержит информацию об изменениях в коллекции
    {
        private List<JournalEntry> journal;

        public Journal()
        {
            journal = new List<JournalEntry>();
        }
        public void CollectionCountChanged(object sourse, CollectionHandlerEventArgs e)
        {
            JournalEntry je = new JournalEntry(e.NameCollection, e.ChangeCollection, e.Obj.ToString());
            journal.Add(je);
        }
        public void CollectionReferenceChanged(object sourse, CollectionHandlerEventArgs e)
        {
            JournalEntry je = new JournalEntry(e.NameCollection, e.ChangeCollection, e.Obj.ToString());
            journal.Add(je);
        }
        public override string ToString()
        {
            foreach(JournalEntry item in journal)
            {
                Console.WriteLine(item.ToString());
            }
            return "";
        }
    }
    public class JournalEntry
    {
        public string Name { get; set; }
        public string TypeOfChange { get; set; }
        public string ObjectData { get; set; }

        public JournalEntry(string name, string type, string data)
        {
            Name = name;
            TypeOfChange = type;
            ObjectData = data;
        }

        public override string ToString()
        {
            return "Имя коллекции: " + this.Name + ". Тип изменения: " + this.TypeOfChange + ". Объект: " + this.ObjectData;
        }
    }
}
