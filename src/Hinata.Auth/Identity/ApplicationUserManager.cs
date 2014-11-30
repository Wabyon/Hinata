using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Hinata.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(ApplicationUserStore store) : base(store)
        {
        }

        public static ApplicationUserManager Create(ApplicationUserStore store)
        {
            return Create(store, new EmptyIdentityServiceService());
        }

        public static ApplicationUserManager Create(ApplicationUserStore store, IIdentityMessageService emailService)
        {
            var manager = new ApplicationUserManager(store);

            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            manager.UserLockoutEnabledByDefault = false;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            if (emailService is EmptyIdentityServiceService) return manager;

            manager.RegisterTwoFactorProvider("電子メール コード", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "セキュリティ コード",
                BodyFormat = "あなたのセキュリティ コードは {0} です。"
            });

            manager.EmailService = emailService;

            manager.UserTokenProvider =
                            new DataProtectorTokenProvider<ApplicationUser>(new MachineKeyProtectionProvider().Create("EmailConfirmation"));
            
            return manager;
        }
    }
}
