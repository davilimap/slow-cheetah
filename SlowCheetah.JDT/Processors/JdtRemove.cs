// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the Remove transformation
    /// </summary>
    internal class JdtRemove : JdtArrayProcessor
    {
        private JdtAttributeValidator attributeValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="JdtRemove"/> class.
        /// </summary>
        public JdtRemove()
        {
            // Remove only accepts the path attribute
            this.attributeValidator = new JdtAttributeValidator(JdtAttributes.Path);
        }

        /// <inheritdoc/>
        public override string Verb { get; } = "remove";

        /// <inheritdoc/>
        protected override bool ProcessCore(JObject source, JToken transformValue, JsonTransformationContextLogger logger)
        {
            switch (transformValue.Type)
            {
                case JTokenType.String:
                    // TO DO: warning if unable to remove
                    // If the value is just a string, remove that node
                    if (!source.Remove(transformValue.ToString()))
                    {
                        logger.LogWarning("Unable to remove node", ErrorLocation.Transform, transformValue);
                    }

                    break;
                case JTokenType.Boolean:
                    if ((bool)transformValue)
                    {
                        // If the transform value is true, remove the entire node
                        return this.RemoveThisNode(source);
                    }

                    break;
                case JTokenType.Object:
                    // If the value is an object, verify the attributes within and perform the remove
                    return this.RemoveWithAttributes(source, (JObject)transformValue);
                default:
                    throw JdtException.FromLineInfo($"{transformValue.Type.ToString()} is not a valid transform value for Remove", ErrorLocation.Transform, transformValue);
            }

            // If nothing indicates a halt, continue with transforms
            return true;
        }

        private bool RemoveWithAttributes(JObject source, JObject removeObject)
        {
            var attributes = this.attributeValidator.ValidateAndReturnAttributes(removeObject);

            // The remove attribute only accepts objects if they have only the path attribute
            JToken pathToken;
            if (attributes.TryGetValue(JdtAttributes.Path, out pathToken))
            {
                if (pathToken.Type == JTokenType.String)
                {
                    // Removes all of the tokens specified by the path
                    foreach (JToken token in source.SelectTokens(pathToken.ToString()).ToList())
                    {
                        if (token.Equals(source))
                        {
                            // If the path specifies the current node
                            if (!this.RemoveThisNode(source))
                            {
                                // Halt transformations
                                return false;
                            }
                        }
                        else
                        {
                            if (token.Parent.Type == JTokenType.Property)
                            {
                                // If the token is the value of a property,
                                // the property must be removed
                                token.Parent.Remove();
                            }
                            else
                            {
                                // If the token is a property or an element in an array,
                                // it must be removed directly
                                token.Remove();
                            }
                        }
                    }
                }
                else
                {
                    throw JdtException.FromLineInfo("Path attribute must be a string", ErrorLocation.Transform, pathToken);
                }
            }
            else
            {
                throw JdtException.FromLineInfo("Remove transformation requires the path attribute", ErrorLocation.Transform, removeObject);
            }

            // If nothing indicates a halt, continue transforms
            return true;
        }

        private bool RemoveThisNode(JObject nodeToRemove)
        {
            // Removes the give node
            nodeToRemove.ThrowIfRoot("You cannot remove the root");

            var parent = (JProperty)nodeToRemove.Parent;
            if (parent == null)
            {
                // TO DO: Potentially log a warning and continue with transformations
                throw new JdtException("Could not perform remove");
            }
            else
            {
                parent.Value = null;
            }

            // Informs not to perform any more transformations on this node
            return false;
        }
    }
}
