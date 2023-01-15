using Entities.Helpers.TextHelper;

namespace UseCases
{
    public static class Check
    {
        public static void Equal<T>(T expected, T actual, Func<string> message)
        {
            if (!object.Equals((object)expected, (object)actual))
                throw new AppException(message());
        }

        public static void NotEqual<T>(T expected, T actual, Func<string> message)
        {
            if (object.Equals((object)expected, (object)actual))
                throw new AppException(message());
        }

        public static void True(bool condition, Func<string> message)
        {
            if (condition == true)
                return;

            throw new AppException(message());
        }

        public static void False(bool condition, Func<string> message)
        {
            if (condition == false)
                return;

            throw new AppException(message());
        }

        public static void Null(object obj, Func<string> message)
        {
            if (obj != null)
                throw new AppException(message());
        }

        public static void NotNull(object obj, Func<string> message)
        {
            if (obj == null)
                throw new AppException(message());
        }

    }
    
    public class AppException : Exception
    {
        public AppException(string message)
            : base(message != null ? message.ApplyGlossary() : (string)null)
        {
        }
    }
}
