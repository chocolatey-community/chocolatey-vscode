using System;
using System.Net;

namespace Chocolatey.Language.Server.Utility
{

    public class Utility
    {
	    
        /// <summary>
        ///   Tries to validate an URL
        /// </summary>
        /// <param name="url">Uri object</param>
        public static bool url_is_valid(Uri url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Timeout = 15000;
            //This would allow 301 and 302 to be valid as well
            request.AllowAutoRedirect = true;
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