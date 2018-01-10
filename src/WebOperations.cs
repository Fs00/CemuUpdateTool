using System;
using System.Net;

namespace CemuUpdateTool
{
    /*
     *  This derived class is needed in order to guarantee the possibility to cancel the current WebRequest when checking latest version
     */
    public class MyWebClient : WebClient
    {
        /*
         *  Returns the response for a HTTP HEAD request to the given address
         */
        public WebResponse GetWebResponseHead(string address)
        {
            HttpWebRequest request;
            HttpWebResponse response;

            request = (HttpWebRequest) GetWebRequest(new Uri(BaseAddress + address));
            request.Method = "HEAD";
            request.Timeout = 30000;

            // This will avoid that the method throws an exception when an error response arrives
            try
            {
                response = (HttpWebResponse) GetWebResponse(request);
            }
            catch (WebException exc) when (exc.Response != null)
            {
                return exc.Response;
            }
            return response;
        }
    }

    public static class WebOperations
    {
        /*
         *  Checks if a remote file exists checking the code of the server response
         */
        public static bool CheckIfRemoteFileExists(MyWebClient client, string relativeUrl)
        {
            var response = (HttpWebResponse) client.GetWebResponseHead(relativeUrl);
            if (response.StatusCode == HttpStatusCode.OK)
                return true;
            else
                return false;
        }
    }
}
