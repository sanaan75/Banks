namespace Entities;

public static class Check
{
    public static void Null(object obj, Func<string> message)
    {
        if (obj == null) return;
        throw new AppException(message());
    }

    public static void NotNull(object obj, Func<string> message)
    {
        if (obj != null) return;
        throw new AppException(message());
    }

    public static void Equal<T>(T expected, T actual, Func<string> message)
    {
        if (Equals(expected, actual)) return;

        throw new AppException(message());
    }

    public static void NotEqual<T>(T expected, T actual, Func<string> message)
    {
        if (Equals(expected, actual))
            throw new AppException(message());
    }

    public static void NotEmpty(string text, Func<string> message)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new AppException(message());
    }

    public static void Given(string text, Func<string> message)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new AppException(message());
    }

    public static void Contains<T>(IEnumerable<T> sequence, T element, Func<string> message)
    {
        if (sequence == null || !sequence.Contains<T>(element))
            throw new AppException(message());
    }

    public static void DoesNotContain<T>(
        IEnumerable<T> collection,
        T element,
        Func<string> message)
    {
        if (collection == null)
            throw new AppException(message());
        if (collection.Contains<T>(element))
            throw new AppException(message());
    }

    public static void NotContains<T>(IEnumerable<T> sequence, T element, Func<string> message)
    {
        if (sequence?.Contains(element) == false) return;
        throw new AppException(message());
    }

    public static void InRange<T>(T actual, T low, T high, Func<string> message) where T : IComparable<T>
    {
        if ((object)actual != null && actual.CompareTo(low) < 0)
            throw new AppException(message());
        if ((object)actual != null && actual.CompareTo(high) > 0)
            throw new AppException(message());
    }

    public static void Any<T>(
        IEnumerable<T> collection,
        Func<T, bool> condition,
        Func<string> message)
    {
        if (!collection.Select<T, bool>(condition).Any<bool>((Func<bool, bool>)(i => i)))
            throw new AppException(message());
    }

    public static void AscendsOrEqual<T>(T a, T b, Func<string> message) where T : IComparable<T> =>
        Check.True(a.CompareTo(b) <= 0, message);

    public static void Ascends<T>(T a, T b, Func<string> message) where T : IComparable<T> =>
        Check.True(a.CompareTo(b) < 0, message);

    public static void AllUnique<T>(IList<T> collection, Func<string> message) =>
        Check.Equal<int>(collection.Count<T>(), collection.Distinct<T>().Count<T>(), message);

    public static void Positive<T>(T value, Func<string> message) where T : IComparable<T> =>
        Check.True(value.CompareTo(default(T)) > 0, message);

    public static void Negative<T>(T value, Func<string> message) where T : IComparable<T> =>
        Check.True(value.CompareTo(default(T)) < 0, message);

    public static void PositiveOrZero<T>(T value, Func<string> message) where T : IComparable<T> =>
        Check.True(value.CompareTo(default(T)) >= 0, message);


    public static void True(bool condition, Func<string> message)
    {
        if (condition) return;
        throw new AppException(message());
    }

    public static void False(bool condition, Func<string> message)
    {
        True(condition == false, message);
    }
}