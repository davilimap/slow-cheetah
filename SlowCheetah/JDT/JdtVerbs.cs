// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    /// <summary>
    /// The possible tranformations that JDT executes
    /// </summary>
    internal enum JdtVerbs
    {
        /// <summary>
        /// Represents invalid verbs
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents the Remove transformation
        /// </summary>
        Remove = 1,

        /// <summary>
        /// Represents the Replace transformation
        /// </summary>
        Replace = 2,

        /// <summary>
        /// Represents the Rename transformation
        /// </summary>
        Rename = 3,

        /// <summary>
        /// Represents the Merge transformation
        /// </summary>
        Merge = 4
    }
}
