namespace SlowCheetah.JDT
{
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the Replace transformation
    /// </summary>
    internal class JdtReplace : JdtArrayProcessor
    {
        private JdtAttributeValidator attributeValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="JdtReplace"/> class.
        /// </summary>
        public JdtReplace()
        {
            this.attributeValidator = new JdtAttributeValidator(JdtAttributes.Path, JdtAttributes.Value);
        }

        /// <inheritdoc/>
        public override string Verb { get; } = "replace";

        /// <inheritdoc/>
        protected override bool ProcessCore(JObject source, JToken transformValue, JsonTransformationContextLogger logger)
        {
            if (transformValue.Type == JTokenType.Object)
            {
                // If the value is an object, analyze the contents and perform the appropriate transform
                return this.ReplaceWithProperties(source, (JObject)transformValue);
            }
            else
            {
                // If the value is not an object, simply replace the original node with the new value
                source.Replace(transformValue);

                // If the node is replaced, stop transformations on it
                return false;
            }
        }

        private bool ReplaceWithProperties(JObject source, JObject replaceObject)
        {
            var attributes = this.attributeValidator.ValidateAndReturnAttributes(replaceObject);

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

                    foreach (JToken nodeToReplace in source.SelectTokens(pathToken.ToString()).ToList())
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
                            return false;
                        }
                    }
                }
                else
                {
                    // If either is not present, throw
                    throw JdtException.FromLineInfo("Replace requires both path and value", ErrorLocation.Transform, replaceObject);
                }

                // If we got here, transformations should continue
                return true;
            }
            else
            {
                // If there are no attributes, replace the current object with the given object
                source.Replace(replaceObject);

                // If the node is replaced, stop transformations on it
                return false;
            }
        }
    }
}
