using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LessIO.Tests
{
    [TestClass]
    public class PathTests
    {
        const string Win32LongPathPrefix = @"\\?\";
        private void TestGetParentPath(string expected, string testInput)
        {
            var actual = new Path(testInput).Parent.FullName;
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
        public void EqualSimple()
        {
            Path a = new Path(@"c:\aroot\aparent");
            Path b = new Path(@"c:\aroot\aparent");
            TestEquality(a, b, true);
        }

        [TestMethod]
        public void EqualDespiteCase()
        {
            Path a = new Path(@"c:\aroot\aparent");
            Path b = new Path(@"C:\AROOT\APARENT");
            TestEquality(a, b, true);
        }

        [TestMethod]
        public void NotEqualSimple()
        {
            Path a = new Path(@"c:\aroot\aparent");
            Path b = new Path(@"c:\aroot\bparent");
            TestEquality(a, b, false);
        }

        [TestMethod]
        public void EqualTrailingPath()
        {
            Path a = new Path(@"c:\aroot\aparent");
            Path b = new Path(@"c:\aroot\aparent\");
            TestEquality(a, b, true);
        }

        [TestMethod]
        public void EqualDespiteWin32Prefix()
        {
            Path a = new Path(@"c:\aroot\aparent");
            Path b = new Path(Win32LongPathPrefix + @"c:\aroot\aparent");
            TestEquality(a, b, true);
        }

        [TestMethod]
        public void NotEqualWin32Prefix()
        {
            Path a = new Path(@"c:\aroot\aparent");
            Path b = new Path(Win32LongPathPrefix + @"c:\aroot\bparent");
            TestEquality(a, b, false);
        }

        [TestMethod]
        public void NotEqualToEmpty()
        {
            Path a = new Path(@"c:\aroot\aparent");
            TestEquality(a, Path.Empty, false);
        }

        [TestMethod]
        public void NotEqualToNull()
        {
            Path a = new Path(@"c:\aroot\aparent");
            Assert.AreNotEqual(a, null);
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(object.Equals(a, null));
        }

        private void TestEquality(Path a, Path b, bool areEqual)
        {
            if (areEqual)
                Assert.AreEqual(a, b);
            else
                Assert.AreNotEqual(a, b);

            Assert.IsTrue(areEqual == (a == b));
            Assert.IsTrue(areEqual != (a != b));
            Assert.IsTrue(areEqual == a.Equals(b));
            Assert.IsTrue(areEqual == b.Equals(a));
            Assert.IsTrue(areEqual == object.Equals(a, b));
        }
    }
}
