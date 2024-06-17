using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                case 5: SolveTask5(); break;
                default: Console.WriteLine("Unknown task"); break;
            }
            Console.ReadKey();
        }

        private static void SolveTask1()
        {
            string input = Console.ReadLine();
            File.WriteAllText("myfile.txt", input);
            Console.WriteLine(File.ReadAllText("myfile.txt"));
        }

        private static void SolveTask2()
        {
            using (FileStream fileStream = File.OpenRead("myfile.txt"))
            {
                int read;
                do
                {
                    read = fileStream.ReadByte();
                    Console.WriteLine(read);
                }
                while (read >= 0);
            }
        }

        private static void SolveTask3()
        {
            //FileStream file = File.OpenRead("myfile.txt");
            using (FileStream file = File.OpenRead("myfile.txt"))
            //using (var sr = new StreamReader(file, , , , leaveOpen: false))
            using (var sr = new StreamReader(file)) // мы проверили, что добираться до leaveOpen особого смысла нету
            {
                
            } // здесь Dispose у file-а будет вызван дважды
        }

        private static void SolveTask4()
        {
            using (FileStream file = File.Open(
                "myfile.bin",
                FileMode.Create,
                FileAccess.Write,
                FileShare.None))
            using (var bw = new BinaryWriter(file))
            {
                bw.Write(245);
                bw.Write(245);
                bw.Write(245);
                bw.Write(64433);
                bw.Write(64433);
                bw.Write("asdfasdf 💩");
                bw.Write("фыва");
            }
        }

        private static void SolveTask5()
        {
            Directory.CreateDirectory(@"asdf\asdf\asdf\asdf\asdf\asd\asdf");

            var di = new DirectoryInfo(@"kjl\jjkjf\fasd");
            di.Create();

            var regex = new Regex(
                @"^((?!building).)*bomb", // выражение ищет слово бомба, перед которым не встречалось слово здание
                RegexOptions.Compiled); // оптимизация для повторного использования одного и того же выражения
            for (int i = 0; i < 10; i++)
            {
                regex.Match();
            }

            Regex.Match("asdfas building asd bomb fs",
                @"^((?!building).)*building((?!building).)*bomb", // выражение ищет слово бомба, перед которым встречалось ровно одно слово здание
                RegexOptions.ECMAScript); // посмотрите https://regex101.com/
        }
    }
}
