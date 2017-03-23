// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.Tests.JDT
{
    using SlowCheetah.JDT;
    using Xunit;

    public class JsonUtilitiesTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("string")]
        [InlineData("jdt.Verb")]
        [InlineData("@jdtverb")]
        [InlineData("@jdt")]
        [InlineData("@JDT.WrongCase")]
        public void IsJdtSyntaxInvalid(string key)
        {
            Assert.False(JsonUtilities.IsJdtSyntax(key));
        }

        [Theory]
        [InlineData("@jdt.NotAVerb")]
        [InlineData("@jdt.Remove")]
        [InlineData("@jdt.merge")]
        [InlineData("@jdt.")]
        [InlineData("@jdt.  ")]
        public void IsJdtSyntaxValid(string key)
        {
            Assert.True(JsonUtilities.IsJdtSyntax(key));
        }
    }
}
