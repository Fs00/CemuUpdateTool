using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CemuUpdateTool
{
    public struct VersionNumber
    {
        private List<int> fields;           // list that contains each of the "sub-numbers"
        public int Depth => fields.Count;   // returns the "depth" of the version number (e.g. 1.6.4 has depth 3)

        /*
         *  "Alias" properties for first, second, third and fourth field, like Microsoft does in FileVersionInfo class
         */
        public int Major {
            get => fields[0];
            set => fields[0] = value;
        }

        public int Minor {
            get
            {
                if (Depth < 2)
                    throw new InvalidOperationException("Field \"Minor\" does not exist: Depth is less than 2.");
                return fields[1];
            }
            set
            {
                if (Depth < 2)
                    throw new InvalidOperationException("Field \"Minor\" does not exist: Depth is less than 2.");
                fields[1] = value;
            }
        }

        public int Build {
            get
            {
                if (Depth < 3)
                    throw new InvalidOperationException("Field \"Build\" does not exist: Depth is less than 3.");
                return fields[2];
            }
            set
            {
                if (Depth < 3)
                    throw new InvalidOperationException("Field \"Build\" does not exist: Depth is less than 3.");
                fields[2]= value;
            }
        }

        public int Private {
            get
            {
                if (Depth < 4)
                    throw new InvalidOperationException("Field \"Private\" does not exist: Depth is less than 4.");
                return fields[3];
            }
            set
            {
                if (Depth < 4)
                    throw new InvalidOperationException("Field \"Build\" does not exist: Depth is less than 3.");
                fields[3] = value;
            }
        }

        // Indexer for fields list
        public int this[int i]
        {
            get => fields[i];
            set => fields[i] = value;
        }

        // Constructor that takes in input a string like "1.5.2" (separator is '.')
        public VersionNumber(string version)
        {
            string[] splittedVersionNr = version.Split('.');
            fields = new List<int>();
            foreach (string field in splittedVersionNr)
                fields.Add(Convert.ToInt32(field));
        }

        // Constructor that takes in input a FileVersionInfo object. The optional parameter specifies the depth of the new VersionNumber instance
        public VersionNumber(FileVersionInfo versionInfo, int depth = 4)
        {
            if (depth < 1 || depth > 4)
                throw new ArgumentException("Depth value is not valid.");

            fields = new List<int>(depth);
            fields[0] = versionInfo.FileMajorPart;
            if (depth > 1)
            {
                fields[1] = versionInfo.FileMinorPart;
                if (depth > 2)
                {
                    fields[2] = versionInfo.FileBuildPart;
                    if (depth > 3)
                        fields[3] = versionInfo.FilePrivatePart;
                }
            }
        }
    }
}
