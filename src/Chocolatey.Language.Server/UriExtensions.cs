using System;
using System.Net;

namespace Chocolatey.Language.Server
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

        /// <summary>
        ///   Tries to validate if an URL is SSL capable. 
        ///   HTTP: Will return true if the URL validates with SSL, otherwise false.
        ///   HTTPS: it returns false
        /// </summary>
        /// <param name="url">Uri object</param>
        public static bool SslCapable(this Uri url)
        {
            if (url.Scheme.Equals(Uri.UriSchemeHttps))
            {
                return false;
            }

            var uri = new UriBuilder(url);
            // Handle http: override the scheme and use the default https port
            uri.Scheme = Uri.UriSchemeHttps;
            uri.Port = -1;
            return uri.Uri.IsValid();     
        }
    }
}
