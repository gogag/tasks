using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonTasks
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            bool retry = true;

            while (retry)
                try
                {
                    //retry = false;
                    //Console.WriteLine("Enter task number:");
                    //int taskNumber = int.Parse(Console.ReadLine());
                    //switch (taskNumber)
                    //{
                    //    case 1: SolveTask1(); break;
                    //    case 2: SolveTask2(); break;
                    //    case 3: SolveTask3(); break;
                    //    case 4: SolveTask4(); break;
                    //    default: Console.WriteLine("Unknown task"); break;
                    //}

                    retry = SolveTaskHandled(); // лучше одну строчку оставлять
                }
                //catch (ArgumentNullException e) //
                //{
                //    Console.WriteLine(e.Message);
                //    e.StackTrace.Contains("SolveTask4"); // 🙈
                //}
                catch (StudentException studentException) when (studentException.IsTransient)
                {
                    retry = true;
                    Console.WriteLine(studentException.Message); // логгирование
                }
                catch (Exception e)
                {
                    //if (e is ArgumentNullException) -- это не сюда, а просто как избежать копи-паста
                    //{

                    //}

                    if (e is StudentException studentException && studentException.IsTransient)
                    {
                        retry = true;
                    }

                    Console.WriteLine(e.Message); // логгирование
                    Console.WriteLine(e.StackTrace);
                }

            Console.ReadKey();
        }

        private static bool SolveTaskHandled()
        {
            bool retry = false;
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

            return retry;
        }

        private static void SolveTask1()
        {
            Assert.NotNull<object>(null); // а так в библиотеках для тестирования выглядит
            //ArgumentOutOfRangeException; // очевидно
            //ArgumentException; // после как-л. не типичная проверки
        }

        private static void SolveTask2()
        {
            var dict = new Dictionary<int, int>();
            if (!dict.ContainsKey(1))
            {
                dict.Add(1, 1);
            }

            try
            {
                dict.Add(1, 1); // одна строка в try и ожидаемый контракт
            }
            catch (ArgumentException ex)
            {
                // ooops.. я написал что-то плохое
            }
        }

        private static void SolveTask3()
        {
            try
            {
                // допустим попытались зделать что-то по указанию model
                // сетевой вызов например
            }
            catch (Exception ex)
            {
                // ooops.. я написал что-то плохое
                // что-то будет делать, но упадет

                //if (!model.CanHandle(ex)) // не доделано
                {
                    throw;    // тогда мы просто оригинальный re-throw-им без изменений
                    //throw ex; // перебивает StackTrace, тупо дополнительная работа
                    throw new ApplicationException("asdf", ex);
                }
            }
        }

        private static void SolveTask4()
        {
            //using (var obj = new UnmanagedResourceWrapper())
            //{
            //    obj.SomeMethod();
            //}

            UnmanagedResourceWrapper obj = null;
            try
            {
                obj = new UnmanagedResourceWrapper();
                obj.SomeMethod();
            }
            finally
            {
                if (obj != null)
                {
                    obj.Dispose();
                }
            }
        }

        private static void SolveTask5()
        {
            File.WriteAllText("myfile.txt", "Hello files"); // относительный путь
            Console.WriteLine(File.ReadAllText("myfile.txt"));
            //@"C:\Users\Преподаватель\Desktop\myTest"; // полный путь
            //"C:\\Users\\Преподаватель\\Desktop\\myTest";
        }

        private static class Assert
        {
            internal static void NotNull<T>(T reference) where T : class
            {
                if (reference is null)
                    throw new ArgumentNullException(nameof(reference));
            }
        }
    }
}
