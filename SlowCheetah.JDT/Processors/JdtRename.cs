namespace SlowCheetah.JDT
{
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the Rename transformation
    /// </summary>
    internal class JdtRename : JdtArrayProcessor
    {
        private readonly JdtAttributeValidator attributeValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="JdtRename"/> class.
        /// </summary>
        public JdtRename()
        {
            // Rename accepts the path and value attributes
            this.attributeValidator = new JdtAttributeValidator(JdtAttributes.Path, JdtAttributes.Value);
        }

        /// <inheritdoc/>
        public override string Verb { get; } = "rename";

        /// <inheritdoc/>
        protected override bool ProcessCore(JObject source, JToken transformValue, JsonTransformationContextLogger logger)
        {
            if (transformValue.Type != JTokenType.Object)
            {
                // Rename only accepts objects, either with properties or direct renames
                throw JdtException.FromLineInfo($"{transformValue.Type.ToString()} is not a valid transform value for Rename", ErrorLocation.Transform, transformValue);
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
                            throw JdtException.FromLineInfo("Path attribute must be a string", ErrorLocation.Transform, pathToken);
                        }

                        if (valueToken.Type != JTokenType.String)
                        {
                            throw JdtException.FromLineInfo("Value attribute must be a string", ErrorLocation.Transform, valueToken);
                        }

                        // If the values are correct, rename each token found with the given path
                        foreach (JToken nodeToRename in source.SelectTokens(pathToken.ToString()).ToList())
                        {
                            if (!this.RenameNode(nodeToRename, valueToken.ToString()))
                            {
                                throw JdtException.FromLineInfo("Cannot rename node", ErrorLocation.Transform, renameObject);
                            }
                        }
                    }
                    else
                    {
                        // If either is not present, throw
                        throw JdtException.FromLineInfo("Rename requires both path and value", ErrorLocation.Transform, renameObject);
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
                            throw JdtException.FromLineInfo("Rename value must be a string", ErrorLocation.Transform, renameOperation);
                        }

                        // TO DO: Warning if the node is not found
                        JToken nodeToRename;
                        if (source.TryGetValue(renameOperation.Name, out nodeToRename))
                        {
                            if (!this.RenameNode(nodeToRename, renameOperation.Value.ToString()))
                            {
                                throw JdtException.FromLineInfo("Cannot rename node", ErrorLocation.Transform, renameOperation);
                            }
                        }
                        else
                        {
                            logger.LogWarning($"Node {renameOperation.Name} was not found", ErrorLocation.Transform, renameOperation);
                        }
                    }
                }
            }

            // Do not halt transformations
            return true;
        }

        private bool RenameNode(JToken nodeToRename, string newName)
        {
            // We can only rename tokens belonging to a property
            // This excludes objects from arrays and the root object
            JProperty parent = nodeToRename.Parent as JProperty;

            if (parent == null)
            {
                return false;
            }

            // Replace with a new property of identical value and new name
            parent.Replace(new JProperty(newName, nodeToRename));
            return true;
        }
    }
}
