// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a recursive JDT transformation
    /// </summary>
    internal class JdtReplace : JdtProcessor
    {
        /// <inheritdoc/>
        public override string Verb { get; } = "replace";

        /// <inheritdoc/>
        public override void Process(JObject source, JObject transform)
        {
            throw new NotImplementedException();
        }
    }
}
