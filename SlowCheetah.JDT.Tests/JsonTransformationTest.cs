// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT.Tests
{
    using System.IO;
    using System.Text;
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
            string transformString = @"{ '@jdt.invalid': false }";
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {string.Format(Resources.ErrorMessage_InvalidVerb, "invalid")} Transform 1 17");

            this.TransformFailTest(sourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
        }

        [Fact]
        public void InvalidAttribute()
        {
            string sourceString = @"{ 'A': 1 }";
            string transformString = @"{ '@jdt.replace': { '@jdt.invalid': false } }";
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {string.Format(Resources.ErrorMessage_InvalidAttribute, "invalid")} Transform 1 35");

            this.TransformFailTest(sourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
        }

        public void MixedAttributes()
        {
        }

        public void MissingAttribute()
        {
        }

        public void WrongAttribute()
        {
            string sourceString = @"{ 'A': 1 }";
            string transformString = @"{ '@jdt.remove': { '@jdt.path': false } }";
            StringBuilder errorLogContent = new StringBuilder();
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

                Assert.Equal(errorLogContent, this.logger.ErrorLogText);
                Assert.Equal(warningLogContent, this.logger.WarningLogText);
                Assert.Equal(messageLogContent, this.logger.MessageLogText);
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
