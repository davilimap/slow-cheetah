// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a recursive JDT transformation
    /// </summary>
    internal class JdtRemove : JdtProcessor
    {
        private const string PathAttribute = "path";

        private bool removedThisNode;

        /// <inheritdoc/>
        public override string Verb { get; } = "remove";

        /// <inheritdoc/>
        public override void Process(JObject source, JObject transform)
        {
            this.removedThisNode = false;

            JToken removeValue;
            if (transform.TryGetValue(JsonUtilities.JdtSyntaxPrefix + this.Verb, out removeValue))
            {
                this.Remove(source, removeValue, true);
            }

            if (!this.removedThisNode)
            {
                // If the current node was removed, then do not perform any more transformations here
                this.Successor.Process(source, transform);
            }
        }

        private void Remove(JObject source, JToken removeValue, bool allowArray)
        {
            switch (removeValue.Type)
            {
                case JTokenType.String:
                    // TO DO: warning if unable to remove
                    // If the value is just a string, remove that node
                    source.Remove(removeValue.ToObject<string>());
                    break;
                case JTokenType.Boolean:
                    if (removeValue.ToObject<bool>())
                    {
                        // If the transform value is true, remove the entire node
                        this.RemoveThisNode(source);

                        // Stop transformations if the node was removed
                        return;
                    }

                    break;
                case JTokenType.Object:
                    // If the value is an object, verify the attributes within and perform the remove
                    this.RemoveWithAttributes(source, (JObject)removeValue);
                    break;
                case JTokenType.Array:
                    if (allowArray)
                    {
                        // If the value is an array, perform the remove for each object in the array
                        // Do not allow array values from here
                        foreach (JToken arayValue in (JArray)removeValue)
                        {
                            this.Remove(source, arayValue, false);
                            if (this.removedThisNode)
                            {
                                // If a value in the array performs a remove of the current node,
                                // Stop transformations
                                return;
                            }
                        }
                    }
                    else
                    {
                        // TO DO: Clarify error
                        throw new JdtException(removeValue.Type.ToString() + " is not a valid transform value");
                    }

                    break;
                default:
                    throw new JdtException(removeValue.Type.ToString() + " is not a valid transform value for Remove");
            }
        }

        private void RemoveWithAttributes(JObject source, JObject removeObject)
        {
            string pathFullAttribute = JsonUtilities.JdtSyntaxPrefix + PathAttribute;
            if (removeObject.Properties().Where(p => !p.Name.Equals(pathFullAttribute)).Count() > 0)
            {
                // If any properties other than the path attribute are found
                throw new JdtException("Invalid remove attributes");
            }

            JToken pathToken;
            if (removeObject.TryGetValue(pathFullAttribute, out pathToken))
            {
                if (pathToken.Type == JTokenType.String)
                {
                    // Gets the tokens to be removed and converts to a list
                    // so that the tokens may be removed
                    var tokensToRemove = JsonUtilities.GetTokensFromPath(source, pathToken.ToString());
                    foreach (JToken token in tokensToRemove.ToList())
                    {
                        if (token.Equals(source))
                        {
                            // If the path specifies the current node
                            this.RemoveThisNode(source);
                            return;
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
        }

        private void RemoveThisNode(JObject nodeToRemove)
        {
            // If the value is true, everything from the node and set it to null
            if (nodeToRemove.Root.Equals(nodeToRemove))
            {
                throw new JdtException("You cannot remove the root");
            }

            JProperty parent = (JProperty)nodeToRemove.Parent;
            if (parent == null)
            {
                // TO DO: Log error line
                throw new JdtException("Could not perform remove");
            }
            else
            {
                parent.Value = null;
            }

            // Informs not to perform any more transformations on this node
            this.removedThisNode = true;
        }
    }
}
