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

        /// <inheritdoc/>
        public override string Verb { get; } = "remove";

        /// <inheritdoc/>
        public override void Process(JObject source, JObject transform)
        {
            JToken removeValue;
            if (transform.TryGetValue(JsonUtilities.JdtSyntaxPrefix + this.Verb, out removeValue))
            {
                if (this.Remove(source, removeValue))
                {
                    // If the current node was removed, then do not perform any more transformations here
                    return;
                }
            }

            this.Successor.Process(source, transform);
        }

        private bool Remove(JObject source, JToken removeValue)
        {
            if (removeValue.Type == JTokenType.Array)
            {
                // If the value is an array, perform the remove for each object in the array
                // Do not allow array values from here
                foreach (JToken arrayValue in (JArray)removeValue)
                {
                    if (this.RemoveCore(source, arrayValue))
                    {
                        // If a value in the array performs a remove of the current node,
                        // stop transformations
                        return true;
                    }
                }

                return false;
            }
            else
            {
                // If it is not an array, perform the merge as normal
                return this.RemoveCore(source, removeValue);
            }
        }

        private bool RemoveCore(JObject source, JToken removeValue)
        {
            switch (removeValue.Type)
            {
                case JTokenType.String:
                    // TO DO: warning if unable to remove
                    // If the value is just a string, remove that node
                    source.Remove(removeValue.ToString());
                    break;
                case JTokenType.Boolean:
                    if ((bool)removeValue)
                    {
                        // If the transform value is true, remove the entire node
                        if (this.RemoveThisNode(source))
                        {
                            return true;
                        }
                    }

                    break;
                case JTokenType.Object:
                    // If the value is an object, verify the attributes within and perform the remove
                    this.RemoveWithAttributes(source, (JObject)removeValue);
                    break;
                default:
                    throw new JdtException(removeValue.Type.ToString() + " is not a valid transform value for Remove");
            }

            return false;
        }

        private bool RemoveWithAttributes(JObject source, JObject removeObject)
        {
            string pathFullAttribute = JsonUtilities.JdtSyntaxPrefix + PathAttribute;
            if (removeObject.Properties().Any(p => !p.Name.Equals(pathFullAttribute)))
            {
                // If any properties other than the path attribute are found
                throw new JdtException("Invalid remove attributes");
            }

            JToken pathToken;
            if (removeObject.TryGetValue(pathFullAttribute, out pathToken))
            {
                if (pathToken.Type == JTokenType.String)
                {
                    // Removes all of the tokens specified by the path
                    foreach (JToken token in JsonUtilities.GetTokensFromPath(source, pathToken.ToString()))
                    {
                        if (token.Equals(source))
                        {
                            // If the path specifies the current node
                            if (this.RemoveThisNode(source))
                            {
                                return true;
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

            return false;
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
            return true;
        }
    }
}
