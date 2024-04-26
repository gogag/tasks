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
                case 2: SolveTask2(args); break;
                case 3: SolveTask3(); break;
                case 4: SolveTask4(); break;
                default: Console.WriteLine("Unknown task"); break;
            }
        }

        private static void SolveTask1()
        {
            var arr = new int[5];
            int evenCount = 0;
            int oddCount = 0;
            var hashSet = new HashSet<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                int item = arr[i];
                if (item % 2 == 0)
                {
                    evenCount++;
                }
                else
                {
                    oddCount++;
                }

                hashSet.Add(item);
            }

            Console.WriteLine(String.Format("{0} {1}", evenCount, oddCount));
            Console.WriteLine($"{evenCount} {oddCount} {hashSet.Count}");
            Console.ReadKey();
        }

        private static void SolveTask2(string[] args)
        {
            int result = 0;

            string input = Console.ReadLine();
            string[] numbers = input.Split(',');
            int upperBoundary = ParseSingleNumberArgument(args);
            for (int i = 0; i < numbers.Length; i++)
            {
                int num = int.Parse(numbers[i]);
                if (num < upperBoundary)
                {
                    result++;
                }
            }

            Console.WriteLine($"Thank you for your attention: {result}");
            Console.ReadKey();
        }

        private static void SolveTask3()
        {
            
        }

        private static void SolveTask4()
        {

        }

        private static int ParseSingleNumberArgument(string[] args)
        {
            if (args.Length != 1)
                throw new ApplicationException("args.Length != 1");

            return int.Parse(args[0]);
        }
    }
}
