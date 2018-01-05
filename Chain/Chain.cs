using System;

namespace Chain
{
    public abstract class Chain
    {
        public static OngoingChaining<T> Do<T>(Func<T> startFunction) where T : class
        {
            try
            {
                return new OngoingChaining<T>(startFunction());
            }
            catch
            {
                return new OngoingChaining<T>(null);
            }
        }

        public static OngoingChaining<T> Do<T>(T startObject) where T : class
        {
            return new OngoingChaining<T>(startObject);
        }
    }
}