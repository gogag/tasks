using System;

namespace LessonTasks.Banking
{
    public sealed class Transfer
    {
        private const int MaxDecimals = 5;

        public decimal Amount { get; }
     
        public Transfer(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException("amount");
            if (Math.Round(amount, MaxDecimals) != amount)
                throw new ArgumentOutOfRangeException("amount");

            Amount = amount;
        }
    }
}