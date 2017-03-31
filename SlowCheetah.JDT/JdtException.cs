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
