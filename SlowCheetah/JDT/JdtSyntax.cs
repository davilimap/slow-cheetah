// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    /// <summary>
    /// JDT transformation verbs
    /// </summary>
    internal enum JdtVerbs
    {
        /// <summary>
        /// Represents an invalid verb
        /// </summary>
        Invalid,

        /// <summary>
        /// Represents the Remove transformation
        /// </summary>
        Remove,

        /// <summary>
        /// Represents the Replace transformation
        /// </summary>
        Replace,

        /// <summary>
        /// Represents the Rename transformation
        /// </summary>
        Rename,

        /// <summary>
        /// Represents the Merge transformation
        /// </summary>
        Merge
    }

    /// <summary>
    /// Properties that are present within JDT transforms
    /// </summary>
    internal enum JdtProperties
    {
        /// <summary>
        /// Represents an invalid verb
        /// </summary>
        Invalid,

        /// <summary>
        /// Represents the Value attribute
        /// </summary>
        Value,

        /// <summary>
        /// Represents the JSONPath attribute
        /// </summary>
        Path
    }
}
