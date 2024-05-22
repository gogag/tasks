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
            int? val = null;
            int? value = val + 1;
            Nullable<int>
            int valNotNull = value ?? throw new ApplicationException();
            if (!value.HasValue)
            {
                value.Value
                throw new Exception()
            }
            Console.WriteLine(value);
            Console.ReadKey();
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
