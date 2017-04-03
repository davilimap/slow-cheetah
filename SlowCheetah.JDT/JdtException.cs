// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// The file that caused the exception
    /// </summary>
    internal enum ErrorLocation
    {
        /// <summary>
        /// Represents no location set
        /// </summary>
        None,

        /// <summary>
        /// Represents the source file
        /// </summary>
        Source,

        /// <summary>
        /// Represents the transform file
        /// </summary>
        Transform
    }

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

        /// <summary>
        /// Initializes a new instance of the <see cref="JdtException"/> class.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="location">The file that generated the exception</param>
        internal JdtException(string message, ErrorLocation location)
            : this(message)
        {
            this.Location = location;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JdtException"/> class.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="location">The file that generated the exception</param>
        /// <param name="lineNumber">The line that caused the error</param>
        /// <param name="linePosition">The position in the lite that caused the error</param>
        internal JdtException(string message, ErrorLocation location, int lineNumber, int linePosition)
            : this(message, location)
        {
            this.LineNumber = lineNumber;
            this.LinePosition = linePosition;
        }

        /// <summary>
        /// Gets the line number of the exception
        /// </summary>
        public int LineNumber { get; private set; }

        /// <summary>
        /// Gets the line position of the exception
        /// </summary>
        public int LinePosition { get; private set; }

        /// <summary>
        /// Gets the name of the file that generated the exception
        /// </summary>
        internal ErrorLocation Location { get; private set; } = ErrorLocation.None;

        /// <summary>
        /// Returns a <see cref="JdtException"/> with line info
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="location">The file that generated the exception</param>
        /// <param name="lineInfo">The line info of the object that caused the error</param>
        /// <returns>A new instance of <see cref="JdtException"/></returns>
        internal static JdtException FromLineInfo(string message, ErrorLocation location, IJsonLineInfo lineInfo)
        {
            return new JdtException(message, location, lineInfo?.LineNumber ?? 0, lineInfo?.LinePosition ?? 0);
        }
    }
}
