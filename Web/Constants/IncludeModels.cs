using System.Security.Claims;
namespace Web.Constants;

public static class IncludeModels
{
    public static class UserIdentitiesTools
    {
        public const string IDKey = "ID";
        public static int GetUserIDClaimValue(ClaimsPrincipal User) => int.Parse(User.Claims.First(u => u.Type == IDKey).Value);
    
        public const string RoleKey = ClaimTypes.Role;
        public static string GetUserRoleClaimValue(ClaimsPrincipal User) => User.Claims.First(u => u.Type == RoleKey).Value;
        
        public const string NameKey = ClaimTypes.Name;
        public static string GetUserNameClaimValue(ClaimsPrincipal User) => User.Claims.First(u => u.Type == NameKey).Value;
        
        public const string IsConfirmedKey = "IsConfirmed";
        public static string GetUserIsConfirmedClaimValue(ClaimsPrincipal User) => User.Claims.First(u => u.Type == IsConfirmedKey).Value;
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
}

