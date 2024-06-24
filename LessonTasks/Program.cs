using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LessonTasks.Analysing;

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
                default: Console.WriteLine("Unknown task"); break;
            }

            Console.ReadKey();
        }

        private static void SolveTask1()
        {
            Console.Write("Please input a path to a text file: ");
            string pathToText = Console.ReadLine();
            string text = File.ReadAllText(pathToText);

            IProblemAnalyzer analyzer = ConfigureAnalyzing();

            //string[] words = text.Split(' ', ',', '!');
            // тут удобно пробовать регулярные выражения: https://regex101.com/
            string[] words = Regex.Split(text, @"[^\w]"); // чтоб не перечислять
            for (int i = 0; i < words.Length; i++)
            {
                analyzer.Analyze(words[i]);
            }

            var problems = new Dictionary<string, int>();

            analyzer.CollectProblems((name, count) =>
            {
                try
                {
                    problems.Add(name, count);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Duplicate problem name:");
                    throw;
                };
            });


        }

        private static void SolveTask2()
        {
            string[] asdf = Regex.Split("ба ла лай ка!", @"[^\w]");
            Console.WriteLine($"Tokens: {string.Join(", ", asdf)}");
        }

        private static IProblemAnalyzer ConfigureAnalyzing()
        {
            var problemAnalyzers = new List<IProblemAnalyzer>()
            {
                new BadWordAnalyzer(Console.ReadLine().Split(',')),
                new CipherAnalyzer(),
            };

            //IProblemAnalyzer problemAnalyzer = ProblemAnalyzerExtensions.Compose(problemAnalyzers);
            IProblemAnalyzer problemAnalyzer = problemAnalyzers.Compose();
            //problemAnalyzers.Clear();
            return problemAnalyzer;
        }
    }
}
