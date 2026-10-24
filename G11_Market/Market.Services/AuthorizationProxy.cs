using System.Reflection;
using System.Runtime.ExceptionServices;
using Market.Services.Attributes;

namespace Market.Services;

public class AuthorizationProxy<T> : DispatchProxy where T : class
{
    private T? _target; // რომელ რეალურ სერვისს უნდა გადასცეს გამოძახება
    private AuthorizationService? _authorizationService; // ვის უნდა გადაამოწმებინოს მომხმარებლის როლები

    public static T Wrap(
        T target,
        AuthorizationService authorizationService)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(authorizationService);

        if (!typeof(T).IsInterface)
            throw new ArgumentException(
                "T must be an interface.");

        var proxy = DispatchProxy.Create<T, AuthorizationProxy<T>>();

        var authorizationProxy = (AuthorizationProxy<T>)(object)proxy;

        authorizationProxy._target = target;
        authorizationProxy._authorizationService = authorizationService;

        return proxy;
    }

    protected override object? Invoke(
        MethodInfo? targetMethod,
        object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(targetMethod);

        if (_target == null || _authorizationService == null)
            throw new InvalidOperationException(
                "Authorization proxy is not initialized.");

        if (targetMethod.DeclaringType != typeof(object))
        {
            var attribute = targetMethod
                .GetCustomAttribute<RequireRoleAttribute>();

            if (attribute == null)
                throw new UnauthorizedAccessException(
                    "Authorization policy is not defined.");

            _authorizationService.RequireAnyRole(
                $"{typeof(T).Name}.{targetMethod.Name}",
                attribute.Roles);
        }

        try
        {
            return targetMethod.Invoke(_target, args);
        }
        catch (TargetInvocationException ex)
            when (ex.InnerException != null)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw;
        }
    }
}
