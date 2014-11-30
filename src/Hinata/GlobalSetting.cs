using System;
using System.Configuration;
using System.Linq;

namespace Hinata
{
    public class GlobalSetting
    {
        /// <summary>ユーザー自身でユーザー名を指定できるかを取得します。</summary>
        public static bool AllowSetUserName { get; private set; }

        /// <summary>ユーザー登録可能なEメールアドレスのドメインを取得します。</summary>
        public static string[] EmailAddressDomains { get; private set; }

        /// <summary>Eメールアドレスのドメインでログインが制限されるかどうかを取得します。</summary>
        public static bool IsRestrictedLogin { get { return EmailAddressDomains.Any(); } }

        static GlobalSetting()
        {
            var allowSetUserName = ConfigurationManager.AppSettings["AllowSetUserName"];
            bool o;
            AllowSetUserName = bool.TryParse(allowSetUserName, out o) && o;

            var emailAddressDomainsString = ConfigurationManager.AppSettings["EmailAddressDomains"];
            EmailAddressDomains = string.IsNullOrWhiteSpace(emailAddressDomainsString)
                ? new string[0]
                : emailAddressDomainsString.Split(',');

            if ((!EmailAddressDomains.Any() || EmailAddressDomains.Count() > 1) && !AllowSetUserName)
            {
                throw new InvalidOperationException();
            }
        }
    }
}