namespace SlowCheetah.JDT
{
    using System;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the default JDT transformation
    /// </summary>
    internal class JdtDefault : JdtProcessor
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
                        ((JArray)nodeToTransform).Merge(transformNode.Value.DeepClone());
                    }
                    else if (nodeToTransform.Type != JTokenType.Object || transformNode.Value.Type != JTokenType.Object)
                    {
                        // TO DO: Verify if object has JDT verbs. They shouldn't be allowed here because they won't be processed
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
