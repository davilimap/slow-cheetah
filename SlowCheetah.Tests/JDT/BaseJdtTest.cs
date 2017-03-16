// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.Tests.JDT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using SlowCheetah.JDT;
    using Xunit;

    /// <summary>
    /// Base class for all JDT tests
    /// </summary>
    public class BaseJdtTest
    {
        public void BaseTransformTest(JObject sourceObj, JObject transformObj, JObject expectedObj)
        {
            JsonDocument source = new JsonDocument(sourceObj);

            JsonTransformation transformation = new JsonTransformation(transformObj);

            JsonDocument expectedResult = new JsonDocument(expectedObj);

            transformation.Apply(source);

            Assert.True(source.Equals(expectedResult));
        }
    }
}
