using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Xunit;

namespace NeinLinq.Tests
{
    public sealed class WindowsFactAttribute : FactAttribute
    {
        public WindowsFactAttribute()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Skip = "For Windows only";
            }
        }
    }
}
