using LessonTasks.NumberGenerators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            int somethingInt = 1;
            var smth = (Something)somethingInt;
            Something smth1 = somethingInt;
            var smth2 = Something.From(somethingInt);

            var smthc = new SomethingClass();
            DoSomething(smthc);

            var smthSet = new HashSet<Something>(EqualityComparer<Something>.Default);
            smthSet.Add(new Something { Value = 1 }); // returns true
            smthSet.Add(new Something { Value = 1 }); // returns false

            var virginSet = new HashSet<object>();
            virginSet.Add(new object()); // returns true
            virginSet.Add(new object()); // returns true

            IQueryable<AnemicWebsite> websitesQueriable = null;
            IEnumerable<AnemicWebsite> websitesEnumberable = null;

            Expression<Func<AnemicWebsite, bool>> expression = w => w.SiteName == "academytop.com";
            new SomethingExpressionVisitor().Visit(expression);
            websitesQueriable.Where(expression).ToList();
            websitesEnumberable.Where(w => w.SiteName == "academytop.com");

            int id = default;
            string siteName = default;
            string path = default;
            string description = default;
            string ipAddress = default;
            object obj = new AnemicWebsite(id, description, path, siteName, ipAddress);
            //public AnemicWebsite(int id, string siteName, string path, string description, string ipAddress)
            Console.WriteLine(obj);

            int val = 100;
            UglyMethod(ref val);
            Console.WriteLine(val); // 10

            UglyMethod(ref obj);
            Console.WriteLine(obj); // ha ha
        }

        private static void UglyMethod(ref int arg) // int*
        {
            arg = 10;
        }

        private static void UglyMethod(ref object arg) // object**
        {
            arg.ToString();
            arg = "ha ha";
        }

        private class SomethingExpressionVisitor : ExpressionVisitor
        {
            public override Expression Visit(Expression node)
            {
                if (node is LabelExpression)
                {

                }
                else if (node is BinaryExpression binary)
                {
                    binary.Left;binary.Right
                }
                return base.Visit(node);
            }
        }

        private static void DoSomething(SomethingClass smthc)
        {
            if (smthc is null)
                throw new ArgumentNullException(nameof(smthc));

            ////if (smthc == null)
            //if (ReferenceEquals(smthc, null))
            //if (smthc is null)
            //        throw new ArgumentNullException("ReferenceEquals(smthc, null)");

            //smthc.Equals // throws NullReferenceException
        }

        private struct Something
        {
            public int AnemicWebsiteId { get; set; }
            public int Value { get; set; }
            public int ValueSeparated { get; set; }

            public static Something From(int value, int valueSeparated)
            {

            }

            public static explicit operator Something(int value)
            {
                return new Something { Value = value };
            }

            public override bool Equals(object obj)
            {
                var other = (Something)obj;
                if (Value != other.Value)
                    return false;
                //fadf
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private class SomethingClass : IEquatable<SomethingClass>
        {
            public bool Equals(SomethingClass obj)
            {
                return false;
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public static bool operator ==(SomethingClass dfd)
            {
                return false;
            }

            public static bool operator !=(SomethingClass dfd)
            {
                return true;
            }
        }

        private class SomethingCollectionProxy : IReadOnlyList<Something>
        {
            private readonly IReadOnlyList<Something> _somethings;

            public SomethingCollectionProxy(IReadOnlyList<Something> somethings)
            {
                if (somethings is null)
                    throw new ArgumentNullException(nameof(somethings));

                _somethings = somethings;
            }

            public Something this[int index]
            {
                get
                {


                    //throw new IndexOutOfRangeException();
                    return _somethings[index];
                }
            }

            public Something this[string index]
            {
                get
                {


                    //throw new IndexOutOfRangeException();
                    return _somethings[index];
                }
            }

            public int Count => _somethings.Count;

            public IEnumerator<Something> GetEnumerator()
            {
                return _somethings.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator<Something>)this.GetEnumerator();
            }
        }

        // anemic model example. Из урока 3.
        private class AnemicWebsite
        {
            public int Id { get; set; }
            [Column("SITE_NAME")]
            public string SiteName { get; private set; }
            public string Path { get; private set; }
            public string Description { get; private set; }
            public string IpAddress { get; }
            public string IPAddress { get; private set; }
            public virtual IEnumerable<Something> Somethings { get; private set; }

            public AnemicWebsite(int id, string siteName, string path, string description, string ipAddress)
            {
                Id = id;
                SiteName = siteName;
                Path = path;
                Description = description;
                IpAddress = ipAddress;
            }

            public override string ToString()
            {
                return $"SiteName: {SiteName}";
            }
        }
    }
}

