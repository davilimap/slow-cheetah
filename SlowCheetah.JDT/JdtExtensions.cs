namespace SlowCheetah.JDT
{
    using System;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Defines extension methods used in JDT
    /// </summary>
    internal static class JdtExtensions
    {
        /// <summary>
        /// Throws a <see cref="JdtException"/> if the given node is the root
        /// </summary>
        /// <param name="token">The token to verify</param>
        /// <param name="errorMessage">Error message</param>
        internal static void ThrowIfRoot(this JToken token, string errorMessage)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (token.Root.Equals(token))
            {
                throw new JdtException(errorMessage);
            }
        }
    }
}
