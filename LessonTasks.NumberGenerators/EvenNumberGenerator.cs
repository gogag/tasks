using LessonTasks.NumberGenerators;
using System;
using System.Collections.Generic;

namespace LessonTasks
{
    public sealed class EvenNumberGenerator : NumberGenerator
    {
        private int _current;

        public EvenNumberGenerator()
        {
            _current = 0;
        }

        public override IEnumerator<int> GetEnumerator()
        {
            //return (new List<int> { 4, 5, 6 }).GetEnumerator();
            //yield return 4;
            //yield return 5;
            //yield return 6;

            while (true)
            {
                int current = _current;
                try
                {
                    checked // на самом деле по умолчанию и так checked
                    {
                        _current = current + 2;
                    }
                }
                catch (OverflowException)
                {
                    yield break;
                }

                yield return current;
            }
        }
    }
}