using System;
using System.Configuration;
using System.Web;
using Hinata.Azure.Storage;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Practices.Unity;
using Hinata.Data.SqlServer;
using Hinata.Identity;
using Hinata.Identity.Repositories;
using Hinata.Markdown;
using Hinata.Repositories;
using Hinata.SendGrid;

namespace Hinata
{
    public class UnityConfig
    {
        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        public static IUnityContainer GetConfiguredContainer()
        {
            return Container.Value;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            #region repositories
            var sqldbconnectionstring = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            var storageconnectionstring = ConfigurationManager.ConnectionStrings["Storage"].ConnectionString;

            container.RegisterType<IApplicationUserRepository>(
                new PerRequestLifetimeManager(), new InjectionFactory(_ => new ApplicationUserRepository(sqldbconnectionstring)));
            container.RegisterType<IUserRepository>(new PerRequestLifetimeManager(), new InjectionFactory(c => new UserRepository(sqldbconnectionstring)));
            container.RegisterType<IDraftRepository>(new PerRequestLifetimeManager(), new InjectionFactory(c => new DraftRepository(sqldbconnectionstring)));
            container.RegisterType<IItemRepository>(new PerRequestLifetimeManager(), new InjectionFactory(c => new ItemRepository(sqldbconnectionstring)));
            container.RegisterType<IImageRepository>(new PerRequestLifetimeManager(), new InjectionFactory(c => new ImageRepository(storageconnectionstring)));

            #endregion

            #region identities
            container.RegisterType<IAuthenticationManager>(new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication));

            container.RegisterType<ApplicationUserStore>(new HierarchicalLifetimeManager());

            container.RegisterType<UserManager<ApplicationUser>, ApplicationUserManager>(
                new PerRequestLifetimeManager(), new InjectionFactory(
                    _ =>
                    {
                        var manager = ApplicationUserManager.Create(container.Resolve<ApplicationUserStore>(),
                            new EmailService
                            {
                                FromEmail = ConfigurationManager.AppSettings["sendgrid:FromEmail"],
                                FromName = ConfigurationManager.AppSettings["sendgrid:FromName"],
                                SendGridPassword = ConfigurationManager.AppSettings["sendgrid:Password"],
                                SendGridUserName = ConfigurationManager.AppSettings["sendgrid:UserName"],
                            });
                        return manager;
                    }));

            container.RegisterType<SignInManager<ApplicationUser, string>, ApplicationSignInManager>(new PerRequestLifetimeManager());
            #endregion

            #region markdown
            container.RegisterType<IJsEngine, V8JsEngine>(new PerRequestLifetimeManager());
            container.RegisterType<IMarkdownParser, MarkdownParser>(new PerRequestLifetimeManager());
            #endregion
        }
    }
}
