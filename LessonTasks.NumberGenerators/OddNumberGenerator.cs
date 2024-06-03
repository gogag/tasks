using System.Collections.Generic;

namespace LessonTasks.NumberGenerators
{
    public sealed class OddNumberGenerator : NumberGenerator
    {
        private int _current;

        public OddNumberGenerator()
        {
            _current = 1;
        }

        public override IEnumerator<int> GetEnumerator()
        {
            while (true)
            {
                int current = _current;
                unchecked
                {
                    _current = current + 2;
                    if (_current < current)
                        yield break;
                }
                yield return current;
            }
        }
    }
}
