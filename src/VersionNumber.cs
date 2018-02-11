using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CemuUpdateTool
{
    #pragma warning disable CS0661
    public class VersionNumber : IEquatable<VersionNumber>, IComparable<VersionNumber>
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
            get
            {
                if (Depth < 1)
                    throw new InvalidOperationException("Field \"Major\" does not exist: Depth is 0.");
                return fields[0];
            }
            set
            {
                if (Depth < 1)
                    throw new InvalidOperationException("Field \"Major\" does not exist: Depth is 0.");
                this[0] = value;
            }
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
                    throw new InvalidOperationException("Field \"Private\" does not exist: Depth is less than 4.");
                this[3] = value;
            }
        }

        // Default constructor
        public VersionNumber()
        {
            fields = new List<int>();
        }

        // Copy constructor
        public VersionNumber(VersionNumber copy) : this()
        {
            for (int i = 0; i < copy.Depth; i++)
                fields.Add(copy[i]);
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
         *  ToString() methods
         *  Custom ToString(char) method allows to supply a custom separator between fields (even '' if you want).
         *  Custom ToString(int[, char]) allows to supply a custom depth in order to print only a part of the version number (e.g. 1.6 for 1.6.2)
         *    or an arbitrary number of ".0"s after the version number (e.g. 1.0.0 for 1.0)
         *  Overridden ToString() uses ToString(char) with default separator (which is '.')
         */
        public string ToString(char separator)
        {
            if (Depth == 0)
                return "x";

            string output = "";
            for (int i = 0; i < Depth; i++)
            {
                output += fields[i];
                if (i < Depth-1)
                    output += separator;
            }
            return output;
        }

        public string ToString(int customDepth, char separator = '.')
        {
            string output = "";
            if (customDepth == Depth)
                return ToString(separator);
            else if (customDepth > Depth)
            {
                // adds an extra number of ".0"s until i reaches customDepth
                output += ToString(separator) + separator;
                for (int i = Depth; i < customDepth; i++)
                {
                    output += "0";
                    if (i < customDepth-1)
                        output += separator;
                }
            }
            else
            {
                // like ToString(char), but it stops at customDepth index
                for (int i = 0; i < customDepth; i++)
                {
                    output += fields[i];
                    if (i < customDepth - 1)
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
            if (Depth == 0)
                throw new InvalidOperationException("There are no more fields in this instance.");
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

            int commonDepth = Depth;
            // See if both VersionNumber objects have the same depth, if not set as common depth the lesser between the two
            bool differentDepths = (this.Depth != other.Depth);
            if (differentDepths)
                commonDepth = (this.Depth > other.Depth) ? other.Depth : this.Depth;

            // Compare the common fields. If a pair of fields is different, their difference will be the return value
            for (int i = 0; i < commonDepth; i++)
            {
                if (this[i] != other[i])
                    return this[i]-other[i];
            }

            // See if the extra fields in the object with the higher depth are equal to 0, if not it will mean that the object is > than the other
            if (differentDepths)
            {
                if (this.Depth > other.Depth)
                {
                    for (int i = commonDepth; i < this.Depth; i++)
                    {
                        if (this[i] > 0)
                            return 1;
                    }
                }
                else
                {
                    for (int i = commonDepth; i < other.Depth; i++)
                    {
                        if (other[i] > 0)
                            return -1;
                    }
                }
            }
            // If both objects have same depth or the extra fields are equal to 0, it means that they're equal
            return 0;
        }

        /*
         *  IEquatable<T>.Equals method
         *  Uses CompareTo to determine if two Version number objects are equal.
         *  Remember that 1.3 is considered equal to 1.3.0.0!
         */
        public bool Equals(VersionNumber other)
        {
            if (this.CompareTo(other) == 0)
                return true;
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is VersionNumber)
                return Equals((VersionNumber) obj);
            return base.Equals(obj);
        }

        /*
         *  Comparison operators overloads
         *  They use CompareTo and Equals to do their work
         */
        public static bool operator >(VersionNumber left, VersionNumber right)
        {
            if (left.CompareTo(right) > 0)
                return true;
            else
                return false;
        }

        public static bool operator <(VersionNumber left, VersionNumber right)
        {
            if (left.CompareTo(right) < 0)
                return true;
            else
                return false;
        }

        public static bool operator >=(VersionNumber left, VersionNumber right)
        {
            if (left.CompareTo(right) >= 0)
                return true;
            else
                return false;
        }

        public static bool operator <=(VersionNumber left, VersionNumber right)
        {
            if (left.CompareTo(right) <= 0)
                return true;
            else
                return false;
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
