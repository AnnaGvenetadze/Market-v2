using Market.Services.Attributes;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Market.Services;
// asp.net-ში Authorization Middleware-ის მაგივრად ვინფორმსში პროქსი კლასებია.
// AuthorizationProxy<T> ფუთავს რეალურ ბიზნესსერვისს (_target).
// T არის სერვისის ინტერფეისი (მაგ. IEmployeeService).
// ყოველი მეთოდის გამოძახებისას Proxy კითხულობს [RequireRole] ატრიბუტს
// და AuthorizationService-ს გადასცემს საჭირო როლებს შესამოწმებლად.
// AuthorizationService ამოწმებს მომხმარებლის უფლებებს CurrentUserContext-ით.
// წარმატებული ავტორიზაციის შემდეგ Proxy მეთოდს იძახებს რეალურ სერვისზე.
// ProductService proxyService =
//  AuthorizationProxy<IProductService>.Wrap(someRealService, authorizationService);
public class AuthorizationProxy<T> : DispatchProxy where T : class
{
    private T? _target; // რომელ რეალურ სერვისს უნდა გადასცეს გამოძახება
    private AuthorizationService? _authorizationService; // ვის უნდა გადაამოწმებინოს მომხმარებლის როლები


    public static T Wrap(T target, AuthorizationService authorizationService)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(authorizationService);
        if (!typeof(T).IsInterface)
            throw new ArgumentException("T must be an interface.");

        var proxy = DispatchProxy.Create<T, AuthorizationProxy<T>>();
        var authorizationProxy = (AuthorizationProxy<T>)(object)proxy;
        
        authorizationProxy._target = target;
        authorizationProxy._authorizationService = authorizationService;

        return proxy;
    }


    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(targetMethod);
        if (_target == null || _authorizationService == null)
            throw new InvalidOperationException("Authorization proxy isn't initialized.");

        if (targetMethod.DeclaringType != typeof(object))
        {
            var attribute = targetMethod.GetCustomAttribute<RequireRoleAttribute>();

            if (attribute == null)
                throw new InvalidOperationException(
                    $"Authorization policy isn't defined for {targetMethod.Name}.");

            _authorizationService.RequireAnyRole(
                $"{typeof(T).Name}.{targetMethod.Name}", attribute.Roles);
            // მაგ. "IEmployeeService.ChangeRole", [Role.Cashier] -> UnauthorizedAccessException
        }

        try
        {
            return targetMethod.Invoke(_target, args); // რეალური სერვისის მეთოდის
        }
        catch (TargetInvocationException ex)
            when (ex.InnerException != null)
        {
            // თუ უბრალოდ ვისვრით აჩვენებს სროლის ბოლო ადგილის ხაზს
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw;
        }
        // თუ დისპეჩერიდან ვისვრით აჩვენებს პირველ ადგილს და სტექთრეისს:
        // ArgumentOutOfRangeException: employeeId
        //at EmployeeService.ChangeRole(...) line 15
        //at System.Reflection.MethodBaseInvoker.Invoke(...)
        //-- - End of stack trace from previous location ---
        //at AuthorizationProxy.Invoke(...) line 50
        //at GeneratedProxy.ChangeRole(...)
    }
}