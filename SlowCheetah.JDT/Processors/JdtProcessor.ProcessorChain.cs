// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The JdtProcessor chain
    /// </summary>
    public abstract partial class JdtProcessor
    {
        private class JdtProcessorChain
        {
            // This is a a list of supported transformations
            // It is in order of execution
            private readonly List<JdtProcessor> processors = new List<JdtProcessor>()
            {
                // Validates the JDT verbs in the file
                new JdtValidator(),

                // Supported transformations
                new JdtRecurse(),
                new JdtRemove(),
                new JdtReplace(),
                new JdtRename(),
                new JdtMerge(),
                new JdtDefault()
            };

            public JdtProcessorChain()
            {
                var validator = this.processors.First() as JdtValidator;

                // The successor of each transform processor should be the next one on the list
                // The last processor defaults to the end of chain processor
                var processorsEnumerator = this.processors.GetEnumerator();
                processorsEnumerator.MoveNext();
                foreach (var successor in this.processors.Skip(1))
                {
                    if (!string.IsNullOrEmpty(successor.Verb))
                    {
                        validator.ValidVerbs.Add(successor.Verb);
                    }

                    processorsEnumerator.Current.Successor = successor;
                    processorsEnumerator.MoveNext();
                }
            }

            public void Start(JObject source, JObject transform)
            {
                if (source == null)
                {
                    throw new ArgumentNullException(nameof(source));
                }

                if (transform == null)
                {
                    throw new ArgumentNullException(nameof(transform));
                }

                this.processors.First().Process(source, transform);
            }
        }

        /// <summary>
        /// Represents the end of the transformation chain
        /// </summary>
        private class JdtEndOfChain : JdtProcessor
        {
            private JdtEndOfChain()
            {
            }

            public static JdtEndOfChain Instance { get; } = new JdtEndOfChain();

            public override string Verb { get; } = null;

            public override void Process(JObject source, JObject transform)
            {
                // Do nothing, the chain is done
            }
        }
    }
}
