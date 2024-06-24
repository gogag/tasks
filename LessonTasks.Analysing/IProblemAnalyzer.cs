using System;
using System.Collections.Generic;
using System.Linq;

namespace LessonTasks.Analysing
{
    public interface IProblemAnalyzer
    {
        void Analyze(string token);
        void CollectProblems(ProblemCollector collector);
    }

    public static class ProblemAnalyzerExtensions
    {
        public static IProblemAnalyzer Compose(this IEnumerable<IProblemAnalyzer> problemAnalyzers)
        {
            //return new Composite(problemAnalyzers.ToList());
            return new Composite(problemAnalyzers);
        }

        // никогда бы не наследовали список "в открытую"
        private class Composite : List<IProblemAnalyzer>, IProblemAnalyzer
        {

            public Composite(IEnumerable<IProblemAnalyzer> children)
                : base(children)
            {
                //new List<IProblemAnalyzer>(children);
            }

            // напоминаем про индексаторы
            //public IProblemAnalyzer this[Type type]
            //{
            //    get => this.SingleOrDefault(a => a.GetType() == type)
            //            ?? throw new IndexOutOfRangeException();
            //}

            public void Analyze(string token)
            {
                for (int i = 0; i < Count; i++)
                {
                    //this.Analyze(token); // осторожно! рекурсия
                    this[i].Analyze(token);
                    //this[typeof()].Analyze(token); // тут мы конечно так не делаем
                }
            }

            public void CollectProblems(ProblemCollector collector)
            {
                for (int i = 0; i < Count; i++)
                {
                    this[i].CollectProblems(collector);
                }
            }
        }
    }
}