using System.Net;
using System.Threading;

namespace CemuUpdateTool.Utils
{
    public class RemoteVersionChecker
    {
        // This is Cemu-specific
        private static readonly VersionNumber defaultStartingVersion = new VersionNumber(1, 0, 0);

        private readonly string urlPrefix, urlSuffix;
        private readonly int maxVersionLength;

        private CancellationToken cancToken;
        private MyWebClient webClient;

        public RemoteVersionChecker(string urlPrefix, string urlSuffix, int maxVersionLength)
        {
            this.urlPrefix = urlPrefix;
            this.urlSuffix = urlSuffix;
            this.maxVersionLength = maxVersionLength;
        }

        /*
         *  Finds the latest online version for a program in a given branch (e.g. 1.x, 2.5.x, x, etc.).
         *  In order for this method to work, all versions must be in the same remote folder (obviously publicly accessible)
         *  and files be named in the format [urlPrefix][version][urlSuffix].
         */
        public VersionNumber GetLatestRemoteVersionInBranch(VersionNumber branch,
                                                            VersionNumber startingVersion = null,
                                                            CancellationToken? cancToken = null)
        {
            using (webClient = new MyWebClient())
            {
                if (cancToken != null)
                {
                    this.cancToken = cancToken.Value;
                    this.cancToken.Register(() => webClient.CancelAsync());
                }
                else
                    this.cancToken = CancellationToken.None;

                if (startingVersion != null)
                {
                    if (!RemoteVersionExists(startingVersion))
                        startingVersion = null;
                }

                return RecursiveGetLatestRemoteVersionInBranch(new VersionNumber(branch), startingVersion);
            }
        }

        private VersionNumber RecursiveGetLatestRemoteVersionInBranch(VersionNumber branch, VersionNumber startingVersion = null)
        {
            if (startingVersion != null)
            {
                // If this condition is not met, the starting version is useless (e.g. if startingVersion is 1.10.3 and branch is 1.11,
                // the segment '3' must not be used since we are in an other branch (1.11.x vs 1.10.x))
                if (!startingVersion.IsSubVersionOf(branch))
                    startingVersion = null;
            }

            int startingSegmentValueForCurrentBranch;
            if (startingVersion != null)
                startingSegmentValueForCurrentBranch = startingVersion[branch.Length];
            else
                startingSegmentValueForCurrentBranch = defaultStartingVersion[branch.Length];
            branch.AppendSegment(startingSegmentValueForCurrentBranch);

            RunVersionCheckingLoopForCurrentBranch(branch);

            if (branch.Length < maxVersionLength)
                return RecursiveGetLatestRemoteVersionInBranch(branch, startingVersion);
            else
                return branch;
        }

        /*
         * latestKnownVersion parameter gets updated with the latest version found.
         * This method assumes that the initial value of latestKnownVersion represents an existing version, since:
         *   - in the first recursive call, branch must belong to an existing branch
         *   - in the subsequent recursive calls, latestKnownVersion has already been checked
         *     in the loop of the previous recursive call
         */
        private void RunVersionCheckingLoopForCurrentBranch(VersionNumber latestKnownVersion)
        {
            bool latestVersionFound = false;
            VersionNumber nextVersionToCheck = new VersionNumber(latestKnownVersion);
            while (!latestVersionFound)
            {
                cancToken.ThrowIfCancellationRequested();
                nextVersionToCheck++;
                if (RemoteVersionExists(nextVersionToCheck))
                    latestKnownVersion++;
                else
                    latestVersionFound = true;
            }
        }

        private bool RemoteVersionExists(VersionNumber version)
        {
            return RemoteVersionExists(version, webClient);
        }

        public bool RemoteVersionExists(VersionNumber version, MyWebClient webClient)
        {
            using (HttpWebResponse response = webClient.GetWebResponseHead(urlPrefix + version.ToString(maxVersionLength) + urlSuffix))
            {
                return response.StatusCode == HttpStatusCode.OK;
            }
        }
    }
}
