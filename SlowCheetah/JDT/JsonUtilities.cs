// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Utilities class for handling JSON files
    /// </summary>
    internal static class JsonUtilities
    {
        private static readonly string JdtVerbPrefix = "@JDT.";

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
                    LineInfoHandling = LineInfoHandling.Ignore
                };

                return JObject.Load(reader, loadSettings);
            }
        }

        /// <summary>
        /// Gets the JDT verb in the key
        /// </summary>
        /// <param name="key">The JSON key to analyze</param>
        /// <returns>The verb in the string. Null if the string is not a verb</returns>
        internal static JdtVerbs GetJdtVerb(string key)
        {
            if (string.IsNullOrEmpty(key) || key.StartsWith(JdtVerbPrefix, StringComparison.CurrentCultureIgnoreCase))
            {
                return JdtVerbs.None;
            }
            else
            {
                return key.Substring(JdtVerbPrefix.Length);
            }
        }
    }
}
