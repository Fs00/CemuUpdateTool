using System;
using System.Net;
using System.Threading;

namespace CemuUpdateTool
{
    public static class WebUtils
    {
        /*
         *  Method that recursively finds the latest online version for a program in a given branch (e.g. 1.x, 2.5.x, x, etc.)
         *  In order for this method to work, all versions must be in the same remote folder (obviously publicly accessible)
         *  and files be named in the format [prefix][version][suffix].
         *  Parameters:
         *   - branch: the VersionNumber representing the branch you want to find the latest version of
         *   - client: the MyWebClient used for web requests. Its BaseAddress MUST contain the URL prefix inclusive of file name prefix (see above)
         *   - urlSuffix: the part of the URL which comes after the version number (see above)
         *   - maxVersionLength: the maximum branch Length that must be reached at the latest recursive call (in a few words, where the function must stop)
         *   - startingVersion: the VersionNumber you can use to start the version scanning from a version you know that exists to avoid useless scanning
         *   - cToken: the CancellationToken used to check if work has been cancelled (can be null)
         *   - currentRecursiveCall: name is self-explanatory
         */
        public static VersionNumber GetLatestRemoteVersionInBranch(VersionNumber branch, MyWebClient client, string urlSuffix, int maxVersionLength,
                                                                   VersionNumber startingVersion = null, CancellationToken? cToken = null, int currentRecursiveCall = 0)
        {
            // Check startingVersion validity
            if (startingVersion != null)
            {
                // If this is the first recursive call, make sure that the starting version exists (since it's user-editable)
                if (currentRecursiveCall == 0)
                {
                    if (!startingVersion.IsSubVersionOf(branch) || !client.RemoteFileExists(startingVersion.ToString(maxVersionLength) + urlSuffix))
                        startingVersion = null;
                }
                // Otherwise, make sure that startingVersion is still useful (e.g. if startingVersion is 1.10.3 and at the third recursive call branch is 1.11,
                // the number '3' must not be used given that we are in an other branch (1.11.x vs 1.10.x))
                else
                {
                    if (startingVersion[branch.Length-1] != branch[branch.Length-1])
                        startingVersion = null;
                }
            }

            // Set the value of the last version segment from which start the loop (that's the value of x in 1.5.x for example)
            int startingSegmentValue;
            if (startingVersion != null)
                startingSegmentValue = startingVersion[branch.Length];
            else if (branch.Length == 0)     // here we assume that the program doesn't have a 0.x branch (in our case Cemu)
                startingSegmentValue = 1;
            else
                startingSegmentValue = 0;

            // Start version scanning
            bool lastCheckedVersionExists = true;
            int iterationsCompleted = 0;
            VersionNumber lastCheckedVersion = new VersionNumber(branch);
            lastCheckedVersion.AppendSegment(startingSegmentValue);

            while (lastCheckedVersionExists)
            {
                cToken?.ThrowIfCancellationRequested();
                if (client.RemoteFileExists(lastCheckedVersion.ToString(maxVersionLength) + urlSuffix))
                    lastCheckedVersion++;
                else
                {
                    lastCheckedVersionExists = false;
                    // If no iterations have been completed up to now, it means that no versions have been found
                    if (iterationsCompleted > 0)
                        lastCheckedVersion--;
                    else
                        lastCheckedVersion = null;
                }
                iterationsCompleted++;
            }

            // Decide what to return
            if (lastCheckedVersion == null)                    // no versions have been found in this branch (probable wrong parameter/s)
                return null;
            if (lastCheckedVersion.Length < maxVersionLength)  // maximum version length has not been reached yet
                return GetLatestRemoteVersionInBranch(lastCheckedVersion, client, urlSuffix, maxVersionLength, startingVersion, cToken, currentRecursiveCall + 1);
            else                                               // finished scanning: return the latest version you found
                return lastCheckedVersion;
        }
    }

    /*
     *  MyWebClient
     *  This derived class is needed in order to guarantee the ability to cancel the current WebRequest when checking latest version
     *  and provide remote files existence check functionality
     */
    public class MyWebClient : WebClient
    {
        public WebRequest MyUnderlyingWebRequest { private set; get; }

        /*
         *  Returns the response for a HTTP HEAD request to the given address
         */
        public WebResponse GetWebResponseHead(string address)
        {
            HttpWebRequest request;
            HttpWebResponse response;

            request = (HttpWebRequest) GetWebRequest(new Uri(BaseAddress + address));
            request.Method = "HEAD";
            request.Timeout = 10000;
            request.ServicePoint.MaxIdleTime = 10000;

            try
            {
                MyUnderlyingWebRequest = request;
                response = (HttpWebResponse) GetWebResponse(request);
            }
            // Avoid that the method throws an exception when an error response arrives
            catch (WebException exc) when (exc.Response != null)
            {
                return exc.Response;
            }
            // Avoids unwanted exceptions if the task is canceled during a WebRequest
            catch (WebException exc) when (exc.Status == WebExceptionStatus.RequestCanceled)
            {
                throw new OperationCanceledException();
            }
            return response;
        }

        /*
         *  Checks if a remote file exists by checking the code of the server response
         */
        public bool RemoteFileExists(string relativeUrl)
        {
            var response = (HttpWebResponse) GetWebResponseHead(relativeUrl);
            HttpStatusCode responseCode = response.StatusCode;
            response.Close();
            return responseCode == HttpStatusCode.OK;
        }
    }
}
