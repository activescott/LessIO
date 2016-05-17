using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LessIO.Tests
{
    internal static class AssertEx
    {
        public static void Throws(Type expectedExceptionType, Action action)
        {
            if (!typeof(Exception).IsAssignableFrom(expectedExceptionType))
                throw new ArgumentException("Must be a type of Exception.", "exceptionType");

            Exception caught = null;
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                caught = e;
            }
            Assert.IsTrue(caught != null && expectedExceptionType.IsInstanceOfType(caught), "Expected exception '{0}' but received '{1}'.", expectedExceptionType, caught);
        }
    }
}
