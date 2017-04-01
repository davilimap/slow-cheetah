namespace SlowCheetah.JDT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    /// <summary>
    /// Context of a JSON transformation
    /// </summary>
    internal class JsonTransformContext
    {
        /// <summary>
        /// Gets or sets the source file of the current transformation
        /// </summary>
        internal string SourceFile { get; set; } = null;

        /// <summary>
        /// Gets or sets the transformation file of the current transformation
        /// </summary>
        internal string TransformFile { get; set; } = null;

        /// <summary>
        /// Gets or sets the logger for the current transformation
        /// </summary>
        internal JsonTransformationContextLogger Logger { get; set; } = null;

        /// <summary>
        /// Logs a warning in the current context
        /// </summary>
        /// <param name="message">The warning message</param>
        /// <param name="lineInfo">The line info of the object that generated the warning</param>
        internal void LogTransformWarning(string message, IJsonLineInfo lineInfo)
        {
            if (this.Logger != null)
            {
                if (lineInfo != null && lineInfo.HasLineInfo())
                {
                    
                }
                else
                {
                }
            }
        }
    }
}
