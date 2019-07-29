using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace CemuUpdateTool.Utils
{
    public static class WebUtils
    {
        private const int REQUEST_TIMEOUT_MS = 10000;

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

            return request.PerformAndGetResponse();
        }

        private static HttpWebResponse PerformAndGetResponse(this HttpWebRequest request)
        {
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
            request.Timeout = REQUEST_TIMEOUT_MS;
            return request;
        }
        
        public static string GetErrorMessageFromWebExceptionStatus(WebExceptionStatus excStatus)
        {
            switch (excStatus)
            {
                case WebExceptionStatus.NameResolutionFailure:
                    return "Name resolution failure. This can be due to absent internet connection or wrong Cemu website option.";
                case WebExceptionStatus.ConnectFailure:
                    return "Connection failure. Is your internet connection working?";
                case WebExceptionStatus.Timeout:
                    return "Request timed out. Could be a temporary server error as well as missing internet connection.";
                case WebExceptionStatus.ConnectionClosed:
                    return "The connection was unexpectedly closed by server. Retry later.";
                default:
                    return excStatus + ".";
            }
        }
    }
}
