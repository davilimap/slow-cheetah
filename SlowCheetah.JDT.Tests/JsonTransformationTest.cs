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

        private string simpleSourceString = @"{ 'A': 1 }";

        /// <summary>
        /// Tests the error caused when an invalid verb is found
        /// </summary>
        [Fact]
        public void InvalidVerb()
        {
            string transformString = @"{ 
                                         '@jdt.invalid': false 
                                       }";

            // The error position should be the end of the invalid property key
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {string.Format(Resources.ErrorMessage_InvalidVerb, "invalid")} Transform 2 56");

            this.TryTransformTest(this.simpleSourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
        }

        /// <summary>
        /// Tests the error caused by a verb having an invalid value
        /// </summary>
        [Fact]
        public void InvalidVerbValue()
        {
            string transformString = @"{ 
                                         '@jdt.remove': 10 
                                       }";

            // The error position should be the end of the invalid token
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {string.Format(Resources.ErrorMessage_InvalidRemoveValue, "Integer")} Transform 2 58");

            this.TryTransformTest(this.simpleSourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
        }

        /// <summary>
        /// Tests the error caused when an invalid attribute is found within a verb
        /// </summary>
        [Fact]
        public void InvalidAttribute()
        {
            string transformString = @"{ 
                                         '@jdt.replace': { 
                                           '@jdt.invalid': false 
                                         } 
                                       }";

            // The error position should be at the end of the invalid attribute key
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {string.Format(Resources.ErrorMessage_InvalidAttribute, "invalid")} Transform 3 58");

            this.TryTransformTest(this.simpleSourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
        }

        /// <summary>
        /// Tests the error caused when a required attribute is not found
        /// </summary>
        [Fact]
        public void MissingAttribute()
        {
            string transformString = @"{ 
                                         '@jdt.rename': { 
                                           '@jdt.path': 'A' 
                                         } 
                                       }";

            // The error should point to the beginning of the verb object
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {Resources.ErrorMessage_RenameAttributes} Transform 2 57");

            this.TryTransformTest(this.simpleSourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
        }

        /// <summary>
        /// Tests the error caused when a verb object contains attributes and other objects
        /// </summary>
        [Fact]
        public void MixedAttributes()
        {
            string transformString = @"{ 
                                         '@jdt.rename': { 
                                           '@jdt.path': 'A',
                                           '@jdt.value': 'Astar',
                                           'NotAttribute': true
                                         } 
                                       }";

            // The error should point to the beginning of the verb object
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {Resources.ErrorMessage_InvalidAttributes} Transform 2 57");

            this.TryTransformTest(this.simpleSourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
        }

        /// <summary>
        /// Tests the error caused when an attribute has an incorrect value
        /// </summary>
        [Fact]
        public void WrongAttributeValue()
        {
            string transformString = @"{
                                         '@jdt.remove': { 
                                           '@jdt.path': false
                                         } 
                                       }";

            // The error should point to the end of the invalid token
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {Resources.ErrorMessage_PathContents} Transform 3 61");

            this.TryTransformTest(this.simpleSourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
        }

        public void RemoveNonExistantNode()
        {
            string transformString = @"{
                                         '@jdt.remove': { 
                                           '@jdt.path': 'B'
                                         } 
                                       }";

            // This should log a warning that the path has not been found
            StringBuilder warningLogContent = new StringBuilder();

            this.TryTransformTest(this.simpleSourceString, transformString, string.Empty, warningLogContent.ToString(), string.Empty);
        }

        /// <summary>
        /// Tests the error caused when attempting to remove the root node
        /// </summary>
        [Fact]
        public void RemoveRoot()
        {
            string transformString = @"{
                                         '@jdt.remove': true 
                                       }";

            // The error should point to the end of the invalid token
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {Resources.ErrorMessage_RemoveRoot} Transform 2 60");

            this.TryTransformTest(this.simpleSourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
        }

        /// <summary>
        /// Tests the error when a rename value is invalid
        /// </summary>
        [Fact]
        public void InvalidRenameValue()
        {
            string transformString = @"{
                                         '@jdt.rename': { 
                                           'A': 10
                                         } 
                                       }";

            // The error should point to the end of the invalid token
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {Resources.ErrorMessage_InvalidRenameValue} Transform 3 50");

            this.TryTransformTest(this.simpleSourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
        }

        /// <summary>
        /// Tests the error caused when attempting to rename a non-existant node
        /// </summary>
        [Fact]
        public void RenameNonExistantNode()
        {
            string transformString = @"{
                                         '@jdt.rename': { 
                                           'B': 'Bstar'
                                         } 
                                       }";

            // The error should point to the end of property key
            StringBuilder warningLogContent = new StringBuilder();
            warningLogContent.AppendLine($"{string.Format(Resources.WarningMessage_NodeNotFound, "B")} Transform 3 47");

            this.TryTransformTest(this.simpleSourceString, transformString, string.Empty, warningLogContent.ToString(), string.Empty);
        }

        /// <summary>
        /// Test the error when attempting to replace the root with a non-object token
        /// </summary>
        [Fact]
        public void ReplaceRoot()
        {
            string transformString = @"{
                                         '@jdt.replace': 10
                                       }";

            // The error should point to the end of the invalid token
            StringBuilder errorLogContent = new StringBuilder();
            errorLogContent.AppendLine($"Exception: {Resources.ErrorMessage_ReplaceRoot} Transform 2 59");

            this.TryTransformTest(this.simpleSourceString, transformString, errorLogContent.ToString(), string.Empty, string.Empty);
        }

        /// <summary>
        /// Tests that an exception is thrown when <see cref="JsonTransformation.Apply(Stream)"/> is called
        /// </summary>
        [Fact]
        public void ThrowAndLogException()
        {
            string transformString = @"{ 
                                         '@jdt.invalid': false 
                                       }";
            using (var transformStream = this.GetStreamFromString(transformString))
            using (var sourceStream = this.GetStreamFromString(this.simpleSourceString))
            {
                JsonTransformation transform = new JsonTransformation(transformStream, this.logger);
                var exception = Record.Exception(() => transform.Apply(sourceStream));
                Assert.NotNull(exception);
                Assert.IsType<JdtException>(exception);
                var jdtException = exception as JdtException;
                Assert.Equal(string.Format(Resources.ErrorMessage_InvalidVerb, "invalid"), jdtException.Message);
                Assert.Equal(jdtException.Location, ErrorLocation.Transform);
                Assert.Equal(jdtException.LineNumber, 2);
                Assert.Equal(jdtException.LinePosition, 56);
            }
        }

        private void TryTransformTest(string sourceString, string transformString, string errorLogContent, string warningLogContent, string messageLogContent)
        {
            using (var transformStream = this.GetStreamFromString(transformString))
            using (var sourceStream = this.GetStreamFromString(sourceString))
            {
                JsonTransformation transform = new JsonTransformation(transformStream, this.logger);
                Stream result;

                // If there should be errors, the transform should fail
                bool shouldTransformSucceed = string.IsNullOrEmpty(errorLogContent);

                Assert.Equal(shouldTransformSucceed, transform.TryApply(sourceStream, out result));
                if (shouldTransformSucceed)
                {
                    Assert.NotNull(result);
                }
                else
                {
                    Assert.Null(result);
                }

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
