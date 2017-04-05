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
        private StringBuilder errorLog = new StringBuilder();

        private StringBuilder warningLog = new StringBuilder();

        private StringBuilder messageLog = new StringBuilder();

        public string ErrorLogText
        {
            get
            {
                return this.errorLog.ToString();
            }
        }

        public string WarningLogText
        {
            get
            {
                return this.warningLog.ToString();
            }
        }

        public string MessageLogText
        {
            get
            {
                return this.messageLog.ToString();
            }
        }

        public void LogError(string message)
        {
            this.errorLog.AppendLine(message);
        }

        public void LogError(string message, string fileName, int lineNumber, int linePosition)
        {
            this.errorLog.AppendLine(this.BuildLine(message, fileName, lineNumber, linePosition));
        }

        public void LogErrorFromException(Exception ex)
        {
            this.errorLog.AppendLine($"Exception: {ex.Message}");
        }

        public void LogErrorFromException(Exception ex, string fileName, int lineNumber, int linePosition)
        {
            this.errorLog.AppendLine(this.BuildLine($"Exception: {ex.Message}", fileName, lineNumber, linePosition));
        }

        public void LogMessage(string message)
        {
            this.messageLog.AppendLine(message);
        }

        public void LogWarning(string message)
        {
            this.warningLog.AppendLine(message);
        }

        public void LogWarning(string message, string fileName)
        {
            this.warningLog.AppendLine($"{message} {fileName}");
        }

        public void LogWarning(string message, string fileName, int lineNumber, int linePosition)
        {
            this.warningLog.AppendLine(this.BuildLine(message, fileName, lineNumber, linePosition));
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
