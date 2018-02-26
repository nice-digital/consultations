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
                [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
                [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {

            var expectedFilePath = Path.GetDirectoryName(sourceFilePath);
            var callingFileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFilePath); //usually also the class name of the caller.
            var expectedFileName = $"{callingFileNameWithoutExtension}.{memberName}.txt";

            var fileToOpen = Path.Combine(expectedFilePath, expectedFileName);

            DiffAssert.ThatExpectedFileContentsEqualsActualValue(fileToOpen, stringIn);

            //if (File.Exists(fileToOpen))
            //{
            //    string textInFile;
            //    using (StreamReader streamReader = new StreamReader(fileToOpen))
            //    {
            //        textInFile = streamReader.ReadToEnd();
            //    }

            //   // Assert.Equal(0, String.Compare(textInFile, stringIn, CompareOptions.IgnoreSymbols));

            //    DiffAssert.Equals(textInFile, stringIn);

            //}
            //else
            //{
            //    DiffAssert.th
            //}



           

            


        }

        
    }
}
