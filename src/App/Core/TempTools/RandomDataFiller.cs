#nullable disable
using System.Collections;
using System.Reflection;

namespace TempTools;

public static class RandomDataFiller
{
    private static readonly Random RandomGenerator = new();

    public static T FillWithRandomData<T>() where T : new()
    {
        return (T)FillWithRandomData(typeof(T));
    }

    private static object FillWithRandomData(Type type)
    {
        if (IsPrimitiveType(type, out object primitiveValue))
        {
            return primitiveValue;
        }

        if (type.IsEnum)
        {
            return GenerateRandomEnumValue(type);
        }

        if (IsCollectionType(type))
        {
            return GenerateRandomCollection(type);
        }

        if (CanInstantiateType(type))
        {
            return GenerateRandomComplexObject(type);
        }

        return null;
    }

    private static bool IsPrimitiveType(Type type, out object value)
    {
        value = null;

        if (type == typeof(string))
        {
            value = "Str_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            return true;
        }

        if (type == typeof(int))
        {
            value = RandomGenerator.Next(1, 1000);
            return true;
        }

        if (type == typeof(double))
        {
            value = Math.Round(RandomGenerator.NextDouble() * 100, 2);
            return true;
        }

        if (type == typeof(bool))
        {
            value = RandomGenerator.Next(2) == 1;
            return true;
        }

        if (type == typeof(DateTime))
        {
            value = DateTime.Now.AddDays(RandomGenerator.Next(-365, 365));
            return true;
        }

        if (type == typeof(Guid))
        {
            value = Guid.NewGuid();
            return true;
        }

        return false;
    }

    private static object GenerateRandomEnumValue(Type enumType)
    {
        Array possibleValues = Enum.GetValues(enumType);
        return possibleValues.GetValue(RandomGenerator.Next(possibleValues.Length));
    }

    private static bool IsCollectionType(Type type)
    {
        return typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType;
    }

    private static object GenerateRandomCollection(Type collectionType)
    {
        Type elementType = collectionType.GetGenericArguments()[0];
        Type listType = typeof(List<>).MakeGenericType(elementType);
        IList resultList = (IList)Activator.CreateInstance(listType);

        int itemCount = RandomGenerator.Next(1, 5);
        for (int i = 0; i < itemCount; i++)
        {
            resultList.Add(FillWithRandomData(elementType));
        }

        return resultList;
    }

    private static bool CanInstantiateType(Type type)
    {
        return type.IsClass && type.GetConstructor(Type.EmptyTypes) != null;
    }

    private static object GenerateRandomComplexObject(Type objectType)
    {
        object instance = Activator.CreateInstance(objectType);
        IEnumerable<PropertyInfo> writableProperties = objectType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.CanWrite);

        foreach (PropertyInfo property in writableProperties)
        {
            object randomValue = FillWithRandomData(property.PropertyType);
            property.SetValue(instance, randomValue);
        }

        return instance;
    }
}
