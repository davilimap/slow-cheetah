// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;

    /// <summary>
    /// Exception thrown on JDT error
    /// </summary>
    [Serializable]
    public class JdtException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JdtException"/> class.
        /// </summary>
        /// <param name="message">The exception message</param>
        public JdtException(string message)
            : base(message)
        {
        }
    }
}
