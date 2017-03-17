﻿// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
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
    using Xunit.Extensions;

    /// <summary>
    /// Test class for JDT default transforms
    /// </summary>
    public class DefaultTransformTest : BaseJdtTest
    {
        [Fact]
        public void PrimitiveAddition()
        {
            this.BaseTransformTest(
                JdtTestUtilities.GetDocumentObjectWithPrimitives,
                JdtTestUtilities.GetTransformWithPrimitiveAdds,
                JdtTestUtilities.GetExpectedWithPrimitiveAdds);
        }

        [Fact]
        public void ObjectAddition()
        {
            this.BaseTransformTest(
                JdtTestUtilities.GetDocumentObjectWithPrimitives,
                JdtTestUtilities.GetTransformWithObjectAdds,
                JdtTestUtilities.GetExpectedWithObjectAdds);
        }

        [Fact]
        public void PrimitiveSubstitution()
        {
            this.BaseTransformTest(
                JdtTestUtilities.GetDocumentObjectWithPrimitives,
                JdtTestUtilities.GetTransformWithPrimitiveReplace,
                JdtTestUtilities.GetTransformWithPrimitiveReplace);
        }

        [Fact]
        public void ObjectMerge()
        {
            this.BaseTransformTest(
                JdtTestUtilities.GetDocumentWithObjects,
                JdtTestUtilities.GetTransformWithObjectMerge,
                JdtTestUtilities.GetExpectedWithObjectMerge);
        }

        public void ArrayMerge()
        {
        }

        public void LoadFromFile()
        {

        }

        public void SaveToFile()
        {
        }
    }
}
