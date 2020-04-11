using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Models
{


    public abstract class Currency
    {
        public abstract string Code();
        public abstract string DecSep();
        public abstract string IntSep();
        public abstract string Symbol();
        public override bool Equals(object obj) =>
            obj switch
            {
                Currency c => c.Code().Equals(Code()),
                _ => false
            };
        public override int GetHashCode() => Code().GetHashCode();
        
    }

    public class MXN: Currency
    {
        public override string Code() => "MXN";
        public override string DecSep() => ".";
        public override string IntSep() => ",";
        public override string Symbol() => "$";
    }
    public class USD: Currency
    {
        public override string Code() => "USD";
        public override string DecSep() => ".";
        public override string IntSep() => ",";
        public override string Symbol() => "$";
    }

    public static class CurrencyFactory
    {
        public static Currency New(string currencyCode) =>
            currencyCode switch 
            {
                "MXN" => new MXN(),
                "USD" => new USD(),
                _ => throw new Exception("Wrong currency")
            };
    }
}