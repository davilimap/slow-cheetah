// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.Tests.JDT
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using SlowCheetah.JDT;
    using Xunit;
    using Xunit.Extensions;

    /// <summary>
    /// Test class for JDT default transforms
    /// </summary>
    public class DefaultTransformTest
    {
        // Directory for test inputs, that are JSON files
        private static readonly string TestInputDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\SlowCheetah.Tests\\JDT\\Inputs\\";

        public static IEnumerable<object[]> GetInputs()
        {
            // Each transform file in the test input folder will correspond to one test
            foreach (string file in Directory.GetFiles(TestInputDirectory, "*.Transform.json"))
            {
                // Transform files are called {TestCategory}.{TestName}.Transform.json
                // Expected results files are called {TestCategory}.{TestName}.Expected.json
                // Source files are called {TestCategory}.Source.json
                // This returns {TestCategory}.{TestName} so the test can find the files
                yield return new object[] { Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file)) };
            }
        }

        [Theory]
        [MemberData(nameof(GetInputs))]
        public void TransformTest(string testFileName)
        {
            // Removes the test name to find the source file
            string sourceFileName = Path.GetFileNameWithoutExtension(testFileName);
            JsonDocument source = new JsonDocument(TestInputDirectory + sourceFileName + ".Source.json");

            JsonTransformation transformation = new JsonTransformation(TestInputDirectory + testFileName + ".Transform.json");

            JsonDocument expected = new JsonDocument(TestInputDirectory + testFileName + ".Expected.json");

            transformation.Apply(source);

            Assert.True(source.Equals(expected));
        }
    }
}
