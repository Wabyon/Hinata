using System;
using Microsoft.AspNet.Identity;

namespace Hinata.Identity
{
    public class ApplicationUser : IUser
    {
        public string Id { get; private set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public DateTime LockoutEndDateUtc { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public ApplicationUser()
            : this(Guid.NewGuid().ToString())
        {
        }

        public ApplicationUser(string id)
        {
            Id = id;
        }
    }
}
