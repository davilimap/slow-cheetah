﻿namespace SlowCheetah.JDT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a recursive JDT transformation
    /// </summary>
    internal class JdtRecurse : JdtProcessor
    {
        /// <inheritdoc/>
        internal override string Verb { get; } = null;

        /// <inheritdoc/>
        internal override void Process(JObject source, JObject transform, JsonTransformContext context)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            // Nodes that should be removed from the transform after they are handled
            var nodesToRemove = new List<string>();

            foreach (JProperty transformNode in transform.Properties()
                .Where(p => p.Value.Type == JTokenType.Object && !JsonUtilities.IsJdtSyntax(p.Name)))
            {
                // We recurse into objects that do not correspond to JDT verbs and that exist in both source and transform
                JToken sourceChild;
                if (source.TryGetValue(transformNode.Name, out sourceChild) && sourceChild.Type == JTokenType.Object)
                {
                    ProcessTransform((JObject)sourceChild, (JObject)transformNode.Value, context);

                    // If we have already recursed into that node, it should be removed from the transform
                    nodesToRemove.Add(transformNode.Name);
                }
            }

            // Remove all of the previously handled nodes
            // This is necessary so that a rename does not cause a node to be hadled twice
            nodesToRemove.ForEach(node => transform.Remove(node));

            // Continue to next transformation
            this.Successor.Process(source, transform, context);
        }
    }
}
