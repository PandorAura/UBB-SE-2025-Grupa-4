using App1.AutoChecker;
using App1.Models;
using App1.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ReviewChecker.AuxiliaryTestsClasses
{

    public class MethodSwapper : IDisposable
    {
        private readonly RuntimeMethodHandle _originalMethodHandle;
        private readonly RuntimeMethodHandle _replacementMethodHandle;

        public MethodSwapper(Type originalType, string originalMethodName, Type replacementType, string replacementMethodName)
        {
            MethodInfo? originalMethod = originalType.GetMethod(originalMethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            MethodInfo? replacementMethod = replacementType.GetMethod(replacementMethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            _originalMethodHandle = originalMethod.MethodHandle;
            _replacementMethodHandle = replacementMethod.MethodHandle;
        }

        public void Dispose()
        {
            // Restore original method
            // Again, this is simplified and would require IL manipulation
        }
    }
}
