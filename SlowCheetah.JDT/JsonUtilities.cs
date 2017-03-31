﻿namespace SlowCheetah.JDT
{
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Utilities class for handling JSON files
    /// </summary>
    internal static class JsonUtilities
    {
        /// <summary>
        /// The prefix for all JDT syntax
        /// </summary>
        internal const string JdtSyntaxPrefix = "@jdt.";

        /// <summary>
        /// Wheter the given key corresponds to a JDT verb
        /// </summary>
        /// <param name="key">The JSON key to analyze</param>
        /// <returns>True if the key corresponds to a verb</returns>
        internal static bool IsJdtSyntax(string key)
        {
            // If the key is empty of does not start with the correct prefix,
            // it is not a valid verb
            return !string.IsNullOrEmpty(key) && key.StartsWith(JdtSyntaxPrefix);
        }

        /// <summary>
        /// Gets the JDT syntax in the key
        /// </summary>
        /// <param name="key">The JDT key, in the correct syntax</param>
        /// <returns>The string property. Null if the property does is not JDT syntax</returns>
        internal static string GetJdtSyntax(string key)
        {
            // If the key does not start with the correct prefix, it is not a JDT verb
            // If it is a JDT verb, remove the prefix
            return IsJdtSyntax(key) ? key.Substring(JdtSyntaxPrefix.Length) : null;
        }

        /// <summary>
        /// Throws a <see cref="JdtException"/> if the given node is the root
        /// </summary>
        /// <param name="token">The token to verify</param>
        /// <param name="errorMessage">Error message</param>
        internal static void ThrowIfRoot(JToken token, string errorMessage)
        {
            if (token.Root.Equals(token))
            {
                throw new JdtException(errorMessage);
            }
        }
    }
}
