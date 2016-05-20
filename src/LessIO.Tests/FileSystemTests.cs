using System;
using SysPath = System.IO.Path;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace LessIO.Tests
{
    [TestClass]
    public class FileSystemTests : TestBase
    {
        [TestMethod]
        public void ListContentsEmptyDir()
        {
            Path p = GetTestPath(@"emptyDir");
            var contents = FileSystem.ListContents(p);
            AssertEx.IsEmpty(contents);
        }

        [TestMethod]
        public void ListContentsOneLevel()
        {
            Path p = GetTestPath(@"oneLevel");
            var actual = FileSystem.ListContents(p).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"oneLevel\test - Copy.txt"),
                GetTestPath(@"oneLevel\test.txt"),
            };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void ListContentsTwoLevels()
        {
            Path p = GetTestPath(@"twoLevels");
            var actual = FileSystem.ListContents(p).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"twoLevels\secondLevel"),
                GetTestPath(@"twoLevels\test.txt")
            };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void ListContentsTwoLevelsRecursive()
        {
            Path p = GetTestPath(@"twoLevels");
            var actual = FileSystem.ListContents(p, true).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"twoLevels\test.txt"),
                GetTestPath(@"twoLevels\secondLevel"),
                GetTestPath(@"twoLevels\secondLevel\test (2).txt"),
                GetTestPath(@"twoLevels\secondLevel\test.txt"),
            };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void ListContentsTwoLevelsNotRecursive()
        {
            Path p = GetTestPath(@"twoLevels");
            var actual = FileSystem.ListContents(p, false).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"twoLevels\secondLevel"),
                GetTestPath(@"twoLevels\test.txt")
            };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void ListContentsWithSpaces()
        {
            Path p = GetTestPath(@"with spaces");
            var actual = FileSystem.ListContents(p).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"with spaces\file with spaces.txt"),
            };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void ListContentsLongPath()
        {
            Path p = GetTestPath(@"long-path\long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping\long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\toppinglong-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping\");
            var actual = FileSystem.ListContents(p).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"long-path\long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping\long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\toppinglong-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping\test.txt"),
            };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void ListContentsLevelsWithoutFiles()
        {
            Path p = GetTestPath(@"levelsWithoutFiles");
            var actual = FileSystem.ListContents(p, true).ToArray();
            var expected = new Path[]
            {
                GetTestPath(@"levelsWithoutFiles\secondLevel"),
                GetTestPath(@"levelsWithoutFiles\secondLevel\thirdLevel"),
            };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
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
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
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
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
