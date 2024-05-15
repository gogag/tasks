namespace LessonTasks
{
    public abstract class NumberGenerator
    {
        public abstract int GenerateNext();

        public void Generate(int[] output)
        {
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = GenerateNext();
            }
        }

        public sealed class MaxValueNumberGenerator : NumberGenerator
        {
            public static MaxValueNumberGenerator Default { get; } = new MaxValueNumberGenerator();

            public override int GenerateNext()
            {
                return int.MaxValue;
            }
        }
    }
}

