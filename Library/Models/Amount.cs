using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Models
{

    public static class AmountFactory
    {
        public static Amount New(string amount, Currency currency) => New(Convert.ToDecimal(amount), currency);
        public static Amount New(string amount, string currency) => New(amount, CurrencyFactory.New(currency));
        public static Amount New(decimal amount, Currency currency) => new Amount(amount, currency);
        public static Amount New(decimal amount, string currency) => new Amount(amount, CurrencyFactory.New(currency));
        public static Amount NewZero(string currency) => NewZero(CurrencyFactory.New(currency));
        public static Amount NewZero(Currency currency) => new Amount(0, currency);
    }

    public struct Amount
    {
        private readonly decimal _amount;
        private readonly Currency _currency;
        internal Amount(decimal amount, Currency currency)
        {
            _amount = amount;
            _currency = currency;
        }
        public bool IsZero() => _amount == 0;
        public Currency Currency() => _currency;
        public decimal ToDecimal() => _amount;
        public override string ToString() => _amount.ToString();

        public static Amount operator +(Amount a1, Amount a2) => VerifyCurrency<Amount>(a1, a2, new Amount(a1._amount + a2._amount, a1._currency));
        public static Amount operator -(Amount a1, Amount a2)
        {
            VerifyCurrency<bool>(a1, a2, true);
            if (a1._amount < a2._amount)
            {
                throw new Exception("Amount can't be negative");
            }
            return new Amount(a1._amount - a2._amount, a1._currency);
        }
        public static bool operator >(Amount a1, Amount a2) => VerifyCurrency<bool>(a1, a2, a1._amount > a2._amount);
        public static bool operator <(Amount a1, Amount a2) => VerifyCurrency<bool>(a1, a2, a1._amount < a2._amount);
        public static bool operator >=(Amount a1, Amount a2) => VerifyCurrency<bool>(a1, a2, a1._amount >= a2._amount);
        public static bool operator <=(Amount a1, Amount a2) => VerifyCurrency<bool>(a1, a2, a1._amount <= a2._amount);
        public static bool operator ==(Amount a1, Amount a2) => VerifyCurrency<bool>(a1, a2, a1._amount == a2._amount);
        public static bool operator !=(Amount a1, Amount a2) => VerifyCurrency<bool>(a1, a2, a1._amount != a2._amount);

        

        private static T VerifyCurrency<T>(Amount a1, Amount a2, T t)
        {
            if (a1._currency != a2._currency)
            {
                throw new Exception("invalid currency");
            }
            return t;
        }
    }


}