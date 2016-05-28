using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

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
            Assert.True(caught != null && expectedExceptionType.IsInstanceOfType(caught), string.Format("Expected exception '{0}' but received '{1}'.", expectedExceptionType, caught));
        }

        internal static void IsEmpty<TElement>(IEnumerable<TElement> contents)
        {
            Assert.NotNull(contents);
            Assert.True(0 == Enumerable.Count(contents), "Expected empty enumerable, but it didn't have zero elements.");
        }
    }
}
