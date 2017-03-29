// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

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
        public override void Process(JObject source, JObject transform)
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

            this.Successor.Process(source, transform);
        }
    }
}
