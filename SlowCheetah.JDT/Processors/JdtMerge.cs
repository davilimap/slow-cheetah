// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the Merge transformation
    /// </summary>
    internal class JdtMerge : JdtProcessor
    {
        private const string PathAttribute = "path";
        private const string ValueAttribute = "value";

        /// <inheritdoc/>
        public override string Verb { get; } = "merge";

        /// <inheritdoc/>
        public override void Process(JObject source, JObject transform)
        {
            JToken mergeValue;
            if (transform.TryGetValue(JsonUtilities.JdtSyntaxPrefix + this.Verb, out mergeValue))
            {
                this.Merge(source, mergeValue);
            }

            this.Successor.Process(source, transform);
        }

        private void Merge(JObject source, JToken mergeValue)
        {
            if (mergeValue.Type == JTokenType.Array)
            {
                // If the value is an array, perform the replace for each object in the array
                foreach (JToken arrayValue in (JArray)mergeValue)
                {
                    this.MergeCore(source, arrayValue);
                }
            }
            else
            {
                // If not, perform the merge as normal
                this.MergeCore(source, mergeValue);
            }
        }

        private void MergeCore(JObject source, JToken mergeValue)
        {
            switch (mergeValue.Type)
            {
                case JTokenType.Array:
                    JsonUtilities.ThrowIfRoot(source, "Cannot replace root");
                    source.Replace(mergeValue);
                    break;
                case JTokenType.Object:
                    this.MergeWithObject(source, (JObject)mergeValue);
                    break;
                default:
                    JsonUtilities.ThrowIfRoot(source, "Cannot replace root");
                    source.Replace(mergeValue);
                    break;
            }
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
                        JsonUtilities.MergeArray((JArray)tokenToMerge, (JArray)valueToken);
                    }
                    else
                    {
                        JsonUtilities.ThrowIfRoot(tokenToMerge, "Cannot replace root");

                        tokenToMerge.Replace(valueToken);
                    }
                }
            }
            else
            {
                throw new JdtException("Rename requires both path and value");
            }
        }
    }
}
