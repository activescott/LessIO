using Xunit;

namespace LessIO.Tests
{
    public class PathEquality
    {
        const string Win32LongPathPrefix = @"\\?\";
        [Fact]
        public void EqualSimple()
        {
            Path a = new Path(@"c:\aroot\aparent");
            Path b = new Path(@"c:\aroot\aparent");
            TestEquality(a, b, true);
        }

        [Fact]
        public void EqualDespiteCase()
        {
            Path a = new Path(@"c:\aroot\aparent");
            Path b = new Path(@"C:\AROOT\APARENT");
            TestEquality(a, b, true);
        }

        [Fact]
        public void NotEqualSimple()
        {
            Path a = new Path(@"c:\aroot\aparent");
            Path b = new Path(@"c:\aroot\bparent");
            TestEquality(a, b, false);
        }

        [Fact]
        public void EqualTrailingPath()
        {
            Path a = new Path(@"c:\aroot\aparent");
            Path b = new Path(@"c:\aroot\aparent\");
            TestEquality(a, b, true);
        }

        [Fact]
        public void EqualDespiteWin32Prefix()
        {
            Path a = new Path(@"c:\aroot\aparent");
            Path b = new Path(Win32LongPathPrefix + @"c:\aroot\aparent");
            TestEquality(a, b, true);
        }

        [Fact]
        public void NotEqualWin32Prefix()
        {
            Path a = new Path(@"c:\aroot\aparent");
            Path b = new Path(Win32LongPathPrefix + @"c:\aroot\bparent");
            TestEquality(a, b, false);
        }

        [Fact]
        public void NotEqualToEmpty()
        {
            Path a = new Path(@"c:\aroot\aparent");
            TestEquality(a, Path.Empty, false);
        }

        [Fact]
        public void NotEqualToNull()
        {
            Path a = new Path(@"c:\aroot\aparent");
            Assert.False(a.Equals(null));
            Assert.False(object.Equals(a, null));
        }

        private void TestEquality(Path a, Path b, bool areEqual)
        {
            if (areEqual)
                Assert.Equal(a, b);
            else
                Assert.NotEqual(a, b);

            Assert.True(areEqual == (a == b));
            Assert.True(areEqual != (a != b));
            Assert.True(areEqual == a.Equals(b));
            Assert.True(areEqual == b.Equals(a));
            Assert.True(areEqual == object.Equals(a, b));
        }
    }
}
