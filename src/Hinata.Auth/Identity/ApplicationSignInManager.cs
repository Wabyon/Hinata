using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Hinata.Identity
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(UserManager<ApplicationUser> userManager,
            IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override async Task<SignInStatus> PasswordSignInAsync(string userNameOrEmail, string password,
            bool isPersistent, bool shouldLockout)
        {
            var user = await UserManager.FindByNameAsync(userNameOrEmail) ??
                       await UserManager.FindByEmailAsync(userNameOrEmail);

            if (user == null) return SignInStatus.Failure;

            if (await UserManager.IsLockedOutAsync(user.Id)) return SignInStatus.LockedOut;

            if (!await UserManager.CheckPasswordAsync(user, password))
            {
                if (!shouldLockout) return SignInStatus.Failure;

                await UserManager.AccessFailedAsync(user.Id);
                if (await UserManager.IsLockedOutAsync(user.Id))
                {
                    return SignInStatus.LockedOut;
                }
                return SignInStatus.Failure;
            }

            await base.SignInAsync(user, isPersistent, false);

            return SignInStatus.Success;
        }
    }
}
