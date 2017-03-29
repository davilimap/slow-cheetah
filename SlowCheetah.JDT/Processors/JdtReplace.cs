// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a recursive JDT transformation
    /// </summary>
    internal class JdtReplace : JdtProcessor
    {
        private const string PathAttribute = "path";
        private const string ValueAttribute = "value";

        /// <inheritdoc/>
        public override string Verb { get; } = "replace";

        /// <inheritdoc/>
        public override void Process(JObject source, JObject transform)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            JToken replaceValue;
            if (transform.TryGetValue(JsonUtilities.JdtSyntaxPrefix + this.Verb, out replaceValue))
            {
                if (this.Replace(source, replaceValue))
                {
                    // If the current node was replaced, then do not perform any more transformations here
                    return;
                }
            }

            this.Successor.Process(source, transform);
        }

        private bool Replace(JObject source, JToken replaceValue)
        {
            if (replaceValue.Type == JTokenType.Array)
            {
                // If the value is an array, perform the replace for each object in the array
                foreach (JToken arrayValue in (JArray)replaceValue)
                {
                    if (this.ReplaceCore(source, arrayValue))
                    {
                        // If a value in the array performs a remove of the current node,
                        // Stop transformations
                        return true;
                    }
                }

                return false;
            }
            else
            {
                return this.ReplaceCore(source, replaceValue);
            }
        }

        private bool ReplaceCore(JObject source, JToken replaceValue)
        {
            switch (replaceValue.Type)
            {
                case JTokenType.Object:
                    return this.ReplaceWithProperties(source, (JObject)replaceValue);
                default:
                    source.Replace(replaceValue);
                    return true;
            }
        }

        private bool ReplaceWithProperties(JObject source, JObject replaceObject)
        {
            // TO DO: Verify if the replace value contains JDT syntax
            JToken pathToken, valueToken;
            string pathFullAttribute = JsonUtilities.JdtSyntaxPrefix + PathAttribute;
            bool hasPath = replaceObject.TryGetValue(pathFullAttribute, out pathToken);
            string valueFullAttribute = JsonUtilities.JdtSyntaxPrefix + ValueAttribute;
            bool hasValue = replaceObject.TryGetValue(valueFullAttribute, out valueToken);

            if (!hasPath && !hasValue)
            {
                source.Replace(replaceObject);
                return true;
            }
            else if (hasPath && hasValue)
            {
                if (replaceObject.Properties().Where(p => !p.Name.Equals(pathFullAttribute) && !p.Name.Equals(valueFullAttribute)).Count() > 0)
                {
                    throw new JdtException("Replace only accepts path and value attributes");
                }

                if (pathToken.Type != JTokenType.String)
                {
                    throw new JdtException("Path attribute must be a string");
                }

                foreach (JToken nodeToReplace in JsonUtilities.GetTokensFromPath(source, pathToken.ToString()))
                {
                    bool replacedThisNode = false;

                    if (nodeToReplace.Equals(source))
                    {
                        // If the specified is to the current
                        replacedThisNode = true;
                    }

                    nodeToReplace.Replace(valueToken);

                    if (replacedThisNode)
                    {
                        // If the current node was replaced, stop executing transformations on this node
                        return true;
                    }
                }
            }
            else
            {
                throw new JdtException("Replace requires both path and value");
            }

            return false;
        }
    }
}
