// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Logger wrapper for JDT transformations
    /// </summary>
    internal class JsonTransformationContextLogger
    {
        private IJsonTransformationLogger externalLogger = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTransformationContextLogger"/> class.
        /// </summary>
        /// <param name="extLogger">External logger to be used. Can be null.</param>
        internal JsonTransformationContextLogger(IJsonTransformationLogger extLogger)
        {
            this.externalLogger = extLogger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTransformationContextLogger"/> class.
        /// </summary>
        /// <param name="transformationFile">The file that specifies the transformations</param>
        /// <param name="extLogger">External logger to be used. Can be null.</param>
        internal JsonTransformationContextLogger(string transformationFile, IJsonTransformationLogger extLogger)
        {
            this.externalLogger = extLogger;
            this.TransformFile = transformationFile;
        }

        /// <summary>
        /// Gets or sets the source file of the current transformation
        /// </summary>
        internal string SourceFile { get; set; } = "Source";

        /// <summary>
        /// Gets or sets the transformation file of the current transformation
        /// </summary>
        internal string TransformFile { get; set; } = "Transform";

        /// <summary>
        /// Gets or sets a value indicating whether the logger has logged errrors
        /// </summary>
        internal bool HasLoggedErrors { get; set; }

        /// <summary>
        /// Logs an error from an internal exception
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <returns>True if the exception was logged</returns>
        internal bool LogErrorFromException(Exception exception)
        {
            if (this.externalLogger != null)
            {
                this.HasLoggedErrors = true;
                JdtException jdtException = exception as JdtException;
                if (jdtException != null)
                {
                    this.externalLogger.LogErrorFromException(jdtException, this.TransformFile, jdtException.LineNumber, jdtException.LinePosition);
                }
                else
                {
                    JsonReaderException readerException = exception as JsonReaderException;
                    if (readerException == null)
                    {
                        this.externalLogger.LogErrorFromException(exception);
                    }
                    else
                    {
                        this.externalLogger.LogErrorFromException(readerException, readerException.Path, readerException.LineNumber, readerException.LinePosition);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Logs a warning according to the lineinfo
        /// </summary>
        /// <param name="message">The warning message</param>
        /// <param name="location">The file that caused the warning</param>
        /// <param name="lineInfo">The information of the line that caused the warning</param>
        internal void LogWarning(string message, ErrorLocation location, IJsonLineInfo lineInfo)
        {

        }
    }
}
