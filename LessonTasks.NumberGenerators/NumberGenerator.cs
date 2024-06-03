using System;
using System.Collections;
using System.Collections.Generic;

namespace LessonTasks.NumberGenerators
{
    public abstract class NumberGenerator
    {
        public abstract IEnumerator<int> GetEnumerator();

        public sealed class SingleRandomNumber : NumberGenerator
        {
            private static readonly Random Random = new Random();
            public static SingleRandomNumber Instance { get; } = new SingleRandomNumber();

            private SingleRandomNumber() { }

            public override IEnumerator<int> GetEnumerator()
            {
                yield return Random.Next();
            }
        }
    }
}