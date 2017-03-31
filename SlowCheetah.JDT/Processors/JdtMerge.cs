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
            this.attributeValidator = new JdtAttributeValidator(JdtAttributes.Path, JdtAttributes.Value);
        }

        /// <inheritdoc/>
        public override string Verb { get; } = "merge";

        /// <inheritdoc/>
        protected override bool ProcessCore(JObject source, JToken transformValue)
        {
            switch (transformValue.Type)
            {
                case JTokenType.Array:
                    source.ThrowIfRoot("Cannot replace root");
                    source.Replace(transformValue);
                    break;
                case JTokenType.Object:
                    this.MergeWithObject(source, (JObject)transformValue);
                    break;
                default:
                    source.ThrowIfRoot("Cannot replace root");
                    source.Replace(transformValue);
                    break;
            }

            // Do not halt transformations
            return true;
        }

        private void MergeWithObject(JObject source, JObject mergeObject)
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
                        if (tokenToMerge.Type == JTokenType.Object && valueToken.Type == JTokenType.Object)
                        {
                            ProcessTransform((JObject)tokenToMerge, (JObject)valueToken);
                        }
                        else if (tokenToMerge.Type == JTokenType.Array && valueToken.Type == JTokenType.Array)
                        {
                            ((JArray)tokenToMerge).Merge(valueToken.DeepClone());
                        }
                        else
                        {
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
                ProcessTransform(source, mergeObject);
            }
        }
    }
}
