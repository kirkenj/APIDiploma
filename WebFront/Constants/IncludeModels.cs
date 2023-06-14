using System.Security.Claims;
namespace WebFront.Constants;

public static class IncludeModels
{
    public static class UserIdentitiesTools
    {
        public const string IDKey = "ID";
        public static int GetUserIDClaimValue(ClaimsPrincipal User) => int.Parse(User.Claims.FirstOrDefault(u => u.Type == IDKey)?.Value ?? throw new UnauthorizedAccessException());

        public const string RoleKey = ClaimTypes.Role;
        public static string GetUserRoleClaimValue(ClaimsPrincipal User) => User.Claims.FirstOrDefault(u => u.Type == RoleKey)?.Value ?? throw new UnauthorizedAccessException();

        public const string NameKey = ClaimTypes.Name;
        public static string GetUserNameClaimValue(ClaimsPrincipal User) => User.Claims.FirstOrDefault(u => u.Type == NameKey)?.Value ?? throw new UnauthorizedAccessException();
        public static bool GetUserIsAdminClaimValue(ClaimsPrincipal? User) => User != null && IsAuthorized(User) && PolicyNavigation.OnlyAdminPolicy.RoleNames.Contains(GetUserRoleClaimValue(User));
        public static bool GetUserIsSuperAdminClaimValue(ClaimsPrincipal? User) => User != null && IsAuthorized(User) && PolicyNavigation.OnlySuperAdminPolicy.RoleNames.Contains(GetUserRoleClaimValue(User));
        public static bool IsAuthorized(ClaimsPrincipal? User) => User?.Identity?.IsAuthenticated ?? false;


    }

    public static class PolicyNavigation
    {
        public const string OnlyAdminPolicyName = "OnlyAdmin";
        public static readonly string[] OnlyadminAllowedRoles = new[] { "Admin", "SuperAdmin" };
        public static (string PolicyName, string[] RoleNames) OnlyAdminPolicy => (OnlyAdminPolicyName, OnlyadminAllowedRoles);

        public const string OnlySuperAdminPolicyName = "OnlySuperAdmin";
        public static readonly string[] OnlySuperAdmnAllowedRoles = new[] { "SuperAdmin" };
        public static (string PolicyName, string[] RoleNames) OnlySuperAdminPolicy => (OnlySuperAdminPolicyName, OnlySuperAdmnAllowedRoles);
    }

    public static class BadRequestTextFactory
    {
        public static string GetNoRightsExceptionText() => "You have no rights to do it";
        public static string GetObjectNotFoundExceptionText(string key) => $"Object not found with key {key}";
    }
}

