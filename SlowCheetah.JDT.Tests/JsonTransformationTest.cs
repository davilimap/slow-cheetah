// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT.Tests
{
    using System.IO;
    using Xunit;

    /// <summary>
    /// Test class for <see cref="JsonTransformation"/>
    /// </summary>
    public class JsonTransformationTest
    {
        private JsonTransformationTestLogger logger = new JsonTransformationTestLogger();

        [Fact]
        public void InvalidVerb()
        {
            string sourceString = @"{ 'A': 1 }";
            string transformString = @"{ 
                                        '@jdt.invalid': false 
                                        }";
            string errorLogContent = "Exception: invalid is not a valid JDT verb Transform 0 0\r\n";

            this.TransformFailTest(sourceString, transformString, errorLogContent, string.Empty, string.Empty);
        }

        [Fact]
        public void InvalidAttribute()
        {
            string sourceString = @"{ 'A': 1 }";
            string transformString = @"{ '@jdt.replace': { '@jdt.invalid': false } }";
            string errorLogContent = "Exception: @jdt.invalid is not a valid attribute for this transformation Transform 0 0\r\n";

            this.TransformFailTest(sourceString, transformString, errorLogContent, string.Empty, string.Empty);
        }

        private void TransformFailTest(string sourceString, string transformString, string errorLogContent, string warningLogContent, string messageLogContent)
        {
            using (var transformStream = this.GetStreamFromString(transformString))
            using (var sourceStream = this.GetStreamFromString(sourceString))
            {
                JsonTransformation transform = new JsonTransformation(transformStream, this.logger);
                Stream result;
                Assert.False(transform.TryApply(sourceStream, out result));
                Assert.Null(result);

                Assert.Equal(this.logger.ErrorLogText, errorLogContent);
                Assert.Equal(this.logger.WarningLogText, warningLogContent);
                Assert.Equal(this.logger.MessageLogText, messageLogContent);
            }
        }

        private Stream GetStreamFromString(string s)
        {
            MemoryStream stringStream = new MemoryStream();
            StreamWriter stringWriter = new StreamWriter(stringStream);
            stringWriter.Write(s);
            stringWriter.Flush();
            stringStream.Position = 0;

            return stringStream;
        }
    }
}
