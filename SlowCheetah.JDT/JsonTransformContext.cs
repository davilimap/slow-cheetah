// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Context of a JSON transformation
    /// </summary>
    internal class JsonTransformContext
    {
        /// <summary>
        /// Gets or sets the source file of the current transformation
        /// </summary>
        internal string SourceFile { get; set; } = null;

        /// <summary>
        /// Gets or sets the transformation file of the current transformation
        /// </summary>
        internal string TransformFile { get; set; } = null;

        /// <summary>
        /// Gets or sets the logger for the current transformation
        /// </summary>
        internal JsonTransformationLogger Logger { get; set; } = null;
    }
}
