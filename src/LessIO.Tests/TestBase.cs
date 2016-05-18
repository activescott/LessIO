using System;
using SysPath = System.IO.Path;

namespace LessIO.Tests
{
    public class TestBase
    {
        protected string AppPath
        {
            get
            {
                var codeBase = new Uri(this.GetType().Assembly.CodeBase);
                return SysPath.GetDirectoryName(codeBase.LocalPath);
            }
        }

        /// <summary>
        /// Gets the specified test dir
        /// </summary>
        /// <param name="testPath">A relative name of the test dir.</param>
        protected Path GetTestPath(string testPath)
        {
            // NOTE: We don't copy the TestFiles into a safe directory because almost nothing on windows can deal with the long-path directory's extreme length.
            var bin = SysPath.GetDirectoryName(AppPath);
            var project = SysPath.GetDirectoryName(bin);
            var testFiles = SysPath.Combine(project, "TestFiles");
            return new Path(SysPath.Combine(testFiles, testPath));
        }
    }
}
