namespace SlowCheetah.JDT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Validates the JDT verbs in the transformation
    /// </summary>
    internal class JdtValidator : JdtProcessor
    {
        /// <summary>
        /// Gets set of the valid verbs for the transformation
        /// </summary>
        public HashSet<string> ValidVerbs { get; } = new HashSet<string>();

        /// <inheritdoc/>
        public override string Verb { get; } = null;

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

            foreach (JProperty transformNode in transform.Properties()
                .Where(p => JsonUtilities.IsJdtSyntax(p.Name)))
            {
                string verb = JsonUtilities.GetJdtSyntax(transformNode.Name);
                if (verb != null)
                {
                    if (!this.ValidVerbs.Contains(verb))
                    {
                        throw new JdtException(verb + " is not a valid JDT verb");
                    }
                }
            }

            this.Successor.Process(source, transform, context);
        }
    }
}
