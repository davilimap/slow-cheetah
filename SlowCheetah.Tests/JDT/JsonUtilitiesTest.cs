// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.Tests.JDT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
        public void GetJdtVerbNotVerb(string key)
        {
            Assert.Equal(JsonUtilities.GetJdtVerb(key), JdtVerbs.None);
        }

        [Theory]
        [InlineData("")]
        [InlineData("string")]
        [InlineData("@JDTVerb")]
        [InlineData("JDT.Verb")]
        public void GetJdtVerbInvalidVerb(string key)
        {
            Assert.Equal(JsonUtilities.GetJdtVerb(key), JdtVerbs.None);
        }

        public void GetJdtVerbValidVerbs()
        {
        }
    }
}
