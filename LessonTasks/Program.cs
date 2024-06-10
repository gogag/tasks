using LessonTasks.Storages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
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
                case 5: SolveTask5(); break;
                default: Console.WriteLine("Unknown task"); break;
            }
        }

        private static void SolveTask1()
        {
            var arrayList = new ArrayList();
            arrayList.Add("asdf");
            arrayList.Add(1);

            var list = new List<object>();
            list.Add("asdf");
            list.Add(1); // этот - упаковка
            //Console.WriteLine("{1:1}", 1); // та же проблема

            var intList = new List<int>();
            intList.Add(1); // или этот?? - здесь не будет упаковки (boxing, где последующая unboxing - распаковка не так страшна)

            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("someHeaderName", "someValue");
            //nameValueCollection.Add("Content-Type", "application/json"); // пример на будущее
        }

        private static void SolveTask2()
        {
            //new Stack<int>().Push();
            //new Queue<>
            IEnumerable<int> array = new[] { 1, 2, 3, 34, 5 };

            //array.ToList().ToList().ToList().ToList().ToList().ToList().ToList(); // антипример
            new List<int>(array); // проверяет ICollection<int>.Count и оптимизирует вставку
        }

        private static void SolveTask3()
        {
            var uniqueValues = new HashSet<int>();
            
            uniqueValues.Add(1);
            Console.WriteLine(uniqueValues.Count); // 1
            
            uniqueValues.Add(1);
            Console.WriteLine(uniqueValues.Count); // 1

            var duplicateValues = new HashSet<int>(new TrickyEqualityComparer());
            var defaultValues = new HashSet<int>(EqualityComparer<int>.Default);

            duplicateValues.Add(1);
            duplicateValues.Add(1);

            Console.WriteLine(duplicateValues.Count); // 2
        }

        private static void SolveTask4()
        {
            int asdf = GetDefault<int>(); // параметризация шаблона обязательна
            //Func<int> arg = null; // с ограничением у GetValue на ссылочные типы не соберется
            //Func<string> arg = null; // c where T : class, new(), тоже уже не может
            //new string(); // нету конструктора без параметров
            Func<object> arg = null; // c where T : class, new(), тоже уже не может
            GetValue(arg); // тип параметра подбирается автоматом

            // демонстрация небольшой пример рефлексии и динамической компиляции шаблонных методов
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                MethodInfo methodInfo = typeof(Program)
                    .GetMethod(nameof(GetDefault), BindingFlags.NonPublic | BindingFlags.Static)
                    .GetBaseDefinition().MakeGenericMethod(types[i]);
                object defaultValue = methodInfo.Invoke(null, null);
                Console.WriteLine(defaultValue);
            }
        }

        private static void SolveTask5()
        {
            List<(string, int)> fileSizes = GenerateFiles();

            List<Storage> storages = EnterStorages();

            int storageIndex = 0;
            //foreach (KeyValuePair<string, int> item in fileSizes) // это мы побаловались со словарем, но передумали его использовать не по назначению
            //foreach ((string, int) item in fileSizes) // не так гибко
            for (int fileIndex = 0; fileIndex < fileSizes.Count; fileIndex++)
            {
                //string fileName = item.Key;
                //item.Value; // тоже со словарем баловались
                (string fileName, int fileSize) = fileSizes[fileIndex];
                Storage currentStorage = storages[storageIndex];
                while (currentStorage != null)
                {
                    if (!currentStorage.Add(fileName, fileSize))
                    {
                        storageIndex++;
                        currentStorage = storageIndex >= storages.Count
                            ? null
                            : storages[storageIndex];
                    }
                }
            }
        }

        private static List<(string FileName, int FileSize)> GenerateFiles()
        {
            //Tuple<> - история кортежей, вот то что сверху 👆
            const int fileSize = 780;
            //var files = new Dictionary<string, int> // получился анти пример использования словаря
            //{
            //    ["asdf"] = fileSize,
            //    // что если кто-то захочет захардкодить еще значение и ключ, после строки сверху
            //};

            // рандомные имена, размер файла 780 (будем в Мб), нагенерить на общую сумму 565 000 (чтоб не меньше)
            фыва;
            throw new NotImplementedException();
        }

        private static List<Storage> EnterStorages()
        {
            var storages = new List<Storage>();

            Console.WriteLine("Enter one or more storages for estimation:");
            do { } while (TryEnterStorages(storages));

            Console.WriteLine();

            return storages;
        }

        private static bool TryEnterStorages(List<Storage> storages)
        {
            Storage prototype;

            Console.WriteLine("Choose storage type (or finish):");
            string storageType = Console.ReadLine();
            switch (storageType)
            {
                case "flash":
                    prototype = EnterFlashStorage();
                    break;
                case "finish":
                    return false;
                default:
                    throw new ApplicationException(storageType);
            }

            Console.WriteLine("Enter how many storages of this type you have:");
            int storagesCount = int.Parse(Console.ReadLine());

            storages.Add(prototype);
            for (int i = 0; i < storagesCount - 1; i++)
            {
                storages.Add((Storage)prototype.Clone());
            }

            return true;
        }

        private static Storage EnterFlashStorage()
        {
            Console.WriteLine("Choose speed: usb2.0, usb3.0");

            asdf;
            return new FlashStorage();
        }

        // template <typename T>
        private static T GetDefault<T>() 
        {
            return default;
        }

        //private static T GetValue<T>(Func<T> func = null)
        private static T GetValue<T>(Func<T> func = null) where T : class, new()
        {
            //if (func != null)
            //    return func();

            //return default; // так мы пишем без "ограничений типа" contstrain-ов
            //return func?.Invoke() ?? default; // если func не был передан или вернул null, то вернуть default - т.е. тоже null???
            return func?.Invoke() ?? new T(); // если func не был передан или вернул null, то вернуть default - т.е. тоже null???
        }

        private static T GetValue2<T>(Func<T> func = null) where T : struct
        {
            //if (func != null)
            //    return func();

            //return default; // так мы пишем без "ограничений типа" contstrain-ов
            //return func?.Invoke() ?? new T(); // если func не был передан или вернул null, то вернуть default - т.е. тоже null???
            return func?.Invoke() ?? default; // если func не был передан или вернул null, то вернуть default - т.е. тоже null???
        }

        internal class TrickyEqualityComparer : IEqualityComparer<int>
        {
            public TrickyEqualityComparer() { }

            public bool Equals(int x, int y)
            {
                return false;
            }

            public int GetHashCode(int obj)
            {
                return 0;
            }
        }
    }
}
