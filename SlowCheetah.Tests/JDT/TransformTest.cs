// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.Tests.JDT
{
    using System.Collections.Generic;
    using System.IO;
    using SlowCheetah.JDT;
    using Xunit;

    /// <summary>
    /// Test class for JDT default transforms
    /// </summary>
    public class TransformTest
    {
        // Directory for test inputs, that are JSON files
        private static readonly string TestInputDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\SlowCheetah.Tests\\JDT\\Inputs\\";

        // Directory for Default transformation tests
        private static readonly string DefaultTestDirectory = TestInputDirectory + "Default\\";

        // Directory for Remove transformation test
        private static readonly string RemoveTestDirectory = TestInputDirectory + "Remove\\";

        // Directory for Rename transformation test
        private static readonly string RenameTestDirectory = TestInputDirectory + "Rename\\";

        // Directory for Replace transformation test
        private static readonly string ReplaceTestDirectory = TestInputDirectory + "Replace\\";

        // Directory for Merge transformation test
        private static readonly string MergeTestDirectory = TestInputDirectory + "Merge\\";

        public static IEnumerable<object[]> GetDefaultInputs
        {
            get
            {
                // Gets inputs from Default transformation test directory
                return GetInputs(DefaultTestDirectory);
            }
        }

        public static IEnumerable<object[]> GetRemoveInputs
        {
            get
            {
                return GetInputs(RemoveTestDirectory);
            }
        }

        public static IEnumerable<object[]> GetRenameInputs
        {
            get
            {
                return GetInputs(RenameTestDirectory);
            }
        }

        public static IEnumerable<object[]> GetReplaceInputs
        {
            get
            {
                return GetInputs(ReplaceTestDirectory);
            }
        }

        public static IEnumerable<object[]> GetMergeInputs
        {
            get
            {
                return GetInputs(MergeTestDirectory);
            }
        }

        [Theory]
        [MemberData(nameof(GetDefaultInputs))]
        public void Default(string testName)
        {
            BaseTransformTest(DefaultTestDirectory, testName);
        }

        [Theory]
        [MemberData(nameof(GetRemoveInputs))]
        public void Remove(string testFileName)
        {
            BaseTransformTest(RemoveTestDirectory, testFileName);
        }

        [Theory]
        [MemberData(nameof(GetRenameInputs))]
        public void Rename(string testFileName)
        {
            BaseTransformTest(RenameTestDirectory, testFileName);
        }

        [Theory]
        [MemberData(nameof(GetReplaceInputs))]
        public void Replace(string testFileName)
        {
            BaseTransformTest(ReplaceTestDirectory, testFileName);
        }

        [Theory]
        [MemberData(nameof(GetMergeInputs))]
        public void Merge(string testFileName)
        {
            BaseTransformTest(MergeTestDirectory, testFileName);
        }

        private static IEnumerable<object[]> GetInputs(string testDirectory)
        {
            // Each transform file in the test input folder will correspond to one test
            foreach (string file in Directory.GetFiles(testDirectory, "*.Transform.json"))
            {
                // Transform files are called {TestCategory}.{TestName}.Transform.json
                // Expected results files are called {TestCategory}.{TestName}.Expected.json
                // Source files are called {TestCategory}.Source.json
                // This returns {TestCategory}.{TestName} so the test can find the files
                yield return new object[] { Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file)) };
            }
        }

        private static void BaseTransformTest(string inputsDirectory, string testName)
        {
            // Removes the test name to find the source file
            string sourceName = Path.GetFileNameWithoutExtension(testName);

            JsonDocument source = new JsonDocument(inputsDirectory + sourceName + ".Source.json");

            JsonTransformation transformation = new JsonTransformation(inputsDirectory + testName + ".Transform.json");

            JsonDocument expected = new JsonDocument(inputsDirectory + testName + ".Expected.json");

            transformation.Apply(source);

            Assert.True(source.Equals(expected));
        }
    }
}
