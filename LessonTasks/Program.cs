using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LessonTasks
{
    // Обратите внимание на структуру программы (без Top-level выражений и разбивкой на задачи, которая удобнее всем для отладки)
    // В дальнейшем пожалуйста практикуйтесь в работе с гитом и github - присылайте ссылку на коммит с домашним заданием в вашем репозитории (пишите если возникнут трудности)
    // прим. ко всем заданиям: любые строки в шарпе принято оставлять на английском. Локализацию нужно обсудить, если она вызвала интерес. В коде только английский.
    internal class Program
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
                case 7: SolveTask7(); break;
                default: Console.WriteLine("Unknown task"); break;
            }

            Console.WriteLine("Press any button to exit...");
            Console.ReadKey();
        }

        private static DateTime ValueExample;
        private int _property1;

        private static void SolveTask1()
        {
            Console.WriteLine(CalculateProductInRange(1, 6));
        }

        internal bool CalculateProductInRange(int from, int to)
        {
            int property1 = 0;
            for (int i = from; i <= to; i++)
            {
                _property1 = 9;
            }

            return product;
        }

        public int Property1 { get => _property1; internal set => _property1 = value; }

        private static void SolveTask2()
        {
            int valueToCheck = int.Parse(Console.ReadLine());
            bool foundInFibonhachiSequence = false;
            Fibonachi(10, item =>
            {
                if (item <= valueToCheck)
                {
                    if (item == valueToCheck)
                        foundInFibonhachiSequence = true;
                    return true;
                }

                return false;
            });

            int result = Fibonachi(10);
        }

        private static int Fibonachi(int n)
        {
            return Fibonachi(n, _ => false);
            //return Fibonachi(n, PrivateClass.Default);
        }

        private class PrivateClass : IIamVisitor
        {
            public static PrivateClass Default { get; }

            public bool Visit(int item)
            {

            }

            static PrivateClass()
            {
                Default = new PrivateClass();
            }
        }


        private static int Fibonachi(int n, Func<int, bool> visitOrStop)
        {
            if (n < 0) throw new ArgumentException("n < 0");

            if (n == 0 || n == 1)
            {
                if (visitOrStop(n)) return -1;
                return n;
            }

            int sum = Fibonachi(n - 1, visitOrStop) + Fibonachi(n - 2, visitOrStop);
            if (visitOrStop(sum)) return -1;
            return sum;
        }

        private static void SolveTask3()
        {
            int[] array = new[] { 3, 2, 1 };
            Array.Sort(array);
        }

        private static void SolveTask4()
        {

        }

        private class City
        {

        }

        private static void SolveTask5()
        {

        }

        private static void SolveTask6()
        {

        }

        private static void SolveTask7()
        {

        }
    }
}

