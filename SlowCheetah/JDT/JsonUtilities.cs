﻿// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Utilities class for handling JSON files
    /// </summary>
    internal static class JsonUtilities
    {
        /// <summary>
        /// The suffix for all JDT syntax
        /// </summary>
        internal const string JdtSyntaxPrefix = "@jdt.";

        /// <summary>
        /// Merges an array into another.
        /// Merges a clone of the array.
        /// </summary>
        /// <param name="original">Array to merge into</param>
        /// <param name="arrayToMerge">Array to be merged</param>
        internal static void MergeArray(JArray original, JArray arrayToMerge)
        {
            foreach (JToken token in arrayToMerge)
            {
                original.Add(token.DeepClone());
            }
        }

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
            return !string.IsNullOrEmpty(key) && key.StartsWith(JdtSyntaxPrefix);
        }

        /// <summary>
        /// Gets the JDT syntax in the key
        /// </summary>
        /// <param name="key">The JDT key, in the correct syntax</param>
        /// <returns>The string property. Null if the property does is not JDT syntax</returns>
        internal static string GetJdtSyntax(string key)
        {
            if (!IsJdtSyntax(key))
            {
                // Empty or null strings
                // If the key does not start with the correct prefix,
                // it is not a JDT verb
                return null;
            }
            else
            {
                // Remove the prefix
                return key.Substring(JdtSyntaxPrefix.Length);
            }
        }

        /// <summary>
        /// Gets tokens from a node or its root using JSONPath
        /// </summary>
        /// <param name="node">The node to base the search on</param>
        /// <param name="path">The JSONPath to find nodes.</param>
        /// <returns>The corresponding tokens</returns>
        internal static IEnumerable<JToken> GetTokensFromPath(JObject node, string path)
        {
            /* TO DO: Evaluate lower level paths that start from the root
            JToken startingNode = node.Root;
            if (path.StartsWith("@"))
            {
                path = "$" + path.Substring(1);
                startingNode = node;
            } */

            return node.SelectTokens(path, true);
        }
    }
}
