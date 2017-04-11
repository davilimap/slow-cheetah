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

        /// <summary>
        /// Tests the failiure caused when an invalid verb is found
        /// </summary>
        [Fact]
        public void InvalidVerb()
        {
            string sourceString = @"{ 'A': 1 }";
            string transformString = @"{ 
                                         '@jdt.invalid': false 
                                       }";

            // The error position should be the end of the invalid property key
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {string.Format(Resources.ErrorMessage_InvalidVerb, "invalid")} Transform 2 56");

            this.TransformFailTest(sourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
        }

        /// <summary>
        /// Tests the failiure caused by a verb having an invalid value
        /// </summary>
        [Fact]
        public void InvalidVerbValue()
        {
            string sourceString = @"{ 'A': 1 }";
            string transformString = @"{ 
                                         '@jdt.remove': 10 
                                       }";

            // The error position should be the end of the invalid token
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {string.Format(Resources.ErrorMessage_InvalidRemoveValue, "Integer")} Transform 2 58");

            this.TransformFailTest(sourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
        }

        /// <summary>
        /// Tests the failiure when an invalid attribute is found within a verb
        /// </summary>
        [Fact]
        public void InvalidAttribute()
        {
            string sourceString = @"{ 'A': 1 }";
            string transformString = @"{ 
                                         '@jdt.replace': { 
                                           '@jdt.invalid': false 
                                         } 
                                       }";

            // The error position should be at the end of the invalid attribute key
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {string.Format(Resources.ErrorMessage_InvalidAttribute, "invalid")} Transform 3 58");

            this.TransformFailTest(sourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
        }

        /// <summary>
        /// Tests the failiure when a required attribute is not found
        /// </summary>
        [Fact]
        public void MissingAttribute()
        {
            string sourceString = @"{ 'A': 1 }";
            string transformString = @"{ 
                                         '@jdt.rename': { 
                                           '@jdt.path': 'A' 
                                         } 
                                       }";

            // The error should point to the beginning of the verb object
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {Resources.ErrorMessage_RenameAttributes} Transform 2 57");

            this.TransformFailTest(sourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
        }

        /// <summary>
        /// Tests the failiure when an attribute has an incorrect value
        /// </summary>
        [Fact]
        public void WrongAttributeValue()
        {
            string sourceString = @"{ 'A': 1 }";
            string transformString = @"{
                                         '@jdt.remove': { 
                                           '@jdt.path': false
                                         } 
                                       }";

            // The error should point to the end of the invalid token
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {Resources.ErrorMessage_PathContents} Transform 3 61");

            this.TransformFailTest(sourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
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
