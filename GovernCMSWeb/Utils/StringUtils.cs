using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace GovernCMS.Utils
{
    public static class StringUtils
    {
        public static string CleanEmailAddr(string emailAddr)
        {
            // Guard block
            if (String.IsNullOrEmpty(emailAddr))
            {
                return null;
            }

            return emailAddr.Trim().ToLower();
        }

        /// <summary>
        /// This method will accept a date and time string and reformat it type DateTime.  
        /// </summary>
        /// <param name="dateString">Date String in the format MM/DD/YYYY. Month or day can be in format '1', '01', or '10'.</param>
        /// <param name="timeString">Time String in the format of HH:MM AM. Hour can be in format '1', '01', or '10'</param>
        /// <returns>Returns parsed DateTime format.  Returns DateTime.MinValue if parse fails.</returns>
        public static DateTime ParseDateAndTime(String dateString, String timeString)
        {
            DateTime dt;
            string dateTimeMask = "M/d/yyyyh:mm tt";
            if (dateString == null)
            {
                return DateTime.Now;
            }
            if (string.IsNullOrEmpty(timeString))
            {
                timeString = string.Empty;
                dateTimeMask = "M/d/yyyy";
            }
            DateTime.TryParseExact(dateString + timeString, 
                                   dateTimeMask,
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.None,
                                   out dt);
            return dt;
        }

        /// <summary>
        /// Create an Organization Slug
        /// </summary>
        /// <param name="organizationName">The Organization Name</param>
        /// <returns>The Organization Slug</returns>
        public static string CreateSlug(string organizationName)
        {
            // Guard block
            if (string.IsNullOrEmpty(organizationName))
            {
                return null;
            }
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            string slug = rgx.Replace(organizationName, "").ToLower().Trim().Replace(" ", "-");
            return slug;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static String EscapeHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            return input.Replace("'", "\\'");
        }

        public static string ExtractDomainFromUrl(string url)
        {
            Uri uri = new Uri(url);
            return uri.Host;
        }

        public static string AddProtocolToHost(string host)
        {
            return "http://" + host;
        }
    }
}
