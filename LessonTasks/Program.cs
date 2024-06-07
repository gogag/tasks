using System;
using System.Collections.Generic;
using System.Linq;
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

            Func<double> funcExample = () => 0;

            Delegate;
            MulticastDelegate;

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
                    double item = rand.NextInt64(long.MinValue, long.MaxValue); // чтоб побольше числа генерить
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
            
        }

        private static void SolveTask3()
        {
            
        }

        private static void SolveTask4()
        {
            
        }
    }
}
