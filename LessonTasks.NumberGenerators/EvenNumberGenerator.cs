using LessonTasks.NumberGenerators;

namespace LessonTasks
{
    public sealed class EvenNumberGenerator : NumberGenerator
    {
        private int _current;

        public EvenNumberGenerator()
        {
            _current = 0;
        }

        public override int Next()
        {
            int current = _current;
            _current = current + 2;
            return current;
        }
    }
}