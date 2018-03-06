using System;
using System.IO;
using Newtonsoft.Json;
using TestHelpers.DiffAssertions;

namespace Comments.Test.Infrastructure
{
    public static class Extensions
    {
        public static void ShouldMatchApproved(this string stringIn,
                [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
                [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {

            //prettifying.
            if (stringIn.StartsWith("{")) //we'll assume it's JSON.
            {
                try
                {
                    var parsedJson = JsonConvert.DeserializeObject(stringIn);
                    stringIn = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
                }
                catch (Exception) {}
            }
            else if (stringIn.StartsWith("<")) //we'll assume it's HTML. 
            {

            }

            var expectedFilePath = Path.GetDirectoryName(sourceFilePath);
            var callingFileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFilePath); //usually also the class name of the caller.
            var expectedFileName = $"{callingFileNameWithoutExtension}.{memberName}";

            var fileToOpen = Path.Combine(expectedFilePath, expectedFileName);

            DiffAssert.ThatExpectedFileContentsEqualsActualValue(fileToOpen, stringIn);
        }        
    }
}
