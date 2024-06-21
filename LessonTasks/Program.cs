using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonTasks
{
    class SickClass
    {
        private int _field;

        public static void Increment(SickClass self)
        {
            self._field++;
        }
    }

    class HealthyClass
    {
        private int _field;

        public void Increment()
        {
            _field++;
        }
    }

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
                case 5: SolveTask5(); break;
                case 6: SolveTask6(); break;
                default: Console.WriteLine("Unknown task"); break;
            }
        }

        private static void SolveTask1()
        {
            //const string separator = "|";
            //string caption = null;
            //string body = null;
            //if (caption.Contains())
            //File.WriteAllText("asdf.txt", string.Join(separator, caption, body));
        }

        private static void SolveTask2()
        {
            int count = 0;
            Action<char> increment = symbol => count++;
            for (int i = 0; i < 10; i++)
            {
                increment('*');
            }

            count -= 1;

            Console.WriteLine($"asdf: {count}");

            string asdf = "";
            //SolveTask3(asdf.Contains);
        }

        private static void SolveTask3()
        {
            var sick = new SickClass();
            SickClass.Increment(sick);
            //DoManySomething(10, SickClass.Increment);
        }

        private static void SolveTask4()
        {
            int someVar = 0;
            var healthy = new HealthyClass();
            healthy.Increment();
            var healthy2 = new HealthyClass();
            var healthy3 = new HealthyClass();
            var healthy4 = new HealthyClass();
            
            Action increment1 = healthy.Increment;
            Action increment2 = healthy2.Increment;
            Action increment3 = healthy3.Increment;
            Action increment4 = healthy4.Increment;
            Action increment = increment1 + increment2 + increment3 + increment4;
            Delegate.Combine(increment1, increment2, increment3, increment4);
            //Delegate.CreateDelegate();
            DoManySomething(10, increment);
        }

        private static void DoManySomething(int count, Action action)
        {
            for (int i = 0; i < count; i++)
            {
                action();
            }
        }

        private static void SolveTask5()
        {
            char symbol;
            
            symbol = '*'; // в чем разница?

            Console.WriteLine(symbol); // смотрит на стек
            
            Action lambda1 = () =>
            {
                //char symbol = '*';

                for (int i = 0; i < 4; i++)
                {
                    Console.Write(i % 2 == 0 ? symbol : ' ');
                }
            };

            symbol = '|'; // в чем разница?

            Console.WriteLine(symbol); // смотрит на оъект замыкания, созданный для делегата lambda1

            for (int i = 0; i < 10; i++)
            {
                lambda1();
                Console.WriteLine();
            }
        }

        private static void SolveTask6()
        {
            Action<char> lambda1 = CreateLambda1();
            Action<char> lambda2 = CreateLambda2();

            Action<char> writeAction = lambda1 + lambda2;

            for (int i = 0; i < 10; i++)
            {
                writeAction('*');
                Console.WriteLine();
            }
        }

        private static Action<char> CreateLambda1()
        {
            return symbol =>
            {
                for (int i = 0; i < 4; i++)
                {
                    Console.Write(i % 2 == 0 ? symbol : ' ');
                }
            };
        }

        private static Action<char> CreateLambda2()
        {
            var random = new Random();
            int counter = 0;
            return symbol =>
            {
                counter++;
                int printIndex = random.Next(0, 4); ;
                for (int i = 0; i < 4; i++)
                {
                    Console.Write(i == printIndex ? symbol : ' ');
                }
            };
        }
    }
}
