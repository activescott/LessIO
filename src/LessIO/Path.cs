namespace LessIO
{
    /// <summary>
    /// Represents a file system path.
    /// </summary>
    public struct Path
    {
        private readonly string _path;
        private static readonly string _pathEmpty = string.Empty;
        public static readonly Path Empty = new Path();
        
        // TODO: Add validation using a strategy? Or just use it as a strongly typed path to force caller to be explicit?

        public Path(string path)
        {
            _path = path;
        }

        private string PathString
        {
            get { return _path != null ? _path : _pathEmpty; }
        }

        public override bool Equals(object obj)
        {
            return PathString.Equals(obj);
        }

        public override int GetHashCode()
        {
            return PathString.GetHashCode();
        }

        public override string ToString()
        {
            return PathString.ToString();
        }
    }
}
