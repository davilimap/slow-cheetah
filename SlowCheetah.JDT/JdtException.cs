// Copyright (c) Microsoft Corporation. All rights reserved.
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
        /// Initializes a new instance of the <see cref="JdtException"/> class
        /// from a <see cref="JsonReaderException"/>
        /// </summary>
        /// <param name="ex">The original exception</param>
        public JdtException(JsonReaderException ex)
            : base(ex.Message, ex)
        {
            this.LineNumber = ex.LineNumber;
            this.LinePosition = ex.LinePosition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JdtException"/> class.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="filePath">The file that generated the exception</param>
        /// <param name="lineNumber">The line that caused the error</param>
        /// <param name="linePosition">The position in the lite that caused the error</param>
        public JdtException(string message, string filePath, int lineNumber, int linePosition)
            : this(message, filePath)
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
        public string FileName { get; private set; } = null;

        /// <summary>
        /// Casts a <see cref="JsonReaderException"/> to a <see cref="JdtException"/>
        /// </summary>
        /// <param name="ex">The <see cref="JsonReaderException"/></param>
        public static implicit operator JdtException(JsonReaderException ex)
        {
            return new JdtException(ex);
        }

        /// <summary>
        /// Returns a <see cref="JdtException"/> with line info
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="filePath">The path to the file that generated the exception</param>
        /// <param name="lineInfo">The line info of the object that caused the error</param>
        /// <returns>A new instance of <see cref="JdtException"/></returns>
        public static JdtException FromLineInfo(string message, string filePath, IJsonLineInfo lineInfo)
        {
            return new JdtException(message, filePath, lineInfo?.LineNumber ?? 0, lineInfo?.LinePosition ?? 0);
        }
    }
}
