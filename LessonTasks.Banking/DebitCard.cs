using System;
using System.Collections.Generic;

namespace LessonTasks.Banking
{
    public sealed class DebitCard : IEquatable<DebitCard>, IComparable<DebitCard>
    {
        public decimal Balance { get; }
        public string CardNumber { get; }
        public string Cvc { get; private set; }

        public DebitCard(string cardNumber, decimal balance)
        {
            if (string.IsNullOrEmpty(cardNumber))
                throw new ArgumentException($"\"{nameof(cardNumber)}\" не может быть неопределенным или пустым.", nameof(cardNumber));
            if (balance < 0)
                throw new ArgumentOutOfRangeException("balance");

            CardNumber = cardNumber;
            Balance = balance;
        }

        public DebitCard WithSecret(string cvc)
        {
            if (cvc is null)
                throw new ArgumentNullException(nameof(cvc));
            if (cvc.Length != 3 || !int.TryParse(cvc, out _))
                throw new ArgumentException("CVC is not a valid 3 digit number");

            return new DebitCard(CardNumber, Balance) { Cvc = cvc };
        }

        public override bool Equals(object obj)
        {
            return obj is DebitCard card &&
                   Balance == card.Balance &&
                   CardNumber == card.CardNumber &&
                   Cvc == card.Cvc;
        }

        public override int GetHashCode()
        {
            int hashCode = 2052914502;
            hashCode = hashCode * -1521134295 + Balance.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CardNumber);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Cvc);
            return hashCode;
        }

        public static DebitCard operator +(DebitCard original, Transfer transfer)
        {
            return new DebitCard(original.CardNumber, original.Balance + transfer.Amount);
        }

        public static DebitCard operator -(DebitCard original, Transfer transfer)
        {
            return new DebitCard(original.CardNumber, original.Balance - transfer.Amount);
        }

        public static bool operator >(DebitCard a, DebitCard b)
        {
            return a.Balance > b.Balance;
        }

        public static bool operator <(DebitCard a, DebitCard b)
        {
            return a.Balance < b.Balance;
        }

        public static bool operator ==(DebitCard a, DebitCard b)
        {
            return a.Cvc == b.Cvc;
        }

        public static bool operator !=(DebitCard a, DebitCard b)
        {
            return a.Cvc != b.Cvc;
        }
    }
}