using System;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using Hinata.Models;
using Hinata.Repositories;
using Microsoft.AspNet.Identity;

namespace Hinata.Utilities
{
    internal static class PrincipalExtensions
    {
        public static async Task<User> GetCurrentAsync(this IPrincipal principal)
        {
            var id = principal.Identity.GetUserId();

            if (id == null) return null;

            var repository = DependencyResolver.Current.GetService<IUserRepository>();

            var user = await repository.FindByIdAsync(id);
            user.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");

            return user;
        }
    }
}