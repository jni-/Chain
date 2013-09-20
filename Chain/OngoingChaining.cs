using System;

namespace Chain
{
    public class OngoingChaining<T> where T : class
    {
        private  T _currentResult;

        internal OngoingChaining(T currentResult)
        {
            _currentResult = currentResult;
        }

        public OngoingChaining<TNext> Then<TNext>(Func<T, TNext> nextFunction) where TNext : class
        {
            return _currentResult != null ? new OngoingChaining<TNext>(nextFunction(_currentResult)) : new OngoingChaining<TNext>(null);
        }

        public OngoingChaining<T> OrElse(T fallback)
        {
            _currentResult = _currentResult ?? fallback;
            return this;
        }

        public OngoingChaining<T> OrElse(Func<T> fallback)
        {
            _currentResult = _currentResult ?? fallback();
            return this;
        }

        public static implicit operator T(OngoingChaining<T> chaining)
        {
            return chaining._currentResult;
        }
    }
}