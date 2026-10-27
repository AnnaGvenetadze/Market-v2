using Market.Services.Attributes;
using Market.Services.Enums;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Market.Services;
// asp.net-ში Authorization Middleware-ის მაგივრად ვინფორმსში პროქსი კლასია.
// AuthorizationProxy<T> ფუთავს რეალურ ბიზნესსერვისს (მაგ. EmployeeService)

/// IEmployeeService proxyService =
///  AuthorizationProxy<IEmployeeService>.Wrap(employeeService, authorizationService);
public class AuthorizationProxy<T> : DispatchProxy where T : class
{
    private T? _target; // რომელ რეალურ სერვისს უნდა გადასცეს გამოძახება
    private AuthorizationService? _authorizationService; // ვის უნდა გადაამოწმებინოს მომხმარებლის როლები

    // სამიზნე სერვისი და შუალედური სერვისი იფუთება პროქსი ობიექტად.
    // სანამ სერვისის რეალურ მეთოდს გამოიძახებს აიძულებს ჯერ სხვა ადგილას შევიდეს
    public static T Wrap(T target, AuthorizationService authorizationService)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(authorizationService);
        if (!typeof(T).IsInterface)
            throw new ArgumentException("T must be an interface.");
        // რანთაიმზე შექმნის შესაბამის პროქსი ობიექტს
        var serviceProxy = DispatchProxy.Create<T, AuthorizationProxy<T>>();
        var authorizationProxy = (AuthorizationProxy<T>)(object)serviceProxy;

        authorizationProxy._target = target;
        authorizationProxy._authorizationService = authorizationService;

        return serviceProxy;
    }


    // სამიზნე მეთოდს ყოველი გამოძახებისას Proxy კითხულობს [RequireRole] ატრიბუტს
    // და AuthorizationService-ს გადასცემს საჭირო როლებს შესამოწმებლად.
    // გამოიძახებს ბილთინ Invoke()-ს რეალური მეთოდის გამოძახებისთვის.
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(targetMethod);
        if (_target == null || _authorizationService == null)
            throw new InvalidOperationException("Authorization proxy isn't initialized.");

        if (targetMethod.DeclaringType != typeof(object))
        {
            string methodName = targetMethod.Name;
            // მეთოდის მეტამონაცემების (მაგ. IEmployeeService.ChangeRole(string))
            // მიხედვით ვამოწმებთ ქეშს. თუ ჩანაწერი არ არსებობს,
            // ვკითხულობთ ატრიბუტს და ვინახავთ საჭირო როლებს.
            Role[] roles =
                AuthorizationAttributeCache.Roles.GetOrAdd(targetMethod, GetRequiredRoles);

            if (roles.Length > 0)
            {
                _authorizationService.RequireAnyRole(
                $"{typeof(T).Name}.{methodName}", roles);
                // მაგ. "IEmployeeService.ChangeRole", [Role.Cashier] -> UnauthorizedAccessExcepti
            }
            else // არაავთენთიფიცირებულს არ გაეხსნება უატრიბუტო მეთოდი
            {
                _authorizationService.ThrowIfNotAuthenticated(methodName);
            }
            // უატრიბუტო მეთოდი გაეხსენება ნებისმიერი როლის ავთენთიფიცირებულ
            // მომხმარებელს, Invoke()-ით გამოიძახება ისიც.

            // AuthService-ს არ უნდა და მსგავსი სერვისი არც გაივლის AuthorizationService-ში
            // რადგან პროქსის Wrap()-ს არ გადავცემ
        }

        try
        {
            return targetMethod.Invoke(_target, args); // რეალური სერვისის მეთოდის გამოძახება
        }
        catch (TargetInvocationException ex)
            when (ex.InnerException != null)
        {
            // თუ ExceptionDispatchInfo ვისვრით აჩვენებს სროლის
            // თავდაპირველ ადგილს და სტექთრეისს. ამის გარეშე შეცვლის სროლის ხაზს.
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw;
        }
    }


    // GetOrAdd გაუშვებს რეფლექციას მხოლოდ მეთოდის პირველივე გამოძახებისას.
    private static Role[] GetRequiredRoles(MethodInfo method)
    {
        var attribute = method.GetCustomAttribute<RequireRoleAttribute>();

        if (attribute == null)
            return Array.Empty<Role>();

        return attribute.Roles.ToArray();
    }
}

// საერთო ქეში ყველა AuthorizationProxy<T>-ისთვის.
// მეთოდების ატრიბუტების განმეორებით წაკითხვას თავიდან გვარიდებს
// და სხვადასხვა Proxy-ს ერთი და იმავე MethodInfo-ს შედეგის გაზიარების საშუალებას აძლევს.
// file - მხოლოდ მიმდინარე ფაილში ხელმისაწვდომი
file static class AuthorizationAttributeCache  
{
    public static readonly ConcurrentDictionary<MethodInfo, Role[]> Roles = new();
}