using System;
using System.Threading.Tasks;
using Hinata.Identity.Repositories;
using Microsoft.AspNet.Identity;

namespace Hinata.Identity
{
    public class ApplicationUserStore:
        IUserStore<ApplicationUser>,
        IUserPasswordStore<ApplicationUser>,
        IUserEmailStore<ApplicationUser>,
        IUserLockoutStore<ApplicationUser, string>
    {
        private readonly IApplicationUserRepository _userRepository;

        public ApplicationUserStore(IApplicationUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task CreateAsync(ApplicationUser user)
        {
            return _userRepository.CreateAsync(user);
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            return _userRepository.UpdateAsync(user);

        }

        public Task DeleteAsync(ApplicationUser user)
        {
            return _userRepository.DeleteAsync(user);
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return _userRepository.FindByIdAsync(userId);
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return _userRepository.FindByNameAsync(userName);
        }

        public virtual Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            if (user == null) throw new ArgumentNullException("user");
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public virtual Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.PasswordHash);
        }

        public virtual Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.PasswordHash != null);
        }

        public virtual Task SetEmailAsync(ApplicationUser user, string email)
        {
            if (user == null) throw new ArgumentNullException("user");
            user.Email = email;
            return Task.FromResult(0);
        }

        public virtual Task<string> GetEmailAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.Email);
        }

        public virtual Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.EmailConfirmed);
        }

        public virtual Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            if (user == null) throw new ArgumentNullException("user");
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return _userRepository.FindByEmailAsync(email);
        }

        public virtual Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(new DateTimeOffset(user.LockoutEndDateUtc));
        }

        public virtual Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            if (user == null) throw new ArgumentNullException("user");
            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            return Task.FromResult(0);
        }

        public virtual Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public virtual Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public virtual Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.AccessFailedCount);
        }

        public virtual Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.LockoutEnabled);
        }

        public virtual Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            if (user == null) throw new ArgumentNullException("user");
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        public virtual void Dispose()
        {
        }
    }
}
