using System;
using System.Linq;
using BadPath = System.IO.Path;

namespace LessIO
{
    /// <summary>
    /// Represents a file system path.
    /// </summary>
    public struct Path : IEquatable<Path>
    {
        private readonly string _path;
        private static readonly string _pathEmpty = string.Empty;
        public static readonly Path Empty = new Path();

        // TODO: Add validation using a strategy? Or just use it as a strongly typed path to force caller to be explicit?

        public Path(string path)
        {
            //To maintain sanity NEVER let the Path object store the long path prefixes. That is a hack for Win32 that should only ever be used just before calling the Win32 API and stripped out of any paths coming out of the Win32 API.
            _path = StripWin32PathPrefix(path);
        }

        private static string StripWin32PathPrefix(string pathString)
        {
            //TODO: UNC
            if (pathString.StartsWith(Path.Win32LongPathPrefixDontUse))
                return pathString.Substring(Path.Win32LongPathPrefixDontUse.Length);
            else
                return pathString;
        }

        /// <summary>
        /// Returns the directory seperator characers.
        /// </summary>
        internal static char[] DirectorySeperatorChars
        {
            get
            {
                return new char[] { BadPath.DirectorySeparatorChar, BadPath.AltDirectorySeparatorChar };
            }
        }

        internal static bool IsDirectorySeparator(char ch)
        {
            return Array.Exists<char>(DirectorySeperatorChars, c => c == ch);
        }

        /// <summary>
        /// Gets the full path of the directory or file.
        /// </summary>
        public string FullName
        {
            get { return _path != null ? _path : _pathEmpty; }
        }

        public bool IsEmpty
        {
            get { return Equals(Path.Empty); }
        }

        /// <summary>
        /// Indicates if the two paths are equivelent and point to the same file or directory.
        /// </summary>
        private static bool PathEquals(string pathA, string pathB)
        {
            /* Now we never let the Win32 long path prefix get into a Path instance:
            pathA = StripWin32PathPrefix(pathA);
            pathB = StripWin32PathPrefix(pathB);
            */
            pathA = pathA.TrimEnd(DirectorySeperatorChars);
            pathB = pathB.TrimEnd(DirectorySeperatorChars);
            var partsA = pathA.Split(DirectorySeperatorChars);
            var partsB = pathB.Split(DirectorySeperatorChars);
            if (partsA.Length != partsB.Length)
                return false;

            for (var i = 0; i < partsA.Length; i++)
            {
                var areEqual = string.Equals(partsA[i], partsB[i], StringComparison.InvariantCultureIgnoreCase);
                if (!areEqual)
                    return false;
            }
            return true;
        }

        public static bool operator ==(Path a, Path b)
        {
            return Path.Equals(a, b);
        }

        public static bool operator !=(Path a, Path b)
        {
            return !Path.Equals(a, b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            return Equals((Path)obj);
        }

        public bool Equals(Path other)
        {
            return Path.PathEquals(this.FullName, other.FullName);
        }

        internal static bool Equals(Path a, Path b)
        {
            return PathEquals(a.FullName, b.FullName);
        }

        
        /// <summary>
        /// This is the special prefix to prepend to paths to support up to 32,767 character paths.
        /// </summary>
        private static readonly string Win32LongPathPrefixDontUse = @"\\?\";
        /*
        /// <summary>
        /// Returns the specified path with the <see cref="LongPathPrefix"/> prepended if necessary.
        /// </summary>
        public static string EnsureWin32LongPathPrefix(string path)
        {
            //TODO: Needs to look for/support UNC path prefix too!
            if (!path.StartsWith(Win32LongPathPrefixDontUse)) // More consistent to deal with if we just add it to all of them: if (!path.StartsWith(LongPathPrefix) && path.Length >= MAX_PATH)
                return Win32LongPathPrefixDontUse + path;
            else
                return path;
        }

        public static Path EnsureWin32LongPathPrefix(Path path)
        {
            return new Path(EnsureWin32LongPathPrefix(path.ToString()));
        }
        */

        /// <summary>
        /// Long-form filenames are not supported by the .NET system libraries, so we do win32 calls.
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/aa365247%28v=vs.85%29.aspx#maxpath
        /// </summary>
        /// <remarks>
        /// The <see cref="Path"/> object will never store the Win32 long path prefix. Instead use this method to trim it off when necessary.
        /// </remarks>
        public string WithWin32LongPathPrefix()
        {
            const string Win32LongPathPrefix = @"\\?\";
            //TODO: Needs to look for/support UNC path prefix too!
            if (!FullName.StartsWith(Win32LongPathPrefix)) // More consistent to deal with if we just add it to all of them: if (!path.StartsWith(LongPathPrefix) && path.Length >= MAX_PATH)
                return Win32LongPathPrefix + this.FullName;
            else
                return this.FullName;
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        public override string ToString()
        {
            return FullName.ToString();
        }

        /// <summary>
        /// Gets the root directory information of the specified path.
        /// </summary>
        public string PathRoot
        {
            get
            {
                return GetPathRoot(this.FullName);
            }
        }

        /// <summary>
        /// Modeled after <see cref="System.IO.Path.GetPathRoot(string)"/> but supports long path names.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <remarks>
        /// See https://msdn.microsoft.com/en-us/library/system.io.path.getpathroot%28v=vs.110%29.aspx
        /// Possible patterns for the string returned by this method are as follows:
        /// An empty string (path specified a relative path on the current drive or volume).
        /// "/"(path specified an absolute path on the current drive).
        /// "X:"(path specified a relative path on a drive, where X represents a drive or volume letter).
        /// "X:/"(path specified an absolute path on a given drive).
        /// "\\ComputerName\SharedFolder"(a UNC path).
        /// </remarks>
        internal static string GetPathRoot(string path)
        {
            // "X:/"(path specified an absolute path on a given drive).
            if (path.Length >= 3 && path[1] == ':' && IsDirectorySeparator(path[2]))
                return path.Substring(0, 3);
            // "X:"(path specified a relative path on a drive, where X represents a drive or volume letter).
            if (path.Length >= 2 && path[1] == ':')
            {
                return path.Substring(0, 2);
            }
            // "\\ComputerName\SharedFolder"(a UNC path).
            // NOTE: UNC Path "root" includes the server/host AND have the root share folder too.
            if (path.Length > 2
                && IsDirectorySeparator(path[0])
                && IsDirectorySeparator(path[1])
                && path.IndexOfAny(DirectorySeperatorChars, 2) > 2)
            {
                var beginShareName = path.IndexOfAny(DirectorySeperatorChars, 2);
                var endShareName = path.IndexOfAny(DirectorySeperatorChars, beginShareName + 1);
                if (endShareName < 0)
                    endShareName = path.Length;
                if (beginShareName > 2 && endShareName > beginShareName)
                    return path.Substring(0, endShareName);
            }
            // "/"(path specified an absolute path on the current drive).
            if (path.Length >= 1 && IsDirectorySeparator(path[0]))
            {
                return path.Substring(0, 1);
            }
            // path specified a relative path on the current drive or volume?
            return "";
        }

        public static Path Combine(Path path1, params string[] pathParts)
        {
            if (path1.IsEmpty)
                throw new ArgumentNullException("path1");
            if (pathParts == null || pathParts.Length == 0)
                throw new ArgumentNullException("pathParts");

            string[] allStrings = new string[pathParts.Length + 1];
            allStrings[0] = path1.FullName;
            Array.Copy(pathParts, 0, allStrings, 1, pathParts.Length);
            return Combine(allStrings);
        }

        public static Path Combine(params Path[] pathParts)
        {
            if (pathParts == null)
                throw new ArgumentNullException();
            var strs = pathParts.Select(p => p.ToString());
            return Combine(strs.ToArray());
        }

        public static Path Combine(params string[] pathParts)
        {
            if (pathParts == null)
                throw new ArgumentNullException();
            if (pathParts.Length < 2)
                throw new ArgumentException("Expected at least two parts to combine.");
            var output = BadPath.Combine(pathParts[0], pathParts[1]);
            for (var i = 2; i < pathParts.Length; i++)
            {
                output = BadPath.Combine(output, pathParts[i]);
            }
            return new Path(output);
        }

        public Path Parent
        {
            get
            {
                var path = this.FullName;
                path = path.TrimEnd(Path.DirectorySeperatorChars);
                var parentEnd = path.LastIndexOfAny(Path.DirectorySeperatorChars);
                if (parentEnd >= 0 && parentEnd > GetPathRoot(path).Length)
                {
                    var result = path.Substring(0, parentEnd);
                    return new Path(result);
                }
                else
                    return Path.Empty;
            }
        }

        /// <summary>
        /// For code compatibility with <see cref="System.IO.FileSystemInfo.Exists"/>.
        /// </summary>
        public bool Exists
        {
            get { return FileSystem.Exists(this); }
        }

        /// <summary>
        /// For code compatibility with <see cref="System.IO.FileSystemInfo.Delete()"/>
        /// </summary>
        public void Delete()
        {
            FileSystem.RemoveFile(this, false);
        }

        /// <summary>
        /// For code compatibility with <see cref="System.IO.FileInfo.CreateText()"/>
        /// </summary>
        public System.IO.StreamWriter CreateText()
        {
            var stream = FileSystem.CreateFile(this);
            return new System.IO.StreamWriter(stream, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// For code compatibility with <see cref="System.IO.Path.GetFileName(string)"/>
        /// </summary>
        public static string GetFileName(string path)
        {
            return BadPath.GetFileName(path);
        }
    }
}
