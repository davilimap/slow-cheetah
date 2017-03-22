// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Utilities class for handling JSON files
    /// </summary>
    internal static class JsonUtilities
    {
        private static readonly string JdtSyntaxPrefix = "@JDT.";

        /// <summary>
        /// Loads a JSON file to a JToken
        /// </summary>
        /// <param name="filePath">Full path to the JSON file</param>
        /// <returns>JObject extracted from the file</returns>
        internal static JObject LoadObjectFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            using (StreamReader file = File.OpenText(filePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JsonLoadSettings loadSettings = new JsonLoadSettings()
                {
                    CommentHandling = CommentHandling.Ignore,

                    // Obs: LineInfo is handled on Ignore and not Load
                    LineInfoHandling = LineInfoHandling.Ignore
                };

                return JObject.Load(reader, loadSettings);
            }
        }

        /// <summary>
        /// Wheter the given key corresponds to a JDT verb
        /// </summary>
        /// <param name="key">The JSON key to analyze</param>
        /// <returns>True if the key corresponds to a verb</returns>
        internal static bool IsJdtSyntax(string key)
        {
            // If the key is empty of does not start with the correct prefix,
            // it is not a valid verb
            return !string.IsNullOrEmpty(key) && key.StartsWith(JdtSyntaxPrefix, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Gets the JDT property in the key
        /// </summary>
        /// <param name="key">The JDT key, in the correct syntax</param>
        /// <returns>The property in the string</returns>
        internal static JdtProperties GetJdtProperty(string key)
        {
            if (!IsJdtSyntax(key))
            {
                // Empty or null strings
                // If the key does not start with the correct prefix,
                // it is not a JDT verb
                throw new ArgumentException("\"" + key + "\" is not valid JDT property syntax");
            }
            else
            {
                // Remove the prefix
                string propertyName = key.Substring(JdtSyntaxPrefix.Length);
                JdtProperties property;
                if (Enum.TryParse(propertyName, true, out property))
                {
                    // If the property pareses to Invalid, it actually is an invalid transformation
                    return property;
                }
                else
                {
                    // If it is not any of the known properties, it is invalid
                    return JdtProperties.Invalid;
                }
            }
        }

        /// <summary>
        /// Gets the JDT verb in the key
        /// Ignores case
        /// </summary>
        /// <param name="key">The JDT verb, in the correct syntax</param>
        /// <returns>The verb in the string. <see cref="JdtVerbs.Invalid"/> if invalid verb</returns>
        internal static JdtVerbs GetJdtVerb(string key)
        {
            if (!IsJdtSyntax(key))
            {
                // Empty or null strings
                // If the key does not start with the correct prefix,
                // it is not a JDT verb
                throw new ArgumentException("\"" + key + "\" is not valid JDT verb syntax");
            }
            else
            {
                // Remove the prefix
                string verbName = key.Substring(JdtSyntaxPrefix.Length);
                JdtVerbs verb;
                if (Enum.TryParse(verbName, true, out verb))
                {
                    // If the transform pareses to Invalid, it actually is an invalid transformation
                    return verb;
                }
                else
                {
                    // If it is not any of the known verbs, it is invalid
                    return JdtVerbs.Invalid;
                }
            }
        }
    }
}
