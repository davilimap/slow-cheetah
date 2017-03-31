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
        private JdtAttributeValidator attributeValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="JdtRename"/> class.
        /// </summary>
        public JdtRename()
        {
            this.attributeValidator = new JdtAttributeValidator(JdtAttributes.Path, JdtAttributes.Value);
        }

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
                // Try and get attributes from the object
                var renameObject = (JObject)transformValue;
                var attributes = this.attributeValidator.ValidateAndReturnAttributes(renameObject);

                // If there are attributes, handle them accordingly
                if (attributes.Any())
                {
                    // If the object has attributes it must have both path and value
                    JToken pathToken, valueToken;
                    if (attributes.TryGetValue(JdtAttributes.Path, out pathToken) && attributes.TryGetValue(JdtAttributes.Value, out valueToken))
                    {
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
                        // If either is not present, throw
                        throw new JdtException("Rename requires both path and value");
                    }
                }
                else
                {
                    // If the object does not contain attributes, each property is a rename to execute
                    // where the key is the old name and the value must be a string with the new name of the node
                    foreach (JProperty renameOperation in renameObject.Properties())
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
