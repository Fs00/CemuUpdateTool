using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CemuUpdateTool
{
    #pragma warning disable CS0659
    #pragma warning disable CS0661
    public class VersionNumber : IEquatable<VersionNumber>, IComparable<VersionNumber>
    {
        private List<int> segments;            // list that contains each of the version segments
        public int Length => segments.Count;   // returns the number of segments of the version number (e.g. 1.6.4 has length 3)
        
        /*
         *  Indexer for segments list
         *  Input values are checked only here, so every value assignation to segments MUST BE DONE FROM HERE
         */
        public int this[int i]
        {
            get => segments[i];
            set
            {
                if (value < 0)
                    throw new ArgumentException("Segment values must not be negative.");
                segments[i] = value;
            }
        }

        /*
         *  "Alias" properties for first, second, third and fourth segment, like Microsoft does in Version class
         */
        public int Major {
            get
            {
                if (Length < 1)
                    throw new InvalidOperationException("Major segment does not exist: Length is 0.");
                return segments[0];
            }
            set
            {
                if (Length < 1)
                    throw new InvalidOperationException("Major segment does not exist: Length is 0.");
                this[0] = value;
            }
        }

        public int Minor {
            get
            {
                if (Length < 2)
                    throw new InvalidOperationException("Minor segment does not exist: Length is less than 2.");
                return segments[1];
            }
            set
            {
                if (Length < 2)
                    throw new InvalidOperationException("Minor segment does not exist: Length is less than 2.");
                this[1] = value;
            }
        }

        public int Build {
            get
            {
                if (Length < 3)
                    throw new InvalidOperationException("Build segment does not exist: Length is less than 3.");
                return segments[2];
            }
            set
            {
                if (Length < 3)
                    throw new InvalidOperationException("Build segment does not exist: Length is less than 3.");
                this[2] = value;
            }
        }

        public int Revision {
            get
            {
                if (Length < 4)
                    throw new InvalidOperationException("Revision segment does not exist: Length is less than 4.");
                return segments[3];
            }
            set
            {
                if (Length < 4)
                    throw new InvalidOperationException("Revision segment does not exist: Length is less than 4.");
                this[3] = value;
            }
        }

        // Default constructor (used only internally)
        private VersionNumber()
        {
            segments = new List<int>();
        }

        // Calls default private constructor to get an instance with 0 segments
        public static VersionNumber Empty { get => new VersionNumber(); }

        // Copy constructor
        public VersionNumber(VersionNumber copy)
        {
            segments = new List<int>(copy.Length);
            for (int i = 0; i < copy.Length; i++)
                segments.Add(copy[i]);
        }

        // Constructor that takes in input a string like "1.5.2" (only allowed separator is '.')
        public VersionNumber(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Invalid string passed as argument.");

            string[] splittedVersionNr = version.Split('.');
            segments = new List<int>(splittedVersionNr.Length);
            foreach (string segment in splittedVersionNr)
                AppendSegment(Convert.ToInt32(segment));
        }

        /*
         *  Constructor that takes in input a FileVersionInfo object
         *  The optional parameter specifies the length of the new VersionNumber instance.
         *  Input checking using indexer/AppendSegment is not needed since a FileVersionInfo object can't have negative fields
         */
        public VersionNumber(FileVersionInfo versionInfo, int length = 4)
        {
            if (length < 1 || length > 4)
                throw new ArgumentOutOfRangeException("Length value is not valid.");

            segments = new List<int>(length);
            segments.Add(versionInfo.FileMajorPart);
            if (length > 1)
            {
                segments.Add(versionInfo.FileMinorPart);
                if (length > 2)
                {
                    segments.Add(versionInfo.FileBuildPart);
                    if (length > 3)
                        segments.Add(versionInfo.FilePrivatePart);
                }
            }
        }

        // Constructor that takes in input an arbitrary number of ints
        public VersionNumber(params int[] segments)
        {
            if (segments == null)
                throw new ArgumentNullException("Segments array is null.");

            this.segments = new List<int>(segments.Length);
            foreach (int number in segments)
                AppendSegment(number);
        }

        /*
         *  ToString() methods
         *  Custom ToString(char) method allows to supply a custom separator between segments (even '' if you want).
         *  Custom ToString(int[, char]) allows to supply a custom length in order to print only a part of the version number (e.g. 1.6 for 1.6.2)
         *    or an arbitrary number of ".0"s after the version number (e.g. 1.0.0 for 1.0)
         *  Overridden ToString() uses ToString(char) with default separator (which is '.')
         */
        public string ToString(char separator)
        {
            if (Length == 0)
                return "[empty]";

            string output = "";
            for (int i = 0; i < Length; i++)
            {
                output += segments[i];
                if (i < Length-1)
                    output += separator;
            }
            return output;
        }

        public string ToString(int customLength, char separator = '.')
        {
            string output = "";
            if (customLength == Length)
                return ToString(separator);
            else if (customLength > Length)
            {
                // adds an extra number of ".0"s until i reaches customLength
                output += ToString(separator) + separator;
                for (int i = Length; i < customLength; i++)
                {
                    output += "0";
                    if (i < customLength-1)
                        output += separator;
                }
            }
            else
            {
                // like ToString(char), but it stops at customLength index
                for (int i = 0; i < customLength; i++)
                {
                    output += segments[i];
                    if (i < customLength - 1)
                        output += separator;
                }
            }
            return output;
        }

        public override string ToString()
        {
            return ToString('.');
        }

        /*
         *  Methods for increasing/decreasing length
         */
        public void AppendSegment(int number)
        {
            if (number < 0)
                throw new ArgumentException("Segment values must not be negative.");
            segments.Add(number);
        }

        public void RemoveLastSegment()
        {
            if (Length == 0)
                throw new InvalidOperationException("There are no segments in this instance.");
            segments.RemoveAt(Length-1);
        }

        /*
         *  Methods for bumping up/down version number
         *  You can also do major version bumps (e.g. 1.1.5 => 1.2.0)
         */
        public void Bump(int segmentIndex)
        {
            if (segmentIndex < 0 || segmentIndex >= Length)
                throw new ArgumentOutOfRangeException($"Segment at index {segmentIndex} doesn't exist.");

            segments[segmentIndex]++;
            if (segmentIndex < Length-1)   // if you're doing a major version bump
            {
                for (int i = segmentIndex+1; i < Length; i++)
                    segments[i] = 0;
            }
        }

        public void Bump()
        {
            Bump(Length-1);
        }

        public void BumpDown()
        {
            this[Length-1] -= 1;
        }

        /*
         *  Method used to check if a version is a sub-version of another (e.g. 1.5.3 is a subversion of 1 and 1.5)
         *  Take note that a version is not a sub-version of itself and that every version is a sub-version of an empty instance
         */
        public bool IsSubVersionOf(VersionNumber other)
        {
            if (other is null)
                return false;
            if (this.Length <= other.Length)
                return false;

            // Check if the common segments of both instances contain the same values
            for (int i = 0; i < other.Length; i++)
            {
                if (this[i] != other[i])
                    return false;
            }

            return true;
        }

        /*
         *  Increment/decrement operators overload
         */
        public static VersionNumber operator ++(VersionNumber instance)
        {
            instance.Bump();
            return instance;
        }

        public static VersionNumber operator --(VersionNumber instance)
        {
            instance.BumpDown();
            return instance;
        }

        /*
         *  TryParse method to safely check if a string is a valid VersionNumber
         */
        public static bool TryParse(string input, out VersionNumber result)
        {
            try
            {
                result = new VersionNumber(input);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        /*
         *  CompareTo() method.
         *  Returns a negative number if this < other, 0 if other == this, a positive number if this > other
         *  Please take note that 1.3 is considered equal to 1.3.0.0!
         */
        public int CompareTo(VersionNumber other)
        {
            // It's useless to compare the exact same object
            if (ReferenceEquals(this, other))
                return 0;

            if (other is null)
                return 1;

            int commonLength = Length;
            // See if both VersionNumber objects have the same length, if not set as common length the lesser between the two
            bool differentLengths = (this.Length != other.Length);
            if (differentLengths)
                commonLength = (this.Length > other.Length) ? other.Length : this.Length;

            // Compare the common segments. If a pair of segments is different, their difference will be the return value
            for (int i = 0; i < commonLength; i++)
            {
                if (this[i] != other[i])
                    return this[i] - other[i];
            }

            // See if the extra segments in the object with the higher length are equal to 0, if not it will mean that the instance is > than the other
            if (differentLengths)
            {
                if (this.Length > other.Length)
                {
                    for (int i = commonLength; i < this.Length; i++)
                    {
                        if (this[i] > 0)
                            return 1;
                    }
                }
                else
                {
                    for (int i = commonLength; i < other.Length; i++)
                    {
                        if (other[i] > 0)
                            return -1;
                    }
                }
            }
            // If both objects have same length or the extra segments are equal to 0, it means that they're equal
            return 0;
        }

        /*
         *  IEquatable<T>.Equals method
         *  Uses CompareTo to determine if two Version number objects are equal.
         *  Remember that 1.3 is considered equal to 1.3.0.0!
         */
        public bool Equals(VersionNumber other)
        {
            return this.CompareTo(other) == 0;
        }

        /*
         *  Overridden Object.Equals method
         */
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is VersionNumber o)
                return Equals(o);
            return base.Equals(obj);
        }

        /*
         *  Comparison operators overloads
         *  In case the left term is null, there's only a simple rule: null is minor than any VersionNumber instance
         */
        public static bool operator >(VersionNumber left, VersionNumber right)
        {
            if (left is null)
                return false;

            return left.CompareTo(right) > 0;
        }

        public static bool operator <(VersionNumber left, VersionNumber right)
        {
            if (left is null)
            {
                if (!(right is null))
                    return true;
                else
                    return false;
            }

            return left.CompareTo(right) < 0;
        }

        public static bool operator >=(VersionNumber left, VersionNumber right)
        {
            if (left is null)
            {
                if (right is null)
                    return true;
                else
                    return false;
            }

            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(VersionNumber left, VersionNumber right)
        {
            if (left is null)
                return true;

            return left.CompareTo(right) <= 0;
        }

        public static bool operator ==(VersionNumber left, VersionNumber right)
        {
            if (left is null)
                return ReferenceEquals(left, right);

            return left.Equals(right);
        }

        public static bool operator !=(VersionNumber left, VersionNumber right)
        {
            return !(left == right);
        }
    }
}
