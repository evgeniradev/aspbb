using System;
using System.Security.Claims;

namespace ASPbb.Utility
{
    public class SD
    {
        public const string Role_Standard = "Standard";
        public const string Role_Admin = "Admin";

        public static string getCurrentUserId(System.Security.Claims.ClaimsPrincipal user)
        {
            var claimsIdentity = (ClaimsIdentity)user.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return claim.Value;
        }
    }
}
