// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT.Tests
{
    using System;
    using System.Text;

    /// <summary>
    /// Mock logger to test <see cref="JsonTransformation"/>
    /// </summary>
    public class JsonTransformationTestLogger : IJsonTransformationLogger
    {
        public StringBuilder ErrorLog { get; } = new StringBuilder();

        public StringBuilder WarningLog { get; } = new StringBuilder();

        public StringBuilder MessageLog { get; } = new StringBuilder();

        public void LogError(string message)
        {
            this.ErrorLog.AppendLine(message);
        }

        public void LogError(string message, string fileName, int lineNumber, int linePosition)
        {
            this.ErrorLog.AppendLine(this.BuildLine(message, fileName, lineNumber, linePosition));
        }

        public void LogErrorFromException(Exception ex)
        {
            this.ErrorLog.AppendLine($"Exception: {ex.Message}");
        }

        public void LogErrorFromException(Exception ex, string fileName, int lineNumber, int linePosition)
        {
            this.ErrorLog.AppendLine(this.BuildLine($"Exception: {ex.Message}", fileName, lineNumber, linePosition));
        }

        public void LogMessage(string message)
        {
            this.MessageLog.AppendLine(message);
        }

        public void LogWarning(string message)
        {
            this.WarningLog.AppendLine(message);
        }

        public void LogWarning(string message, string fileName)
        {
            this.WarningLog.AppendLine($"{message} {fileName}");
        }

        public void LogWarning(string message, string fileName, int lineNumber, int linePosition)
        {
            this.WarningLog.AppendLine(this.BuildLine(message, fileName, lineNumber, linePosition));
        }

        private string BuildLine(string message, string fileName, int lineNumber, int linePosition)
        {
            string line = message;
            if (fileName != null)
            {
                line += " " + fileName;
            }

            line += $" {lineNumber} {linePosition}";

            return line;
        }
    }
}
