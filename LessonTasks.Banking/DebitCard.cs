using System;

namespace LessonTasks.Banking
{
    public sealed class DebitCard
    {
        public decimal Balance { get; }
        public string CardNumber { get; }

        public DebitCard(string cardNumber, decimal balance)
        {
            if (string.IsNullOrEmpty(cardNumber))
                throw new ArgumentException($"\"{nameof(cardNumber)}\" не может быть неопределенным или пустым.", nameof(cardNumber));
            if (balance < 0)
                throw new ArgumentOutOfRangeException("balance");

            CardNumber = cardNumber;
            Balance = balance;
        }

        public static DebitCard operator +(DebitCard original, Transfer transfer)
        {
            return new DebitCard(original.CardNumber, original.Balance + transfer.Amount);
        }

        public static DebitCard operator -(DebitCard original, Transfer transfer)
        {
            return new DebitCard(original.CardNumber, original.Balance - transfer.Amount);
        }
    }
}