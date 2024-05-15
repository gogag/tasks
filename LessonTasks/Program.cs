using LessonTasks.NumberGenerators;
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
                default: Console.WriteLine("Unknown task"); break;
            }

            Console.WriteLine("Press any button to exit...");
            Console.ReadKey();
        }

        private static DateTime ValueExample;
        private int _property1;

        /// <summary>
        /// Задание 1
        /// Создайте классы для генерации четных чисел,
        /// нечетных чисел, простых чисел, чисел Фибоначчи.
        /// Используйте механизмы пространств имён.
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        private static void SolveTask1()
        {
            //INumberGenerator generatorAsInterface = null;
            //NumberGeneratorExtensions.Generate(generatorAsInterface, generatedNumbers);
            //generatorAsInterface.Generate(generatedNumbers);
            NumberGenerator numberGenerator;

            // пример каррирования
            //Func<int, int, SequentialNumberGenerator> func = SequentialNumberGenerator.Create;
            //int arg1 = 1;
            //Func<int, SequentialNumberGenerator> carriedFunc = Carry(func, arg1);
            //func(1, 2);

            // пример Fluent API
            new[] { 1, 2, 3 }.Where(item => item < 2).Where(item => item > 0).ToList();

            Console.WriteLine("Select generator: 1 - even, 2 - odd, 3 - simple, 4 - fibanachi");
            switch (int.Parse(Console.ReadLine()))
            {
                case 1:
                    numberGenerator = new SequentialNumberGenerator.EvenZeroBased();
                    break;
                case 2:
                    // наш Fluent API + вырожденная реализация паттерна Builder
                    numberGenerator = SequentialNumberGenerator.From(1).IncrementBy(2).Build();
                    break;
                case 3:
                    numberGenerator = new SequentialNumberGenerator(0, 1);
                    break;
                default:
                    throw new ApplicationException($"Unknown generator {selectedGeneratorStr}");
            }
            var generatedNumbers = new int[10];
            numberGenerator.Generate(generatedNumbers);
            Console.WriteLine(generatedNumbers);
        }

        //private static Func<int, SequentialNumberGenerator> Carry(Func<int, int, SequentialNumberGenerator> func, int arg1)
        //{
        //    return arg2 => func(arg1, arg2);
        //}

        public class Poco
        {
            public int Prop1 { get; set; }
            public int Prop2 { get; set; }
        }

        // а-та-та
        //private static int GenerateNext(GeneratorType generatorType, int prevValue, int )
        //{
        //    switch (generatorType)
        //    {
        //        case GeneratorType.Even:
        //            return 0;
        //        case GeneratorType.Odd:
        //            break;
        //        case GeneratorType.Simple:
        //            break;
        //        case GeneratorType.Fibanachi:
        //            break;
        //    }
        //}

        //private enum GeneratorType
        //{
        //    Even,
        //    Odd,
        //    Simple,
        //    Fibanachi
        //}
    }
}

