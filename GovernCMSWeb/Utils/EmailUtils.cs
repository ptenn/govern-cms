using System;
using System.Linq;
using System.Net.Mail;

namespace GovernCMS.Utils
{
    public static class EmailUtils
    {
        private static readonly String[] commonHostProviders =
        {
            "outlook.com",
            "hotmail.com",
            "live.com",
            "gmail.com",
            "yahoo.com",
            "ymail.com",
            "inbox.com",
            "icloud.com",
            "aol.com",

            // Mail.com variants
            "mail.com",
            "email.com",
            "usa.com",
            "myself.com",
            "consultant.com",
            "post.com",
            "europe.com",
            "asia.com",
            "iname.com",
            "writeme.com",
            "dr.com",
            "engineer.com",
            "cheerful.com",
            "accountant.com",
            "techie.com",
            "linuxmail.org",
            "uymail.com",
            "contractor.net"
        };

        public static Boolean IsEmailHostCommonProvider(string emailHost)
        {
            // Guard block
            if (string.IsNullOrEmpty(emailHost))
            {
                return false;
            }
            return commonHostProviders.ToList().Contains(emailHost.Trim().ToLower());
        }

        public static string GetDomainFromEmailAddr(string emailAddr)
        {
            MailAddress addr = new MailAddress(emailAddr);
            string domain = addr.Host;
            return domain;
        }
    }
}
