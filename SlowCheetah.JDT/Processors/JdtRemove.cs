namespace SlowCheetah.JDT
{
    using System;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a recursive JDT transformation
    /// </summary>
    internal class JdtRemove : JdtArrayProcessor
    {
        private JdtAttributeValidator attributeValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="JdtRemove"/> class.
        /// </summary>
        public JdtRemove()
        {
            this.attributeValidator = new JdtAttributeValidator(JdtAttributes.Path);
        }

        /// <inheritdoc/>
        public override string Verb { get; } = "remove";

        /// <inheritdoc/>
        protected override bool ProcessCore(JObject source, JToken transformValue)
        {
            switch (transformValue.Type)
            {
                case JTokenType.String:
                    // TO DO: warning if unable to remove
                    // If the value is just a string, remove that node
                    source.Remove(transformValue.ToString());
                    break;
                case JTokenType.Boolean:
                    if ((bool)transformValue)
                    {
                        // If the transform value is true, remove the entire node
                        if (!this.RemoveThisNode(source))
                        {
                            return false;
                        }
                    }

                    break;
                case JTokenType.Object:
                    // If the value is an object, verify the attributes within and perform the remove
                    return this.RemoveWithAttributes(source, (JObject)transformValue);
                default:
                    throw new JdtException(transformValue.Type.ToString() + " is not a valid transform value for Remove");
            }

            return true;
        }

        private bool RemoveWithAttributes(JObject source, JObject removeObject)
        {
            var attributes = this.attributeValidator.ValidateAndReturnAttributes(removeObject);

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
                    throw new JdtException("Path attribute must be a string");
                }
            }
            else
            {
                throw new JdtException("Remove transformation requires the path attribute");
            }

            return true;
        }

        private bool RemoveThisNode(JObject nodeToRemove)
        {
            JsonUtilities.ThrowIfRoot(nodeToRemove, "You cannot remove the root");

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
