// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a recursive JDT transformation
    /// </summary>
    internal class JdtRename : JdtProcessor
    {
        private const string PathAttribute = "path";
        private const string ValueAttribute = "value";

        /// <inheritdoc/>
        public override string Verb { get; } = "rename";

        /// <inheritdoc/>
        public override void Process(JObject source, JObject transform)
        {
            JToken removeValue;
            if (transform.TryGetValue(JsonUtilities.JdtSyntaxPrefix + this.Verb, out removeValue))
            {
                this.Rename(source, removeValue, true);
            }

            this.Successor.Process(source, transform);
        }

        private void Rename(JObject source, JToken renameValue, bool allowArray)
        {
            switch (renameValue.Type)
            {
                case JTokenType.Array:
                    if (allowArray)
                    {
                        // If the value is an array, perform the replace for each object in the array
                        // Do not allow array values from here
                        foreach (JToken arayValue in (JArray)renameValue)
                        {
                            this.Rename(source, arayValue, false);
                        }
                    }
                    else
                    {
                        // TO DO: Clarify error
                        throw new JdtException(renameValue.Type.ToString() + " is not a valid transform value");
                    }

                    break;
                case JTokenType.Object:
                    this.RenameWithProperties(source, (JObject)renameValue);
                    break;
                default:
                    throw new JdtException(renameValue.Type.ToString() + " is not a valid transform value for Rename");
            }
        }

        private void RenameWithProperties(JObject source, JObject renameProperties)
        {
            JToken pathToken, valueToken;
            bool hasPath = renameProperties.TryGetValue(JsonUtilities.JdtSyntaxPrefix + PathAttribute, out pathToken);
            bool hasValue = renameProperties.TryGetValue(JsonUtilities.JdtSyntaxPrefix + ValueAttribute, out valueToken);

            if (!hasPath && !hasValue)
            {
                foreach (JProperty renameOperation in renameProperties.Properties())
                {
                    if (renameOperation.Value.Type != JTokenType.String)
                    {
                        throw new JdtException("Rename value must be a string");
                    }

                    // TO DO: Warning if the node is not found
                    JToken nodeToRename;
                    if (source.TryGetValue(renameOperation.Name, out nodeToRename))
                    {
                        this.RenameCore(nodeToRename, renameOperation.Value.ToString());
                    }
                }
            }
            else if (hasPath && hasValue)
            {
                if (renameProperties.Properties().Where(p => !JsonUtilities.IsJdtSyntax(p.Name)).Count() > 0)
                {
                    throw new JdtException("JDT syntax cannot be mixed with other properties");
                }

                if (pathToken.Type != JTokenType.String)
                {
                    throw new JdtException("Path attribute must be a string");
                }

                if (valueToken.Type != JTokenType.String)
                {
                    throw new JdtException("Value attribute must be a string");
                }

                var tokensToRename = JsonUtilities.GetTokensFromPath(source, pathToken.ToString());
                foreach (JToken nodeToRename in tokensToRename.ToList())
                {
                    this.RenameCore(nodeToRename, valueToken.ToString());
                }
            }
            else
            {
                throw new JdtException("Rename requires both path and value");
            }
        }

        private void RenameCore(JToken nodeToRename, string newName)
        {
            // We can only rename tokens belonging to a property
            // This excludes objects from arrays and the root object
            JProperty parent = nodeToRename.Parent as JProperty;

            if (parent == null)
            {
                throw new JdtException("Cannot rename node");
            }

            // Replace with a new property of identical value and new name
            parent.Replace(new JProperty(newName, nodeToRename));
        }
    }
}
