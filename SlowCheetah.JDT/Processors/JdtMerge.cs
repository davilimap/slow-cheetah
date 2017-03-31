namespace SlowCheetah.JDT
{
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the Merge transformation
    /// </summary>
    internal class JdtMerge : JdtArrayProcessor
    {
        private JdtAttributeValidator attributeValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="JdtMerge"/> class.
        /// </summary>
        public JdtMerge()
        {
            // Merge accepts path and value attributes
            this.attributeValidator = new JdtAttributeValidator(JdtAttributes.Path, JdtAttributes.Value);
        }

        /// <inheritdoc/>
        public override string Verb { get; } = "merge";

        /// <inheritdoc/>
        protected override bool ProcessCore(JObject source, JToken transformValue, JsonTransformContext context)
        {
            if (transformValue.Type == JTokenType.Object)
            {
                // If both source and transform are objects,
                // analyze the contents and perform the appropriate transforms
                this.MergeWithObject(source, (JObject)transformValue, context);
            }
            else
            {
                // If the transform value is not an object, then simply replace it with
                source.ThrowIfRoot("Cannot replace root");
                source.Replace(transformValue);
            }

            // Do not halt transformations
            return true;
        }

        private void MergeWithObject(JObject source, JObject mergeObject, JsonTransformContext context)
        {
            var attributes = this.attributeValidator.ValidateAndReturnAttributes(mergeObject);

            // If there are attributes, handle them accordingly
            if (attributes.Any())
            {
                // If the object has attributes it must have both path and value
                // TO DO: Accept value without path
                JToken pathToken, valueToken;
                if (attributes.TryGetValue(JdtAttributes.Path, out pathToken) && attributes.TryGetValue(JdtAttributes.Value, out valueToken))
                {
                    if (pathToken.Type != JTokenType.String)
                    {
                        throw new JdtException("Path attribute must be a string");
                    }

                    foreach (JToken tokenToMerge in source.SelectTokens(pathToken.ToString()).ToList())
                    {
                        // Perform the merge for each element found through the path
                        if (tokenToMerge.Type == JTokenType.Object && valueToken.Type == JTokenType.Object)
                        {
                            // If they are both objects, start a new transformation
                            ProcessTransform((JObject)tokenToMerge, (JObject)valueToken, context);
                        }
                        else if (tokenToMerge.Type == JTokenType.Array && valueToken.Type == JTokenType.Array)
                        {
                            // If they are both arrays, add the new values to the original
                            ((JArray)tokenToMerge).Merge(valueToken.DeepClone());
                        }
                        else
                        {
                            // If they are primitives or have different values,
                            // perform a replace
                            tokenToMerge.ThrowIfRoot("Cannot replace root");
                            tokenToMerge.Replace(valueToken);
                        }
                    }
                }
                else
                {
                    // If either is not present, throw
                    throw new JdtException("Merge requires both path and value");
                }
            }
            else
            {
                // If the merge object does not contain attributes,
                // simply execute the transform with that object
                ProcessTransform(source, mergeObject, context);
            }
        }
    }
}
