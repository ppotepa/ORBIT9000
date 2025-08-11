using System.Collections;
using System.Reflection;
namespace TempTools;

public static class RandomDataFiller
{
    private static readonly Random _random = new();

    public static T FillWithRandomData<T>() where T : new()
    {
        return (T)FillWithRandomData(typeof(T));
    }

    private static object FillWithRandomData(Type type)
    {
        if (type == typeof(string))
            return "Str_" + Guid.NewGuid().ToString("N")[..8];
        if (type == typeof(int))
            return _random.Next(1, 1000);
        if (type == typeof(double))
            return Math.Round(_random.NextDouble() * 100, 2);
        if (type == typeof(bool))
            return _random.Next(2) == 1;
        if (type == typeof(DateTime))
            return DateTime.Now.AddDays(_random.Next(-365, 365));
        if (type == typeof(Guid))
            return Guid.NewGuid();
        if (type.IsEnum)
        {
            var values = Enum.GetValues(type);
            return values.GetValue(_random.Next(values.Length));
        }

        // Handle arrays and generic lists
        if (typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
        {
            var elementType = type.GetGenericArguments()[0];
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType);
            int count = _random.Next(1, 5);
            for (int i = 0; i < count; i++)
                list.Add(FillWithRandomData(elementType));
            return list;
        }

        // Handle complex/nested types
        if (type.IsClass && type.GetConstructor(Type.EmptyTypes) != null)
        {
            var instance = Activator.CreateInstance(type);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p => p.CanWrite);

            foreach (var prop in properties)
            {
                var value = FillWithRandomData(prop.PropertyType);
                prop.SetValue(instance, value);
            }

            return instance;
        }

        return null; // Default fallback
    }
}
