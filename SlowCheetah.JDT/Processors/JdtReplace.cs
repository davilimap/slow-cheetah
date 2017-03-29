// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a recursive JDT transformation
    /// </summary>
    internal class JdtReplace : JdtProcessor
    {
        private const string PathAttribute = "path";
        private const string ValueAttribute = "value";

        private bool replacedThisNode;

        /// <inheritdoc/>
        internal override string Verb { get; } = "replace";

        /// <inheritdoc/>
        internal override void Process(JObject source, JObject transform, JsonTransformContext context)
        {
            this.replacedThisNode = false;

            JToken replaceValue;
            if (transform.TryGetValue(JsonUtilities.JdtSyntaxPrefix + this.Verb, out replaceValue))
            {
                this.Replace(source, replaceValue, true);
            }

            if (!this.replacedThisNode)
            {
                // If the current node was replaced, then do not perform any more transformations here
                this.Successor.Process(source, transform, context);
            }
        }

        private void Replace(JObject source, JToken replaceValue, bool allowArray)
        {
            switch (replaceValue.Type)
            {
                case JTokenType.Array:
                    if (allowArray)
                    {
                        // If the value is an array, perform the replace for each object in the array
                        foreach (JToken arayValue in (JArray)replaceValue)
                        {
                            this.Replace(source, arayValue, false);
                            if (this.replacedThisNode)
                            {
                                // If a value in the array performs a remove of the current node,
                                // Stop transformations
                                return;
                            }
                        }
                    }
                    else
                    {
                        source.Replace(replaceValue);
                        this.replacedThisNode = true;
                    }

                    break;
                case JTokenType.Object:
                    this.ReplaceWithProperties(source, (JObject)replaceValue);
                    break;
                default:
                    source.Replace(replaceValue);
                    this.replacedThisNode = true;
                    break;
            }
        }

        private void ReplaceWithProperties(JObject source, JObject replaceObject)
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
                this.replacedThisNode = true;
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

                var tokensToReplace = JsonUtilities.GetTokensFromPath(source, pathToken.ToString());
                foreach (JToken nodeToReplace in tokensToReplace.ToList())
                {
                    if (nodeToReplace.Equals(source))
                    {
                        // If the specified is to the current
                        this.replacedThisNode = true;
                    }

                    nodeToReplace.Replace(valueToken);

                    if (this.replacedThisNode)
                    {
                        // If the current node was replaced, stop executing transformations on this node
                        return;
                    }
                }
            }
            else
            {
                throw new JdtException("Replace requires both path and value");
            }
        }
    }
}
