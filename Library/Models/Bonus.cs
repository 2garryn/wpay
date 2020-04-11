using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Models
{
    public static class BonusFactory
    {
        public static Bonus NewZero() => New(0L);
        public static Bonus New(long b) => new Bonus(b);
    }
        
    public class Bonus
    {
        private long _bonus;
        internal Bonus(long bonus) => _bonus = bonus;
        public bool IsNegative() => _bonus < 0;
        public bool IsPositive() => _bonus > 0;
        public bool IsZero() => _bonus == 0;
        public long ToLong() => _bonus;
    }

    
}