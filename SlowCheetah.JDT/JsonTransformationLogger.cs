// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;

    /// <summary>
    /// Logger wrapper for JDT transformations
    /// </summary>
    internal class JsonTransformationLogger
    {
        private IJsonTransformationLogger externalLogger = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTransformationLogger"/> class.
        /// </summary>
        /// <param name="extLogger">External logger to be used. Can be null.</param>
        internal JsonTransformationLogger(IJsonTransformationLogger extLogger)
        {
            this.externalLogger = extLogger;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the logger has logged errrors
        /// </summary>
        internal bool HasLoggedErrors { get; set; }

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="message">The message to log</param>
        internal void LogMessage(string message)
        {
            if (this.externalLogger != null)
            {
                this.externalLogger.LogMessage(message);
            }
        }

        /// <summary>
        /// Logs a warning
        /// </summary>
        /// <param name="message">The warning message</param>
        internal void LogWarning(string message)
        {
        }

        /// <summary>
        /// Logs an error
        /// </summary>
        /// <param name="message">The error message</param>
        internal void LogError(string message)
        {
        }

        /// <summary>
        /// Logs an error from an internal exception
        /// </summary>
        /// <param name="exception">The exception to log</param>
        internal void LogErrorFromException(Exception exception)
        {
            if (this.externalLogger != null)
            {
                this.HasLoggedErrors = true;
                JdtException jdtException = exception as JdtException;
                if (jdtException != null)
                {
                    this.externalLogger.LogErrorFromException(jdtException, jdtException.FileName, jdtException.LineNumber, jdtException.LinePosition);
                }
                else
                {
                    this.externalLogger.LogErrorFromException(exception);
                }
            }
            else
            {
                throw exception;
            }
        }
    }
}
