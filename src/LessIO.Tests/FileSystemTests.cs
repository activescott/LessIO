using Xunit;
using System.Diagnostics;
using System.Linq;

namespace LessIO.Tests
{
    public class FileSystemTests : TestBase
    {
        [Fact]
        public void ListContentsEmptyDir()
        {
            Path p = GetTestPath(@"emptyDir");
            var contents = FileSystem.ListContents(p);
            AssertEx.IsEmpty(contents);
        }

        [Fact]
        public void ListContentsOneLevel()
        {
            Path p = GetTestPath(@"oneLevel");
            var actual = FileSystem.ListContents(p).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"oneLevel\test - Copy.txt"),
                GetTestPath(@"oneLevel\test.txt"),
            };
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ListContentsTwoLevels()
        {
            Path p = GetTestPath(@"twoLevels");
            var actual = FileSystem.ListContents(p).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"twoLevels\secondLevel"),
                GetTestPath(@"twoLevels\test.txt")
            };
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ListContentsTwoLevelsRecursive()
        {
            Path p = GetTestPath(@"twoLevels");
            var actual = FileSystem.ListContents(p, true).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"twoLevels\secondLevel"),
                GetTestPath(@"twoLevels\test.txt"),
                GetTestPath(@"twoLevels\secondLevel\test (2).txt"),
                GetTestPath(@"twoLevels\secondLevel\test.txt"),
            };
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ListContentsTwoLevelsNotRecursive()
        {
            Path p = GetTestPath(@"twoLevels");
            var actual = FileSystem.ListContents(p, false).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"twoLevels\secondLevel"),
                GetTestPath(@"twoLevels\test.txt")
            };
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ListContentsWithSpaces()
        {
            Path p = GetTestPath(@"with spaces");
            var actual = FileSystem.ListContents(p).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"with spaces\file with spaces.txt"),
            };
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ListContentsLongPath()
        {
            //NOTE: windows is terrible with a path this long so even git won't allow committing a file with a path this long on windows. So we create it here:
            Path initTestFile = GetTestPath(@"long-path\long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping\long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\toppinglong-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping\test.txt");
            if (!FileSystem.Exists(initTestFile))
            {
                FileSystem.CreateDirectory(initTestFile.Parent);
                FileSystem.Copy(GetTestPath(@"test.txt"), initTestFile);
            }

            //NOW onto the test...
            Path p = GetTestPath(@"long-path\long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping\long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\toppinglong-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping\");
            var actual = FileSystem.ListContents(p).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"long-path\long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping\long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\toppinglong-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping\test.txt"),
            };
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ListContentsAllLevelsWithoutFiles()
        {
            Path p = GetTestPath(@"allLevelsWithoutFiles");
            var actual = FileSystem.ListContents(p, true).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"allLevelsWithoutFiles\secondLevel"),
                GetTestPath(@"allLevelsWithoutFiles\secondLevel\thirdLevel"),
            };
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ListContentsSomeLevelsWithoutFiles()
        {
            Path p = GetTestPath(@"someLevelsWithoutFiles");
            var actual = FileSystem.ListContents(p, true).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"someLevelsWithoutFiles\secondLevel"),
                GetTestPath(@"someLevelsWithoutFiles\secondLevel\thirdLevel"),
                GetTestPath(@"someLevelsWithoutFiles\secondLevel\thirdLevel\test.txt"),
            };
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ListContentsFiveLevels()
        {
            Path p = GetTestPath(@"fiveLevels");
            var actual = FileSystem.ListContents(p, true).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"fiveLevels\one"),
                GetTestPath(@"fiveLevels\one\two"),
                GetTestPath(@"fiveLevels\one\two\three"),
                GetTestPath(@"fiveLevels\one\two\three\four"),
                GetTestPath(@"fiveLevels\one\two\three\four\5.1"),
                GetTestPath(@"fiveLevels\one\two\three\four\5.2"),
                GetTestPath(@"fiveLevels\one\two\three\four\5.1\test1.txt"),
                GetTestPath(@"fiveLevels\one\two\three\four\5.2\test2.txt"),
            };
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Based on https://github.com/activescott/lessmsi/issues/70
        /// </summary>
        [Fact]
        public void CreateDirectoryOnSubstDrive()
        {
            //This is disgusting to do in a test and risky to cleanup, but I'll clean it up later (maybe)

            //Map drive:
            SubstDrive(false);

            // run test:
            var testPath = new Path(@"t:\CreateDirectoryOnSubstDrive");
            try
            {
                // The test is to make sure that this succeeds and doesn't throw:
                FileSystem.CreateDirectory(testPath);
                Assert.True(FileSystem.Exists(testPath));
            }
            finally
            {
                if (FileSystem.Exists(testPath))
                    FileSystem.RemoveDirectory(testPath);
                //cleanup the subst drive
                SubstDrive(true);
            }
        }

        private void SubstDrive(bool remove)
        {
            var subst = new Process();
            subst.StartInfo.FileName = @"C:\Windows\System32\subst.exe";
            subst.StartInfo.Arguments = remove ? "/D T:" : "T: " + GetTestPath("");
            subst.StartInfo.UseShellExecute = false;
            subst.StartInfo.RedirectStandardError = true;
            subst.StartInfo.RedirectStandardOutput = true;
            var started = subst.Start();
            Debug.Assert(started);
            subst.WaitForExit();
            Debug.Print("Subst output:");
            //because -1 means' already substed??
            if (0 != subst.ExitCode)
            {
                var stderr = subst.StandardError.ReadToEnd();
                var stdout = subst.StandardOutput.ReadToEnd();
                throw new System.Exception("subst failed:" + stdout + "\r\n" + stderr);
            }
        }
    }
}
