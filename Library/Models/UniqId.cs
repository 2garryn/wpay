using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Models
{

    public class UniqId
    {
        public UniqId(Guid uid) => Value = uid;

        public Guid Value { get; }
        public static UniqId New()
        {
            return new UniqId(Guid.NewGuid());
        }
        public static bool operator ==(UniqId b1, UniqId b2)
        {
            if ((object)b1 == null)
                return (object)b2 == null;

            return b1.Equals(b2);
        }
        public static bool operator !=(UniqId b1, UniqId b2)
        {
            return !(b1 == b2);
        }
        public override int GetHashCode() => Value.GetHashCode();
        public override bool Equals(object obj) =>
            obj switch
            {
                UniqId c => c.Value.Equals(Value),
                _ => false
            };
    }

}