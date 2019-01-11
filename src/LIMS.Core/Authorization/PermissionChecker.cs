using Abp.Authorization;
using LIMS.Authorization.Roles;
using LIMS.Authorization.Users;

namespace LIMS.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
