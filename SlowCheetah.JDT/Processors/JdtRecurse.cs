// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a recursive JDT transformation
    /// </summary>
    internal class JdtRecurse : JdtProcessor
    {
        /// <inheritdoc/>
        public override string Verb { get; } = null;

        /// <inheritdoc/>
        public override void Process(JObject source, JObject transform)
        {
            // Nodes that should be removed from the transform after they are handled
            List<string> nodesToRemove = new List<string>();

            foreach (JProperty transformNode in transform.Properties()
                .Where(p => p.Value.Type == JTokenType.Object && !JsonUtilities.IsJdtSyntax(p.Name)))
            {
                JToken sourceChild;
                if (source.TryGetValue(transformNode.Name, out sourceChild) && sourceChild.Type == JTokenType.Object)
                {
                    ProcessTransform((JObject)sourceChild, (JObject)transformNode.Value);

                    // If we have already recursed into that node, it should be removed from the transform
                    nodesToRemove.Add(transformNode.Name);
                }
            }

            // Remove all of the previously handled nodes
            nodesToRemove.ForEach(node => transform.Remove(node));

            // Continue to next transformation
            this.Successor.Process(source, transform);
        }
    }
}
