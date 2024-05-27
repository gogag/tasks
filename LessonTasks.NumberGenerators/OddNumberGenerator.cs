namespace LessonTasks.NumberGenerators
{
    public class OddNumberGenerator
    {
        private int _current;

        public OddNumberGenerator()
        {
            _current = 1;
        }

        public int Next()
        {
            int current = _current;
            _current = current + 2;
            return current;
        }
    }
}
