using System.Collections.Immutable;
using System.Reflection;

namespace EventSourcing.Internal.Extensions;

[ExcludeFromCodeCoverage]
internal static class ReflectionExtensions
{
    #if NETSTANDARD2_1
    public static bool IsAssignableTo(this Type? from,
        Type? to) => from == to || (to?.IsAssignableFrom(from) ?? false);
    #endif
    
    private static readonly Dictionary<Type, IReadOnlyCollection<Type>> derivativeTypes = new();

    public static IReadOnlyCollection<Type> GetAllDerivativeTypes(this Type type,
        Type derivativeType)
    {
        if (derivativeTypes.TryGetValue(type, out var types))
            return types;

        var derivatives = GetAllDerivativeTypesInternal(type, derivativeType).ToImmutableHashSet();
        derivativeTypes[type] = derivatives;

        return derivatives;
    }

    public static IEnumerable<Type> GetAllDerivativeTypesInternal(Type type,
        Type derivativeType)
    {
        if (!type.IsAssignableTo(derivativeType))
            yield break;

        yield return type;

        var interfaces = type.GetInterfaces();

        foreach (var @interface in interfaces)
            if (@interface.IsAssignableTo(derivativeType))
                yield return @interface;

        var baseType = type.BaseType;
        while (baseType is not null && baseType.IsAssignableTo(derivativeType))
        {
            yield return baseType;

            baseType = baseType.BaseType;
        }
    }

    public static Type SwapGenericArguments(this Type? type, params Type[] typeGenericArguments)
    {
        if (type is null || !type.IsGenericType) throw new InvalidOperationException("Must be a generic type.");

        var genericTypeDefinition = type.IsGenericTypeDefinition ? type : type.GetGenericTypeDefinition();

        var swappedType = genericTypeDefinition.MakeGenericType(typeGenericArguments);

        return swappedType;
    }

    public static MethodInfo? SwapTypeGenericArguments(this MethodInfo method, params Type[] typeGenericArguments)
    {
        var type = method.DeclaringType;

        var swappedType = type.SwapGenericArguments(typeGenericArguments);

        var methodParameters = method.GetParameters().Select(p => p.ParameterType).ToArray();

        var swappedMethod = swappedType.GetRuntimeMethod(method.Name, methodParameters);

        return swappedMethod;
    }
}