using System;
using System.Net;

namespace CemuUpdateTool.Utils
{
    /*
     *  This derived class is needed in order to guarantee the ability to cancel the current WebRequest when checking latest version
     *  and to provide remote files existence check functionality
     */
    public class MyWebClient : WebClient
    {
        private WebRequest myUnderlyingWebRequest;

        /*
         *  Returns the response for a HTTP HEAD request to the given address
         */
        public HttpWebResponse GetWebResponseHead(string address)
        {
            HttpWebRequest request;
            HttpWebResponse response;

            request = (HttpWebRequest) GetWebRequest(new Uri(BaseAddress + address));
            request.Method = "HEAD";
            request.Timeout = 10000;
            request.ServicePoint.MaxIdleTime = 10000;

            try
            {
                myUnderlyingWebRequest = request;
                response = (HttpWebResponse) GetWebResponse(request);
            }
            // Avoid that the method throws an exception when an error response arrives
            catch (WebException exc) when (exc.Response != null)
            {
                return (HttpWebResponse) exc.Response;
            }
            // Avoids unwanted exceptions if the task is canceled during a WebRequest
            catch (WebException exc) when (exc.Status == WebExceptionStatus.RequestCanceled)
            {
                throw new OperationCanceledException();
            }
            return response;
        }

        public new void CancelAsync()
        {
            base.CancelAsync();
            myUnderlyingWebRequest?.Abort();
        }
    }
}
