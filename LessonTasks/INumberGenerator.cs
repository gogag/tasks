namespace LessonTasks
{
    public interface INumberGenerator
    {
        int AutoProperty { get; set; } // интерфейс не может требовать способ реализации свойства
        int GenerateNext();
    }

    public abstract class SomethingAbstractWithAutoProperty : INumberGenerator
    {
        private int _something;

        // Оба варианта преспокойно реализуют интерфейс
        public int AutoProperty { get { return 1; } set { _something = value; } }
        //public int AutoProperty { get; set; }
        public int GenerateNext()
        {
            throw new System.NotImplementedException();
        }
    }

    public static class NumberGeneratorExtensions
    {
        public static void Generate(this INumberGenerator generator, int[] output)
        {
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = generator.GenerateNext();
            }
        }

        private class asdf : INumberGenerator
        {
            int INumberGenerator.GenerateNext()
            {
                throw new System.NotImplementedException();
            }
        }

        public static void Do()
        {
            var e = new asdf();
            e.GenerateNext();
            var generator = (INumberGenerator)e;
            generator.GenerateNext();
            e = (asdf)generator;
            if (generator is asdf trash)
            {
                ;
            }

            var maybe = generator as asdf;
        }
    }
}

