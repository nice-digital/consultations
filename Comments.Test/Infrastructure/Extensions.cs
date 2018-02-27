using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using TestHelpers.DiffAssertions;
using Xunit;

namespace Comments.Test.Infrastructure
{
    public static class Extensions
    {
        public static void ShouldMatchApproved(this string stringIn,
                [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
                [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            var expectedFilePath = Path.GetDirectoryName(sourceFilePath);
            var callingFileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFilePath); //usually also the class name of the caller.
            var expectedFileName = $"{callingFileNameWithoutExtension}.{memberName}.txt";

            var fileToOpen = Path.Combine(expectedFilePath, expectedFileName);

            DiffAssert.ThatExpectedFileContentsEqualsActualValue(fileToOpen, stringIn);
        }        
    }
}
