using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace CemuUpdateTool.Utils
{
    public class WebUtils
    {
        private static readonly int requestTimeoutMs = 10000;

        /*
         * Performs a simple HTTP request synchronously with the specified method.
         * This method returns whether the server response is successful or not.
         * In case the request is cancelled, an OperationCanceledException is thrown.
         */
        public static HttpWebResponse SendHttpRequest(HttpMethod method, string url, CancellationToken cancellationToken)
        {
            HttpWebRequest request = BuildWebRequest(method, url);

            cancellationToken.ThrowIfCancellationRequested();
            cancellationToken.Register(request.Abort);

            try
            {
                var response = (HttpWebResponse) request.GetResponse();
                return response;
            }
            // Avoid throwing an exception when an error response arrives
            catch (WebException exc) when (exc.Response != null)
            {
                return (HttpWebResponse) exc.Response;
            }
            catch (WebException exc) when (exc.Status == WebExceptionStatus.RequestCanceled)
            {
                throw new OperationCanceledException();
            }
        }

        private static HttpWebRequest BuildWebRequest(HttpMethod method, string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = method.ToString();
            request.Timeout = requestTimeoutMs;
            return request;
        }
    }
}
