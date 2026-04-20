using System.Reflection;
using System.Runtime.CompilerServices;
using MediatR;
using Moq;
using StudyHub.Core.Users.Commands;

namespace StudyHub.UnitTests.Handlers.Architecture;

public class AllCoreHandlersConstructionTests
{
    public static IEnumerable<object[]> HandlerTypes()
    {
        // Arrange
        var assembly = typeof(RegisterUserCommandHandler).Assembly;

        // Act
        var handlers = assembly
            .GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false, IsPublic: true })
            .Where(type => type.Name.EndsWith("Handler", StringComparison.Ordinal))
            .Where(ImplementsMediatorHandlerContract)
            .OrderBy(type => type.FullName)
            .Select(type => new object[] { type });

        // Assert
        return handlers;
    }

    [Theory]
    [MemberData(nameof(HandlerTypes))]
    public void Constructor_ShouldCreateInstance_ForEachCoreHandler(Type handlerType)
    {
        // Arrange
        var constructor = handlerType
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderByDescending(item => item.GetParameters().Length)
            .First();

        var arguments = constructor
            .GetParameters()
            .Select(parameter => CreateArgument(parameter.ParameterType))
            .ToArray();

        // Act
        var instance = constructor.Invoke(arguments);

        // Assert
        Assert.NotNull(instance);
        Assert.IsType(handlerType, instance);
    }

    private static bool ImplementsMediatorHandlerContract(Type type)
    {
        return type.GetInterfaces().Any(@interface =>
            @interface.IsGenericType &&
            (@interface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
             @interface.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
             @interface.GetGenericTypeDefinition() == typeof(INotificationHandler<>)));
    }

    private static object CreateArgument(Type parameterType)
    {
        if (parameterType == typeof(string))
        {
            return string.Empty;
        }

        if (parameterType.IsValueType)
        {
            return Activator.CreateInstance(parameterType)!;
        }

        if (parameterType.IsInterface || parameterType.IsAbstract)
        {
            var mockType = typeof(Mock<>).MakeGenericType(parameterType);
            var mock = Activator.CreateInstance(mockType)!;
            var objectProperty = mockType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Single(property => property.Name == "Object" && property.PropertyType == parameterType);
            return objectProperty.GetValue(mock)!;
        }

        if (parameterType.GetConstructor(Type.EmptyTypes) != null)
        {
            return Activator.CreateInstance(parameterType)!;
        }

        return RuntimeHelpers.GetUninitializedObject(parameterType);
    }
}
