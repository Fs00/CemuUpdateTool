using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CemuUpdateTool
{
    #pragma warning disable CS0659
    #pragma warning disable CS0661
    /*
     *  VersionNumber
     *  Provides a mutable data structure for version numbers, which can have any number of segments (e.g. 1.6.5 has 3 segments)
     */
    public class VersionNumber : IEquatable<VersionNumber>, IComparable<VersionNumber>
    {
        public static VersionNumber Empty => new VersionNumber();

        private readonly List<int> segments;
        public int Length => segments.Count;

        /*
         *  Indexer for segments list
         *  Every value assignation to segments list must be done using this indexer, since values are checked only here
         */
        public int this[int index]
        {
            get => segments[index];
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));
                segments[index] = value;
            }
        }

        #region Alias properties for first, second and third segment, like Microsoft does in Version class
        public int Major {
            get => this[0];
            set => this[0] = value;
        }

        public int Minor {
            get => this[1];
            set => this[1] = value;
        }

        public int Build {
            get => this[2];
            set => this[2] = value;
        }
        #endregion

        #region Constructor methods
        private VersionNumber()
        {
            segments = new List<int>();
        }

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
                throw new ArgumentOutOfRangeException(nameof(length));

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
                throw new ArgumentNullException(nameof(segments));

            this.segments = new List<int>(segments.Length);
            foreach (int number in segments)
                AppendSegment(number);
        }
        #endregion

        /*
         *  Safely checks if a string is a valid VersionNumber
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

        #region Methods for increasing/decreasing length
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
            segments.RemoveAt(Length - 1);
        }
        #endregion

        #region Methods for bumping up/down version number
        public void Bump()
        {
            segments[Length-1]++;
        }

        public void BumpDown()
        {
            this[Length-1] -= 1;
        }
        #endregion

        public VersionNumber GetCopyOfLength(int copyLength)
        {
            VersionNumber copy = new VersionNumber(this);
            if (this.Length > copyLength)
                copy.RemoveLastSegments(amount: this.Length - copyLength);
            else if (this.Length < copyLength)
                copy.AppendZeroSegments(amount: copyLength - this.Length);
            return copy;
        }

        private void RemoveLastSegments(int amount)
        {
            if (amount > Length)
                throw new ArgumentOutOfRangeException(nameof(amount));
            
            for (int i = 0; i < amount; i++)
                RemoveLastSegment();
        }

        private void AppendZeroSegments(int amount)
        {
            for (int i = 0; i < amount; i++)
                AppendSegment(0);
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

            return CompareCommonSegmentsWith(other) == ComparisonResult.BothInstancesAreEquivalent;
        }

        #region Comparison methods
        private enum ComparisonResult
        {
            BothInstancesAreEquivalent = 0,
            ThisIsGreater = 1,
            OtherIsGreater = -1
        }

        /*
         *  Returns -1 if this < other, 0 if other == this, or 1 if this > other
         *  Please take note that 1.3 is considered equal to 1.3.0.0!
         */
        public int CompareTo(VersionNumber other)
        {
            if (ReferenceEquals(this, other))
                return (int) ComparisonResult.BothInstancesAreEquivalent;

            if (other is null)
                return (int) ComparisonResult.ThisIsGreater;

            ComparisonResult commonSegmentsComparisonResult = CompareCommonSegmentsWith(other);
            if (this.Length == other.Length)
                return (int) commonSegmentsComparisonResult;

            int lengthInCommon = FindLengthInCommonBetweenThisAnd(other);
            VersionNumber longerNumber = (this.Length > other.Length) ? this : other;
            if (longerNumber.AreLastSegmentsEqualToZero(startingFrom: lengthInCommon))
                return (int) ComparisonResult.BothInstancesAreEquivalent;
            else if (ReferenceEquals(longerNumber, this))
                return (int) ComparisonResult.ThisIsGreater;
            else
                return (int) ComparisonResult.OtherIsGreater;
        }

        private ComparisonResult CompareCommonSegmentsWith(VersionNumber other)
        {
            int lengthInCommon = FindLengthInCommonBetweenThisAnd(other);
            for (int i = 0; i < lengthInCommon; i++)
            {
                if (this[i] > other[i])
                    return ComparisonResult.ThisIsGreater;
                if (this[i] < other[i])
                    return ComparisonResult.OtherIsGreater;
            }
            return ComparisonResult.BothInstancesAreEquivalent;
        }

        private int FindLengthInCommonBetweenThisAnd(VersionNumber other)
        {
            return (this.Length > other.Length) ? other.Length : this.Length;
        }

        private bool AreLastSegmentsEqualToZero(int startingFrom)
        {
            if (startingFrom >= Length)
                throw new ArgumentOutOfRangeException(nameof(startingFrom));

            for (int i = startingFrom; i < Length; i++)
            {
                if (this[i] != 0)
                    return false;
            }
            return true;
        }
        #endregion

        #region ToString() methods
        public override string ToString()
        {
            return ToString('.');
        }

        /*
         *  Overload that allows to supply a custom separator between segments
         */
        public string ToString(char separator)
        {
            if (Length == 0)
                return string.Empty;

            StringBuilder output = new StringBuilder();
            for (int i = 0; i < Length; i++)
            {
                output.Append(segments[i]);
                if (i < Length - 1)
                    output.Append(separator);
            }
            return output.ToString();
        }
        #endregion

        #region Equals() methods
        /*
         *  IEquatable<T>.Equals method
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
            if (obj is VersionNumber versionNumber)
                return Equals(versionNumber);
            return base.Equals(obj);
        }
        #endregion

        #region Increment/decrement operators overloads
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
        #endregion

        #region Comparison operators overloads
        // In case the left term is null, there's only a simple rule: null is minor than any VersionNumber instance
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
        #endregion
    }
}
