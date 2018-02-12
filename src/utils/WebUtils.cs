using System;
using System.Net;

namespace CemuUpdateTool
{
    public static class WebUtils
    {
        /*
         *  Checks if a remote file exists checking the code of the server response
         */
        public static bool RemoteFileExists(string relativeUrl, MyWebClient client)
        {
            var response = (HttpWebResponse) client.GetWebResponseHead(relativeUrl);
            HttpStatusCode responseCode = response.StatusCode;
            response.Close();
            if (responseCode == HttpStatusCode.OK)
                return true;
            else
                return false;
        }

        /*
         *  Method that recursively finds the latest online version for a program in a given branch (e.g. 1.x, 2.5.x, x, etc.)
         *  In order for this method to work, all versions must be in the same remote folder (obviously publicly accessible)
         *  and files be named in the format [prefix][version][suffix].
         *  Parameters:
         *   - branch: the VersionNumber representing the branch you want to find the latest version of
         *   - client: the MyWebClient used for web requests. Its BaseAddress MUST contain the URL prefix inclusive of file name prefix (see above)
         *   - urlSuffix: the part of the URL which comes after the version number (see above)
         *   - maxDepth: the maximum branch' Depth that must be reached at the latest recursive call (in a few words, where the function must stop)
         *   - startingVersion: the VersionNumber you can use to start the version scanning from a version you know that exists
         *   - worker: the Worker instance used to check if work has been cancelled
         *   - currentRecursiveCall: name is self-explanatory
         */
        public static VersionNumber GetLatestRemoteVersionInBranch(VersionNumber branch, MyWebClient client, string urlSuffix, int maxDepth,
                                                                   VersionNumber startingVersion, Worker worker, int currentRecursiveCall = 0)
        {
            // Check startingVersion validity
            if (startingVersion != null)
            {
                // If this is the first recursive call, make sure that the starting version exists (since it's user-editable)
                if (currentRecursiveCall == 0)
                {
                    if (!startingVersion.IsSubVersionOf(branch) || !RemoteFileExists(startingVersion.ToString(maxDepth) + urlSuffix, client))
                        startingVersion = null;
                }
                // Otherwise, make sure that startingVersion is still useful (e.g. if startingVersion is 1.10.3 and at the third recursive call branch is 1.11,
                // the number '3' must not be used given that we are in an other branch (1.11.x vs 1.10.x))
                else
                {
                    if (startingVersion[branch.Depth-1] != branch[branch.Depth-1])
                        startingVersion = null;
                }
            }

            // Set the index from which start the loop
            int startingVersionIndex;
            if (startingVersion != null)
                startingVersionIndex = startingVersion[branch.Depth];
            else if (branch.Depth == 0)     // here we assume that the program doesn't have a 0.x branch (in our case Cemu)
                startingVersionIndex = 1;
            else
                startingVersionIndex = 0;

            // Start version scanning
            bool lastCheckedVersionExists = true;
            int iterationsCompleted = 0;
            VersionNumber lastCheckedVersion = new VersionNumber(branch);
            lastCheckedVersion.AddNumber(startingVersionIndex);

            while (lastCheckedVersionExists)
            {
                if (RemoteFileExists(lastCheckedVersion.ToString(maxDepth) + urlSuffix, client))
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
            if (lastCheckedVersion == null)                 // no versions have been found in this branch (probable wrong parameter/s)
                return null;
            if (lastCheckedVersion.Depth < maxDepth)        // maximum depth has not been reached yet
                return GetLatestRemoteVersionInBranch(lastCheckedVersion, client, urlSuffix, maxDepth, startingVersion, worker, currentRecursiveCall + 1);
            else                                            // finished scanning: return the latest version you found
                return lastCheckedVersion;
        }
    }

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
            request.ServicePoint.MaxIdleTime = 30000;

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
}
