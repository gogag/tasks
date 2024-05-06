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
            string str = Console.ReadLine();
            try
            {
                int val = Parse(str);
                Console.WriteLine(val);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Invalid number")
                    throw new ApplicationException("I thought so", ex);

                throw;
            }
        }

        private static int Parse(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            
            int num;
            if (!int.TryParse(str, out num))
                throw new ArgumentException("Invalid number");

            if (num > 10)
                throw new ArgumentOutOfRangeException(nameof(str));

            return num;
        }

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
            var antiWebsite = new AntiWebsite((key, val) => Console.WriteLine($"{key}: {val}"))
            {
                Name = "asdf",
                Description = "no matter what is this, they think it is enough to just put to DB and limit its length with nvarchar(<something>)",
                Url = "trash to save, validated somewhere else",
                IpAddress = "another trash validated somewhere",
            };
            Console.WriteLine(antiWebsite); // чтобы продемонстрировать переопределение ToString
            System.Reflection.PropertyInfo[] propertyInfos = typeof(AntiWebsite).GetProperties();
        }

        // 🤦 в угоду ORM-кам
        private class AntiWebsite
        {
            private readonly Action<string, string> _outputStrategy;
            private string _description;

            public AntiWebsite(Action<string, string> outputStrategy)
            {
                _outputStrategy = outputStrategy;
            }

            public string Name { get; set; }
            public string Description
            {
                get => _description;
                internal set
                {
                    _description = value;
                }
            }
            public string Url { get; set; }
            public string IpAddress { get; set; }

            // шарп не для рисования графических интерфейсов. ToString вообще только для отладки где-то когда-то рекомендовали. Отладчик и так вам позволяет смотреть на текущие значения удобно
            public override string ToString()
            {
                _outputStrategy(nameof(Url), Url);
                _outputStrategy(nameof(Url), Url);
                _outputStrategy(nameof(Url), Url);
                _outputStrategy(nameof(Url), Url);
                return $@"
Super website: {Name}.
It takes the most expensive domain, see: {Url}.
And you know, before you read the description, here is its IP: {IpAddress}.
Now please, don't hesitate to read the whole description: {Description}";
            }
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

        private abstract class BaseClass
        {
            protected abstract int Calculate();

            public string Render()
            {
                int val = Calculate();
                return $"{val} is ok";
            }
        }

        private interface IBaseClass
        {
        }

        private interface IBaseClass1
        {
        }

        private class Inheritor : BaseClass, IBaseClass, IBaseClass1
        {
            private int _value;

            public Inheritor(int value)
            {
                _value = value;
            }

            protected override int Calculate()
            {
                return _value * _value;
            }
        }
    }
}

