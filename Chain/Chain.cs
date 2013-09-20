
using System;

namespace Chain
{
    public abstract class Chain
    {
        public static OngoingChaining<T> Do<T>(Func<T> startFunction) where T : class
        {
            return new OngoingChaining<T>(startFunction());
        }

        public static OngoingChaining<T> Do<T>(T startObject) where T : class
        {
            return new OngoingChaining<T>(startObject);
        }
    }
}
