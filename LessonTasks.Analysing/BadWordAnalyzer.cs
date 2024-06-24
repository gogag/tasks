using System.Collections.Generic;
using System.Linq;

namespace LessonTasks.Analysing
{
    public class BadWordAnalyzer : IProblemAnalyzer
    {
        private readonly Dictionary<string, int> _badWords;

        public BadWordAnalyzer(IEnumerable<string> badWords)
        {
            // зато цикл выигрывает по кол-ву объектов, не нужно делать мусорные делегаты
            //_badWords = new Dictionary<string, int>();
            //foreach (string badWord in badWords)
            //{
            //    if (badWord == null) throw new ApplicationException(badWord == null);
            //    _badWords.Add(badWord, 0);
            //}
            this._badWords = badWords.ToDictionary(badWord => badWord, badWord => 0); // Dictionary не допускает ключей null
        }

        public void Analyze(string token)
        {
            фыва;
            throw new System.NotImplementedException();
        }

        public void CollectProblems(ProblemCollector collector)
        {
            foreach (KeyValuePair<string, int> kvp in _badWords)
            {
                collector(kvp.Key, kvp.Value);
            }
        }
    }
}