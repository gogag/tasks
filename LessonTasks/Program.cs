using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonTasks.NumberGenerators;

namespace LessonTasks
{
    internal static class Program
    {
        private static void Main(string[] args)
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

            Console.ReadKey();
        }

        private static void SolveTask1()
        {
            try
            {
                SolveTask1Handled();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ooops.. Something went wrong:");
                Console.WriteLine(ex.Message);
            }
        }

        private static void SolveTask1Handled()
        {
            Console.WriteLine("Enter count of numbers to generate:");
            int countToGenerate = int.Parse(Console.ReadLine());

            Console.WriteLine("Choose one of supported number generators: odd, even, single");
            string generatorName = Console.ReadLine();

            NumberGenerator numberGenerator;

            switch (generatorName)
            {
                case "odd":
                    numberGenerator = new OddNumberGenerator();
                    break;
                case "even":
                    numberGenerator = new EvenNumberGenerator();
                    break;
                case "single":
                    numberGenerator = NumberGenerator.SingleRandomNumber.Instance;
                    break;
                default:
                    throw new ApplicationException($"Unknown generator: {generatorName}");
            }

            //foreach (int item in numberGenerator)
            //{
            //    if (countToGenerate-- > 0)
            //    {
            //        Console.WriteLine($"Generated number: {item}");
            //    }
            //}

            IEnumerator<int> enumerator = null;

            using (enumerator = numberGenerator.GetEnumerator())
            {
                bool enumerationFinished;
                while (enumerationFinished = enumerator.MoveNext())
                {
                    int item = enumerator.Current;
                    if (countToGenerate-- > 0)
                    {
                        Console.WriteLine($"Generated number: {item}");
                    }
                    else
                    {
                        break;
                    }
                }

                if (!enumerationFinished)
                {
                    Console.WriteLine("Generator has exhausted his values");
                }
            }

            Console.WriteLine("Press any key to finish...");
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
