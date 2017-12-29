using System;

namespace Chain
{
    public class OngoingChaining<T> where T : class
    {
        private T _currentResult;
        private Action<Exception> _errorFallback;
        private Action<Exception> _globalErrorFallback;

        internal OngoingChaining(T currentResult)
        {
            _currentResult = currentResult;
        }

        public OngoingChaining<T> WithErrorFallback(Action<Exception> onError)
        {
            _errorFallback = onError;
            return this;
        }

        public OngoingChaining<T> WithGlobalErrorFallback(Action<Exception> onError)
        {
            _globalErrorFallback = onError;
            return this;
        }

        public OngoingChaining<TNext> Then<TNext>(Func<T, TNext> nextFunction) where TNext : class
        {
            try
            {
                var result = _currentResult != null
                    ? new OngoingChaining<TNext>(nextFunction(_currentResult))
                    : new OngoingChaining<TNext>(null);

                _errorFallback = null;
                return result;
            }
            catch (Exception ex)
            {
                _errorFallback?.Invoke(ex);
                _globalErrorFallback?.Invoke(ex);
                return null;
            }
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