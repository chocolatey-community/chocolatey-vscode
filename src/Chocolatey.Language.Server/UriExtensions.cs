using System;
using System.Net;

namespace Chocolatey.Language.Server.Utility
{
    /// <summary>
    ///   Extensions for Uri
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        ///   Tries to validate an URL
        /// </summary>
        /// <param name="url">Uri object</param>
        public static bool IsValid(this Uri url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            //This would allow 301 and 302 to be valid as well
            request.AllowAutoRedirect = true;
            request.Timeout = 15000;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }
    }
}
