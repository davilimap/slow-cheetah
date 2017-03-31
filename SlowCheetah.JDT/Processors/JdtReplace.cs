namespace SlowCheetah.JDT
{
    using System;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a recursive JDT transformation
    /// </summary>
    internal class JdtReplace : JdtArrayProcessor
    {
        private const string PathAttribute = "path";
        private const string ValueAttribute = "value";

        /// <inheritdoc/>
        public override string Verb { get; } = "replace";

        /// <inheritdoc/>
        protected override bool ProcessCore(JObject source, JToken transformValue)
        {
            switch (transformValue.Type)
            {
                case JTokenType.Object:
                    return this.ReplaceWithProperties(source, (JObject)transformValue);
                default:
                    source.Replace(transformValue);

                    // If the node is replaced, stop transformations on it
                    return false;
            }
        }

        private bool ReplaceWithProperties(JObject source, JObject replaceObject)
        {
            // TO DO: Verify if the replace value contains JDT syntax
            JToken pathToken, valueToken;
            string pathFullAttribute = JsonUtilities.JdtSyntaxPrefix + PathAttribute;
            bool hasPath = replaceObject.TryGetValue(pathFullAttribute, out pathToken);
            string valueFullAttribute = JsonUtilities.JdtSyntaxPrefix + ValueAttribute;
            bool hasValue = replaceObject.TryGetValue(valueFullAttribute, out valueToken);

            if (!hasPath && !hasValue)
            {
                source.Replace(replaceObject);

                // If the node is replaced, stop transformations on it
                return false;
            }
            else if (hasPath && hasValue)
            {
                if (replaceObject.Properties().Where(p => !p.Name.Equals(pathFullAttribute) && !p.Name.Equals(valueFullAttribute)).Count() > 0)
                {
                    throw new JdtException("Replace only accepts path and value attributes");
                }

                if (pathToken.Type != JTokenType.String)
                {
                    throw new JdtException("Path attribute must be a string");
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
                throw new JdtException("Replace requires both path and value");
            }

            return true;
        }
    }
}
