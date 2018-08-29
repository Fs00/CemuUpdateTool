namespace System.Reflection
{
    public class AssemblyReleaseDateAttribute : Attribute
    {
        public DateTime ReleaseDate { get; }

        public AssemblyReleaseDateAttribute(int year, int month, int day)
        {
            ReleaseDate = new DateTime(year, month, day);
        }

        public override string ToString()
        {
            return ReleaseDate.ToString("yyyy/MM/dd");
        }
    }
}
