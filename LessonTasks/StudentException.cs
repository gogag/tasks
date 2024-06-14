using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonTasks
{
    internal class StudentException : Exception
    {
        public bool IsTransient { get; protected set; }

        protected StudentException(string message, Exception inner = null)
            : base(message, inner)
        {
        }

        public static StudentException SimpleCase1()
        {
            return new StudentException("Simple case 1 happened");
        }

        public static StudentException SimpleCase2()
        {
            return new StudentException("Simple case 2 happened");
        }

        public static StudentException ServerConnectionError()
        {
            return new StudentException("asdf")
            {
                IsTransient = true
            };
        }

        //public static StudentException // может быть еще транзитивные ошибке
    }

    internal sealed class StudentLimitExceededException : StudentException
    {
        public int MaxStudentCount { get; private set; }

        private StudentLimitExceededException(string message, Exception inner = null) 
            : base(message, inner) { }

        public static StudentLimitExceededException ForStudentLimit(int maxCount)
        {
            return new StudentLimitExceededException("")
            {
                MaxStudentCount = maxCount
            };
        }
    }
}
