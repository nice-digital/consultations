using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TestHelpers.DiffAssertions;

namespace Comments.Test.Infrastructure
{
    public static class Extensions
    {
        public static void ShouldMatchApproved(this string stringIn,
                Func<string, string>[] scrubbers = null, //sadly we can't use params and stick it on the end since the following parameters need to be optional.
                [System.Runtime.CompilerServices.CallerMemberName] string memberName = null,
                [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = null)
        {
            //prettifying.
            if (stringIn.StartsWith("{") || stringIn.StartsWith("[{")) //we'll assume it's JSON.
            {
                try
                {
                    stringIn = Formatters.FormatJson(stringIn);
                }
                catch (Exception) {}
            }
            else if (stringIn.StartsWith("<")) //we'll assume it's HTML. 
            {
                try
                {
                    stringIn = Scrubbers.ScrubHashFromJavascriptFileName(stringIn);
                    stringIn = Formatters.FormatHtml(stringIn);
                }
                catch (Exception) { }
            }

            //scrubbing changing data such as the current time etc.
            if (scrubbers != null)
            {
                foreach (var scrub in scrubbers)
                {
                    stringIn = scrub(stringIn);
                }
            }

            var expectedFilePath = Path.GetDirectoryName(sourceFilePath);
            var callingFileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFilePath); //usually also the class name of the caller.
            var expectedFileName = $"{callingFileNameWithoutExtension}.{memberName}";

            var fileToOpen = Path.Combine(expectedFilePath, expectedFileName);

            DiffAssert.ThatExpectedFileContentsEqualsActualValue(fileToOpen, stringIn);
        }


        
    }
}
