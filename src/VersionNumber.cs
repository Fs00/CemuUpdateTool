using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CemuUpdateTool
{
    public class VersionNumber : IComparable<VersionNumber>
    {
        private List<int> fields;           // list that contains each of the "sub-numbers"
        public int Depth => fields.Count;   // returns the "depth" of the version number (e.g. 1.6.4 has depth 3)

        /*
         *  Indexer for fields list
         *  Input values are checked only here, so every value assignation to fields MUST BE DONE FROM HERE
         */
        public int this[int i]
        {
            get => fields[i];
            set
            {
                if (value < 0)
                    throw new ArgumentException("Field values must not be negative.");
                fields[i] = value;
            }
        }

        /*
         *  "Alias" properties for first, second, third and fourth field, like Microsoft does in Version and FileVersionInfo class
         */
        public int Major {
            get => fields[0];
            set => this[0] = value;
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
                this[1] = value;
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
                this[2] = value;
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
                    throw new InvalidOperationException("Field \"Private\" does not exist: Depth is less than 3.");
                this[3] = value;
            }
        }

        // Default constructor
        public VersionNumber()
        {
            fields = new List<int>();
            fields.Add(0);      // Depth must never be 0
        }

        // Constructor that takes in input a string like "1.5.2" (only allowed separator is '.')
        public VersionNumber(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Invalid string passed as argument.");

            string[] splittedVersionNr = version.Split('.');
            fields = new List<int>();
            foreach (string field in splittedVersionNr)
                AddNumber(Convert.ToInt32(field));
        }

        /*
         *  Constructor that takes in input a FileVersionInfo object
         *  The optional parameter specifies the depth of the new VersionNumber instance.
         *  Input checking using indexer/AddNumber is not needed since a FileVersionInfo object can't have negative fields
         */
        public VersionNumber(FileVersionInfo versionInfo, int depth = 4)
        {
            if (depth < 1 || depth > 4)
                throw new ArgumentOutOfRangeException("Depth value is not valid.");

            fields = new List<int>();
            fields.Add(versionInfo.FileMajorPart);
            if (depth > 1)
            {
                fields.Add(versionInfo.FileMinorPart);
                if (depth > 2)
                {
                    fields.Add(versionInfo.FileBuildPart);
                    if (depth > 3)
                        fields.Add(versionInfo.FilePrivatePart);
                }
            }
        }

        // Constructor that takes in input an arbitrary number of ints
        public VersionNumber(params int[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("Fields array is null.");

            this.fields = new List<int>();
            foreach (int number in fields)
                AddNumber(number);
        }

        /*
         *  ToString() methods. Custom ToString(char) method permits to supply a custom separator between fields (even '' if you want).
         *  Overridden ToString() uses custom ToString() with default separator (which is '.')
         */
        public string ToString(char separator)
        {
            string output = "";
            for (int i = 0; i < Depth; i++)
            {
                output += fields[i];
                if (i < Depth-1)
                    output += separator.ToString();
            }
            return output;
        }

        public override string ToString()
        {
            return ToString('.');
        }

        /*
         *  Methods for increasing/decreasing depth
         */
        public void AddNumber(int number)
        {
            if (number < 0)
                throw new ArgumentException("Field values must not be negative.");
            fields.Add(number);
        }

        public void RemoveLastNumber()
        {
            if (Depth <= 1)
                throw new InvalidOperationException("Depth can't be less than 1.");
            fields.RemoveAt(Depth-1);
        }

        /*
         *  Methods for bumping up/down version number
         *  You can also do major version bumps (e.g. 1.1.5 => 1.2.0)
         */
        public void BumpNumber(int fieldIndex)
        {
            if (fieldIndex < 0 || fieldIndex >= Depth)
                throw new ArgumentOutOfRangeException($"Field index {fieldIndex} doesn't exist.");

            fields[fieldIndex]++;
            if (fieldIndex < Depth-1)   // if you're doing a major version bump
            {
                for (int i = fieldIndex+1; i < Depth; i++)
                    fields[i] = 0;
            }
        }

        public void BumpNumber()
        {
            BumpNumber(Depth-1);
        }

        public void BumpDownNumber()
        {
            this[Depth-1] -= 1;
        }

        /*
         *  CompareTo() method.
         *  Returns -1 if other < this, 0 if other == this, 1 if other > this
         */
        public int CompareTo(VersionNumber other)
        {
            // TO BE DONE
            throw new NotImplementedException();
        }

        /*
         *  Increment/decrement operators overload
         */
        public static VersionNumber operator ++(VersionNumber instance)
        {
            instance.BumpNumber();
            return instance;
        }

        public static VersionNumber operator --(VersionNumber instance)
        {
            instance.BumpDownNumber();
            return instance;
        }
    }
}
