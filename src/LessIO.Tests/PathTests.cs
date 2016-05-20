using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SysPath = System.IO.Path;

namespace LessIO.Tests
{
    [TestClass]
    public class PathTests : TestBase
    {
        const string Win32LongPathPrefix = @"\\?\";
        private void TestGetParentPath(string expected, string testInput)
        {
            var actual = new Path(testInput).Parent.PathString;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetParentPathBasics()
        {
            TestGetParentPath(@"c:\aroot\aparent", @"c:\aroot\aparent\achild");
            TestGetParentPath(@"c:\aroot\aparent", @"c:\aroot\aparent\achild\");
        }

        [TestMethod]
        public void GetParentPathHandlesRoot()
        {
            TestGetParentPath(@"", @"c:\");
            TestGetParentPath(@"", @"\\myserver");

            TestGetParentPath(@"", @"\\myserver\myshare");
        }

        [TestMethod]
        public void GetParentPathSupportsLongPathNames()
        {
            var input = Win32LongPathPrefix + @"C:\src\lessmsi\src\Lessmsi.Tests\bin\Debug\MsiOutputTemp\long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping\SourceDir\Windows\winsxs\x86_Microsoft.VC90.CRT_1fc8b3b9a1e18e3b_9.0.21022.8_x-ww_d08d0375\once\more\again\what\the\heck\why\not\a\littlebit\longerpathjustforthefunof\it";
            var expected = @"C:\src\lessmsi\src\Lessmsi.Tests\bin\Debug\MsiOutputTemp\long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping\SourceDir\Windows\winsxs\x86_Microsoft.VC90.CRT_1fc8b3b9a1e18e3b_9.0.21022.8_x-ww_d08d0375\once\more\again\what\the\heck\why\not\a\littlebit\longerpathjustforthefunof";
            TestGetParentPath(expected, input);

            input = @"C:\src\lessmsi\src\Lessmsi.Tests\bin\Debug\MsiOutputTemp\long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping\SourceDir\Windows\winsxs\x86_Microsoft.VC90.CRT_1fc8b3b9a1e18e3b_9.0.21022.8_x-ww_d08d0375\once\more\again\what\the\heck\why\not\a\littlebit\longerpathjustforthefunof\it";
            expected = @"C:\src\lessmsi\src\Lessmsi.Tests\bin\Debug\MsiOutputTemp\long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping\SourceDir\Windows\winsxs\x86_Microsoft.VC90.CRT_1fc8b3b9a1e18e3b_9.0.21022.8_x-ww_d08d0375\once\more\again\what\the\heck\why\not\a\littlebit\longerpathjustforthefunof";
            TestGetParentPath(expected, input);
        }

        private void TestGetPathRoot(string expected, string testInput)
        {
            var actual = new Path(testInput).PathRoot;
            Assert.AreEqual(expected, actual);
        }

        // An empty string (path specified a relative path on the current drive or volume).
        [TestMethod]
        public void GetPathRootRelative()
        {
            TestGetPathRoot("", "apath/without/a/root");
            TestGetPathRoot("", "ab/without");
        }

        [TestMethod]
        // "/"(path specified an absolute path on the current drive).
        public void GetPathRootAbsoluteOnCurrentDrive()
        {
            TestGetPathRoot(@"/", @"/apath/with/root");
            TestGetPathRoot(@"\", @"\apath\withroot");
            TestGetPathRoot(@"\", @"\apath");
        }

        [TestMethod]
        // "X:"(path specified a relative path on a drive, where X represents a drive or volume letter).
        public void GetPathRootDriveLetter()
        {
            TestGetPathRoot(@"x:", @"x:");
            TestGetPathRoot(@"x:", @"x:abc");
        }

        [TestMethod]
        // "X:/"(path specified an absolute path on a given drive).
        public void GetPathRootAbsoluteRoot()
        {
            TestGetPathRoot(@"x:\", @"x:\abc");
            TestGetPathRoot(@"x:\", @"x:\abc\def");
            TestGetPathRoot(@"x:\", @"x:\");
            TestGetPathRoot(@"x:", @"x:");
        }

        [TestMethod]
        // "\\ComputerName\SharedFolder"(a UNC path).
        public void GetPathRootUNC()
        {
            TestGetPathRoot(@"\\ComputerName\SharedFolder", @"\\ComputerName\SharedFolder\subfolder");
            TestGetPathRoot(@"\\ComputerName\SharedFolder", @"\\ComputerName\SharedFolder\subfolder\");
            TestGetPathRoot(@"\\ComputerName\SharedFolder", @"\\ComputerName\SharedFolder\subfolder\another");
            TestGetPathRoot(@"\\ComputerName\SharedFolder", @"\\ComputerName\SharedFolder");
        }


        [TestMethod]
        public void FullPath()
        {
            // Tests from https://msdn.microsoft.com/en-us/library/system.io.path.getfullpath.aspx

            string input, expected;
            // GetFullPath('mydir') returns 'C:\temp\Demo\mydir'
            input = @"mydir";
            expected = WorkingDirectory + SysPath.DirectorySeparatorChar + @"mydir";
            Assert.AreEqual(expected, new Path(input).FullPathString);

            // GetFullPath('myfile.ext') returns 'C:\temp\Demo\myfile.ext'
            input = @"myfile.ext";
            expected = WorkingDirectory + SysPath.DirectorySeparatorChar + @"myfile.ext";
            Assert.AreEqual(expected, new Path(input).FullPathString);

            // GetFullPath('\mydir') returns 'C:\mydir'
            input = @"\mydir";
            expected = SysPath.GetPathRoot(WorkingDirectory) + @"mydir";
            Assert.AreEqual(expected, new Path(input).FullPathString);

            input = @"/mydir";
            expected = SysPath.GetPathRoot(WorkingDirectory) + @"mydir";
            Assert.AreEqual(expected, new Path(input).FullPathString);

            input = @"c:\mydir";
            expected = @"c:\mydir";
            Assert.AreEqual(expected, new Path(input).FullPathString);

            input = @"m:\mydir";
            expected = @"m:\mydir";
            Assert.AreEqual(expected, new Path(input).FullPathString);
        }

        private static string WorkingDirectory
        {
            get { return System.IO.Directory.GetCurrentDirectory(); }
        }

        [TestMethod]
        public void IsPathRooted()
        {
            /* 
             * For example, it returns true for path strings such as "\\MyDir\\MyFile.txt", "C:\\MyDir", or "C:MyDir". It returns false for path strings such as "MyDir".
             * - https://msdn.microsoft.com/en-us/library/system.io.path.ispathrooted%28v=vs.110%29.aspx
             */

            string input;
            //it returns true for path strings such as "\\MyDir\\MyFile.txt", "C:\\MyDir", or "C:MyDir". 
            input = @"\MyDir\MyFile.txt";
            Assert.IsTrue(new Path(input).IsPathRooted);

            input = @"C:\MyDir";
            Assert.IsTrue(new Path(input).IsPathRooted);

            input = @"C:MyDir";
            Assert.IsTrue(new Path(input).IsPathRooted);
            
            //It returns false for path strings such as "MyDir".
            input = @"MyDir";
            Assert.IsFalse(new Path(input).IsPathRooted);
        }

        [TestMethod]
        public void PathNormalizesDoubleSeperators()
        {
            //At least with Win32 I found that Double seperators in paths like c:\dir\\file.ext will fail to work (Win32 will not find an existing file due to the double seperator). Specifically I had the path 'C:\src\lessmsi\src\Lessmsi.Tests\bin\Debug\TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi' where the file existed but failed to get a valid handle ot it due to the double slash.
            var p = new Path(@"c:\dir\\file.ext");
            Assert.AreEqual(@"c:\dir\file.ext", p.PathString);
        }

        [TestMethod]
        public void PathWithDoubleSeperatorShouldStillWorkInFileSystem()
        {
            LessIO.Path p = GetTestPath(@"oneLevel\\test.txt");
            Assert.IsTrue(FileSystem.Exists(p), "Path with duplicate seperator should still be found");
        }
    }
}
