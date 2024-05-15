using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonTasks.NumberGenerators
{
    internal class SequentialNumberGenerator : NumberGenerator
    {
        private readonly int _increment;
        private int _prevNum;

        private SequentialNumberGenerator(int prevNum, int increment)
        {
            _prevNum = prevNum;
            _increment = increment;
        }

        public static SequentialNumberGenerator Create(int prevNum, int increment)
        {
            return new SequentialNumberGenerator(prevNum, increment);
        }

        public static Builder From(int prevNum)
        {
            return new Builder(prevNum);
        }

        public override int GenerateNext()
        {
            return _prevNum + _increment;
        }

        public class EvenZeroBased : SequentialNumberGenerator
        {
            public EvenZeroBased() : base(0, 2) { }
        }

        public class Builder
        {
            private readonly int _prevNum;
            private int? _increment;

            public Builder(int prevNum)
            {
                _prevNum = prevNum;
            }

            public Builder IncrementBy(int increment)
            {
                if (_increment.HasValue)
                    throw new ApplicationException("_increment.HasValue");

                _increment = increment;
                return this;
            }

            public SequentialNumberGenerator Build()
            {
                int increment = _increment ?? throw new ApplicationException("!_increment.HasValue");
                return new SequentialNumberGenerator(_prevNum, increment);
            }
        }
    }
}
