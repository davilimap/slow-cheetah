// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the Merge transformation
    /// </summary>
    internal class JdtMerge : JdtArrayProcessor
    {
        private readonly JdtAttributeValidator attributeValidator;

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
        protected override bool ProcessCore(JObject source, JToken transformValue, JsonTransformationContextLogger logger)
        {
            if (transformValue.Type == JTokenType.Object)
            {
                // If both source and transform are objects,
                // analyze the contents and perform the appropriate transforms
                this.MergeWithObject(source, (JObject)transformValue, logger);
            }
            else
            {
                // If the transformation is trying to replace the root with a non-object, throw
                if (source.Root.Equals(source))
                {
                    throw JdtException.FromLineInfo(Resources.ErrorMessage_ReplaceRoot, ErrorLocation.Transform, transformValue);
                }

                // If the transform value is not an object, then simply replace it with the new token
                source.Replace(transformValue);
            }

            // Do not halt transformations
            return true;
        }

        private void MergeWithObject(JObject source, JObject mergeObject, JsonTransformationContextLogger logger)
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
                        throw JdtException.FromLineInfo(Resources.ErrorMessage_PathContents, ErrorLocation.Transform, mergeObject);
                    }

                    foreach (JToken tokenToMerge in source.SelectTokens(pathToken.ToString()).ToList())
                    {
                        // Perform the merge for each element found through the path
                        if (tokenToMerge.Type == JTokenType.Object && valueToken.Type == JTokenType.Object)
                        {
                            // If they are both objects, start a new transformation
                            ProcessTransform((JObject)tokenToMerge, (JObject)valueToken, logger);
                        }
                        else if (tokenToMerge.Type == JTokenType.Array && valueToken.Type == JTokenType.Array)
                        {
                            // If they are both arrays, add the new values to the original
                            ((JArray)tokenToMerge).Merge(valueToken.DeepClone());
                        }
                        else
                        {
                            // If the transformation is trying to replace the root with a non-object, throw
                            if (tokenToMerge.Root.Equals(tokenToMerge))
                            {
                                throw JdtException.FromLineInfo(Resources.ErrorMessage_ReplaceRoot, ErrorLocation.Transform, mergeObject);
                            }

                            // If they are primitives or have different values, perform a replace
                            tokenToMerge.Replace(valueToken);
                        }
                    }
                }
                else
                {
                    // If either is not present, throw
                    throw JdtException.FromLineInfo(Resources.ErrorMessage_MergeAttributes, ErrorLocation.Transform, mergeObject);
                }
            }
            else
            {
                // If the merge object does not contain attributes,
                // simply execute the transform with that object
                ProcessTransform(source, mergeObject, logger);
            }
        }
    }
}
