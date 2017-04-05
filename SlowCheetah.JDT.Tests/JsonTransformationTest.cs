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
            string transformString = @"{ '@jdt.invalid': false }";
            using (var transformStream = this.GetStreamFromString(transformString))
            using (var sourceStream = this.GetStreamFromString(sourceString))
            {
                JsonTransformation transform = new JsonTransformation(transformStream, this.logger);
                Stream result;
                Assert.False(transform.TryApply(sourceStream, out result));
                Assert.Null(result);

                // TO DO: Compare logger contents
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
