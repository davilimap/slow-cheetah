namespace SlowCheetah.JDT
{
    using System;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a recursive JDT transformation
    /// </summary>
    internal class JdtRename : JdtArrayProcessor
    {
        private const string PathAttribute = "path";
        private const string ValueAttribute = "value";

        /// <inheritdoc/>
        public override string Verb { get; } = "rename";

        /// <inheritdoc/>
        protected override bool ProcessCore(JObject source, JToken transformValue)
        {
            if (transformValue.Type != JTokenType.Object)
            {
                // Rename only accepts objects, either with properties or direct renames
                throw new JdtException(transformValue.Type.ToString() + " is not a valid transform value for Rename");
            }
            else
            {
                var renameProperties = (JObject)transformValue;
                JToken pathToken, valueToken;
                string pathFullAttribute = JsonUtilities.JdtSyntaxPrefix + PathAttribute;
                bool hasPath = renameProperties.TryGetValue(pathFullAttribute, out pathToken);
                string valueFullAttribute = JsonUtilities.JdtSyntaxPrefix + ValueAttribute;
                bool hasValue = renameProperties.TryGetValue(valueFullAttribute, out valueToken);

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
                            this.RenameNode(nodeToRename, renameOperation.Value.ToString());
                        }
                    }
                }
                else if (hasPath && hasValue)
                {
                    if (renameProperties.Properties().Where(p => !p.Name.Equals(pathFullAttribute) && !p.Name.Equals(valueFullAttribute)).Count() > 0)
                    {
                        throw new JdtException("Rename only accepts path and value attributes");
                    }

                    if (pathToken.Type != JTokenType.String)
                    {
                        throw new JdtException("Path attribute must be a string");
                    }

                    if (valueToken.Type != JTokenType.String)
                    {
                        throw new JdtException("Value attribute must be a string");
                    }

                    foreach (JToken nodeToRename in source.SelectTokens(pathToken.ToString()).ToList())
                    {
                        this.RenameNode(nodeToRename, valueToken.ToString());
                    }
                }
                else
                {
                    throw new JdtException("Rename requires both path and value");
                }
            }

            // Do not halt transformations
            return true;
        }

        private void RenameNode(JToken nodeToRename, string newName)
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
