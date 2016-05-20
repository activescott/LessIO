using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LessIO.Tests
{
    [TestClass]
    public class PathEquality
    {
        const string Win32LongPathPrefix = @"\\?\";
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
