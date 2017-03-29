﻿// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a recursive JDT transformation
    /// </summary>
    internal class JdtReplace : JdtArrayProcessor
    {
        private const string PathAttribute = "path";
        private const string ValueAttribute = "value";

        /// <inheritdoc/>
        public override string Verb { get; } = "replace";

        /// <inheritdoc/>
        protected override bool TransformCore(JObject source, JToken transformValue)
        {
            switch (transformValue.Type)
            {
                case JTokenType.Object:
                    return this.ReplaceWithProperties(source, (JObject)transformValue);
                default:
                    source.Replace(transformValue);
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
                        // If the specified path is to the current node
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
