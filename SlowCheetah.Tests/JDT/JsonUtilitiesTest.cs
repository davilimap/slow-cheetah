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
        [InlineData("@JDTVerb")]
        [InlineData("JDT.Verb")]
        public void IsJdtSyntaxInvalid(string key)
        {
            Assert.False(JsonUtilities.IsJdtSyntax(key));
        }

        [Theory]
        [InlineData("@JDT.NotAVerb")]
        [InlineData("@JDT.Remove")]
        [InlineData("@JDT.merge")]
        [InlineData("@JDT.")]
        [InlineData("@JDT.  ")]
        public void IsJdtSyntaxValid(string key)
        {
            Assert.True(JsonUtilities.IsJdtSyntax(key));
        }

        [Theory]
        [InlineData("@JDT.NotAVerb")]
        [InlineData("@JDT.NotRemove")]
        [InlineData("@JDT.ReplaceNot")]
        [InlineData("@JDT.Renam")]
        [InlineData("@JDT.Value")]
        [InlineData("@JDT.path")]
        [InlineData("@JDT.")]
        [InlineData("@JDT. ")]
        public void GetJdtVerbInvalidVerb(string key)
        {
            Assert.Equal(JsonUtilities.GetJdtVerb(key), JdtVerbs.Invalid);
        }

        [Fact]
        public void GetJdtVerbValidVerbs()
        {
            // Rename
            Assert.Equal(JsonUtilities.GetJdtVerb("@JDT.Rename"), JdtVerbs.Rename);
            Assert.Equal(JsonUtilities.GetJdtVerb("@jdt.RENAME"), JdtVerbs.Rename);

            // Remove
            Assert.Equal(JsonUtilities.GetJdtVerb("@JDT.Remove"), JdtVerbs.Remove);
            Assert.Equal(JsonUtilities.GetJdtVerb("@Jdt.remove"), JdtVerbs.Remove);

            // Replace
            Assert.Equal(JsonUtilities.GetJdtVerb("@JDT.Replace"), JdtVerbs.Replace);
            Assert.Equal(JsonUtilities.GetJdtVerb("@JdT.RepLacE"), JdtVerbs.Replace);

            // Merge
            Assert.Equal(JsonUtilities.GetJdtVerb("@JDT.Merge"), JdtVerbs.Merge);
            Assert.Equal(JsonUtilities.GetJdtVerb("@jdt.merge"), JdtVerbs.Merge);
        }

        [Theory]
        [InlineData("@JDT.NotAProp")]
        [InlineData("@JDT.NotPath")]
        [InlineData("@JDT.ValueNot")]
        [InlineData("@JDT.Val")]
        [InlineData("@JDT.merge")]
        [InlineData("@JDT.Remove")]
        [InlineData("@JDT.")]
        [InlineData("@JDT. ")]
        public void GetJdtPropertyInvalidProperty(string key)
        {
            Assert.Equal(JsonUtilities.GetJdtProperty(key), JdtProperties.Invalid);
        }

        [Fact]
        public void GetJdtPropertyValidProperties()
        {
            // Path
            Assert.Equal(JsonUtilities.GetJdtProperty("@JDT.Path"), JdtProperties.Path);
            Assert.Equal(JsonUtilities.GetJdtProperty("@jdt.PATH"), JdtProperties.Path);

            // Value
            Assert.Equal(JsonUtilities.GetJdtProperty("@JDT.Value"), JdtProperties.Value);
            Assert.Equal(JsonUtilities.GetJdtProperty("@Jdt.value"), JdtProperties.Value);
        }
    }
}
