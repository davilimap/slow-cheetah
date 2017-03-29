// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the Merge transformation
    /// </summary>
    internal class JdtMerge : JdtArrayProcessor
    {
        private const string PathAttribute = "path";
        private const string ValueAttribute = "value";

        /// <inheritdoc/>
        public override string Verb { get; } = "merge";

        /// <inheritdoc/>
        protected override bool TransformCore(JObject source, JToken transformValue)
        {
            switch (transformValue.Type)
            {
                case JTokenType.Array:
                    source.ThrowIfRoot("Cannot replace root");
                    source.Replace(transformValue);
                    break;
                case JTokenType.Object:
                    this.MergeWithObject(source, (JObject)transformValue);
                    break;
                default:
                    source.ThrowIfRoot("Cannot replace root");
                    source.Replace(transformValue);
                    break;
            }

            // Do not halt transformations
            return false;
        }

        private void MergeWithObject(JObject source, JObject mergeObject)
        {
            JToken pathToken, valueToken;
            string pathFullAttribute = JsonUtilities.JdtSyntaxPrefix + PathAttribute;
            bool hasPath = mergeObject.TryGetValue(pathFullAttribute, out pathToken);
            string valueFullAttribute = JsonUtilities.JdtSyntaxPrefix + ValueAttribute;
            bool hasValue = mergeObject.TryGetValue(valueFullAttribute, out valueToken);

            if (!hasPath && !hasValue)
            {
                // If the merge object does not contain attributes,
                // simply execute the transform with that object
                ProcessTransform(source, mergeObject);
            }
            else if (hasPath && hasValue)
            {
                if (mergeObject.Properties().Any(p => !p.Name.Equals(pathFullAttribute) && !p.Name.Equals(valueFullAttribute)))
                {
                    throw new JdtException("Merge only accepts path and value attributes");
                }

                if (pathToken.Type != JTokenType.String)
                {
                    throw new JdtException("Path attribute must be a string");
                }

                foreach (JToken tokenToMerge in JsonUtilities.GetTokensFromPath(source, pathToken.ToString()))
                {
                    if (tokenToMerge.Type == JTokenType.Object && valueToken.Type == JTokenType.Object)
                    {
                        ProcessTransform((JObject)tokenToMerge, (JObject)valueToken);
                    }
                    else if (tokenToMerge.Type == JTokenType.Array && valueToken.Type == JTokenType.Array)
                    {
                        ((JArray)tokenToMerge).MergeInto((JArray)valueToken);
                    }
                    else
                    {
                        tokenToMerge.ThrowIfRoot("Cannot replace root");

                        tokenToMerge.Replace(valueToken);
                    }
                }
            }
            else
            {
                throw new JdtException("Merge requires both path and value");
            }
        }
    }
}
