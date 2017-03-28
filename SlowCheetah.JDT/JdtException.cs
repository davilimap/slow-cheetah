// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Exception thrown on JDT error
    /// </summary>
    [Serializable]
    public class JdtException : Exception
    {
        private IJsonLineInfo lineInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="JdtException"/> class.
        /// </summary>
        /// <param name="message">The exception message</param>
        public JdtException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JdtException"/> class.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="filePath">The file that generated the exception</param>
        public JdtException(string message, string filePath)
            : this(message)
        {
            this.FileName = filePath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JdtException"/> class.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="filePath">The file that generated the exception</param>
        /// <param name="lineInfo">Line info on the JToken that generated the exception</param>
        public JdtException(string message, string filePath, IJsonLineInfo lineInfo)
            : this(message, filePath)
        {
        }

        /// <summary>
        /// Gets the line number of the exception
        /// </summary>
        public int LineNumber
        {
            get
            {
                return this.lineInfo == null ? this.lineInfo.LineNumber : 0;
            }
        }

        /// <summary>
        /// Gets the line position of the exception
        /// </summary>
        public int LinePosition
        {
            get
            {
                return this.lineInfo == null ? this.lineInfo.LinePosition : 0;
            }
        }

        /// <summary>
        /// Gets the name of the file that generated the exception
        /// </summary>
        public string FileName { get; private set; } = null;
    }
}
