using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LessonTasks
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter task number:");
            int taskNumber = int.Parse(Console.ReadLine());
            switch (taskNumber)
            {
                case 1: SolveTask1(); break;
                case 2: SolveTask2(); break;
                case 3: SolveTask3(); break;
                case 4: SolveTask4(); break;
                default: Console.WriteLine("Unknown task"); break;
            }
        }

        private static void SolveTask1()
        {
            var A = new double[5];
            var B = new double[3, 4];
            double crossMax = double.MinValue;
            double crossMin = double.MaxValue;
            double totalSum = 0;
            double totalProduct = 1;
            double sumEvenA = 0;
            double sumOddColumnB = 0;

            // это чтобы не копировать логику между обходами двух массивов
            Action<double> totalVisitor = item =>
            {
                if (item > crossMax) crossMax = item;
                if (item < crossMin) crossMin = item;
                totalSum += item;
                totalProduct *= item;
            };

            // 💩 копипаста - злооооо
            totalVisitor += item => // если ссылается на что-то извне - то генерируется как объектный метод в сгенерированном классе
            {
                if (item > crossMax) crossMax = item;
                if (item < crossMin) crossMin = item;
                totalSum += item;
                totalProduct *= item;
            };

            totalSum += 1; // он полезет не в стек, а в замыкание

            totalVisitor += Console.WriteLine;
            //totalVisitor += totalVisitor;
            //totalVisitor -= totalVisitor; -- результат null

            //totalVisitor += null; -- не ругается, ниче не меняется. _invocationList == null, как и при создании делегата

            Func<double> funcExample = () => 0; // как статический метод
            Func<double> funcExample2 = () => 1;
            //funcExample += () => 0;
            Console.WriteLine(funcExample());
            Console.WriteLine(funcExample2());

            Action anonymousFunction = delegate () // до создания лямбд
            {

            };

            Delegate.CreateDelegate(
                typeof(Func<double>),
                typeof(Console).GetMethods()
                    //.Select(m => new { m.Name, GetParameters = (Func<ParameterInfo[]>)m.GetParameters })
                    .Single(m => m.Name == nameof(Console.WriteLine) // использование метода LINQ to Object, т.е. из Enumberable
                        && m.GetParameters().Length == 1
                        && m.GetParameters()[0].ParameterType == typeof(double))
                );

            // используется для "эмуляции" SELECT в SQL
            var asdf = new // тут мог быть например StringBuilder, но мы решили объявить анонимный класс и создать его экземпляр
            {
                Capacity = 100
            };

            //asdf.Capacity = 3; // низя

            //from m 

            //Queryable. // практически тот же набор методов расширений что и у Enumerable, только над IQuaryable : IEnumerable

            //Delegate;
            //MulticastDelegate;

            Console.WriteLine($"Enter rational numbers to an array of {A.Length} elements one by one:");
            for (int i = 0; i < A.Length; i++)
            {
                Console.Write($"Element {i + 1}:");
                double item = double.Parse(Console.ReadLine());
                A[i] = item;
                totalVisitor(item);
                if (i % 2 == 0) sumEvenA += item;
            }

            int rows = B.GetLength(0);
            int columns = B.GetLength(1);

            var rand = new Random();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    double item = rand.Next(int.MinValue, int.MaxValue); // чтоб побольше числа генерить
                    B[row, col] = item;
                    totalVisitor(item);
                    if (col % 2 == 1) sumOddColumnB += item;
                }
            }

            Console.WriteLine("Populated data:");
            Console.WriteLine($"Linear array A you entered manually: {string.Join(" ", A)}"); // для демонстрации, вообще форматирование вывода на шарпе вымирает.. используйте шаблонизатор или пишите API

            Console.WriteLine("Matrix B was randomly generated:");
            int newLineCounter = 0;
            foreach (double randomized in B)
            {
                Console.Write("{0,25:E17}", randomized); // создания объекта не избежать (упаковка или строка), если хотим выровненный вывод

                if ((newLineCounter = ++newLineCounter % columns) == 0) // никогда не пишите такой ниндзя код на шарпе :)
                    Console.WriteLine();
                else
                    Console.Write(" ");
            }

            Console.WriteLine($"Cross max: {crossMax}");
            Console.WriteLine($"Cross min: {crossMin}");
            Console.WriteLine($"Total sum: {totalSum}");
            Console.WriteLine($"Total product: {totalProduct}");
            Console.WriteLine($"Sum of even A elements: {sumEvenA}");
            Console.WriteLine($"Sum of odd B columns: {sumOddColumnB}");
        }

        private static void SolveTask2()
        {
            var observable = new Observable();

            // обычно наблюдатели подписываются в разных местах
            observable.Observers += () => Console.WriteLine("Hello");
            observable.Observers += () => Console.WriteLine("Hello");
            observable.Observers += () => Console.WriteLine("Hello");
            observable.Observers += () => Console.WriteLine("Hello");

            // а этот дурачок. События ему делают а-та-та
            //observable.Observers = () => Console.WriteLine("Hello");

            observable.DoSomething();
        }

        private static void SolveTask3()
        {
            var toSort = new[] { 3, 2, 1 };
            Comparison<int> comparison = (a, b) => a.CompareTo(b); // * -1; // так можно в обратном порядке
            Array.Sort(toSort, comparison);
            Console.WriteLine(string.Join(", ", toSort));
        }

        private static void SolveTask4()
        {

        }

        private class Observable
        {
            //private Action _observers;
            private readonly List<Action> _observers = new List<Action>();

            //public event Action Observers;
            public event Action Observers
            {
                add { if (value != null) _observers.Add(value); }
                remove { if (value != null) _observers.Remove(value); }
            }

            public void DoSomething()
            {
                // тут делает что-то полезное

                // а тут дает наблюдать
                //Action observers = Observers;
                //if (observers != null)
                //    observers();

                // современный вариант
                //Observers?.Invoke(); // так только с делегатом (энкапсулируемым за счет событий)

                for (int i = 0; i < _observers.Count; i++)
                {
                    _observers[i].Invoke();
                }
            }
        }
    }
}
