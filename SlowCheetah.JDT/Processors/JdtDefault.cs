// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a recursive JDT transformation
    /// </summary>
    internal class JdtDefault : JdtProcessor
    {
        /// <inheritdoc/>
        internal override string Verb { get; } = null;

        /// <inheritdoc/>
        internal override void Process(JObject source, JObject transform, JsonTransformContext context)
        {
            // JDT Verbs are not handled here
            foreach (JProperty transformNode in transform.Properties()
                .Where(p => !JsonUtilities.IsJdtSyntax(p.Name)))
            {
                JToken nodeToTransform;
                if (source.TryGetValue(transformNode.Name, out nodeToTransform))
                {
                    // If the node is present in both transform and source, analyze the types
                    // If both are objects, that is a recursive transformation, not handled here
                    if (nodeToTransform.Type == JTokenType.Array && transformNode.Value.Type == JTokenType.Array)
                    {
                        // If the original and transform are arrays, merge the contents together
                        JsonUtilities.MergeArray((JArray)nodeToTransform, (JArray)transformNode.Value);
                    }
                    else if (nodeToTransform.Type != JTokenType.Object || transformNode.Value.Type != JTokenType.Object)
                    {
                        // TO DO: Verify if object has JDT verbs
                        // If the contents are different, execute the replace
                        source[transformNode.Name] = transformNode.Value.DeepClone();
                    }
                }
                else
                {
                    // If the node is not present in the original, add it
                    source.Add(transformNode.DeepClone());
                }
            }

            this.Successor.Process(source, transform, context);
        }
    }
}
