using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            Assert.IsTrue(caught != null && expectedExceptionType.IsInstanceOfType(caught), "Expected exception '{0}' but received '{1}'.", expectedExceptionType, caught);
        }

        internal static void IsEmpty<TElement>(IEnumerable<TElement> contents)
        {
            Assert.IsNotNull(contents, "Expected empty enumerable, but it was null.");
            Assert.IsTrue(0 == Enumerable.Count(contents), "Expected empty enumerable, but it didn't have zero elements.");
        }
    }
}
