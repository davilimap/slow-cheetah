// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a transformation
    /// </summary>
    internal abstract partial class JdtProcessor
    {
        private static readonly JdtProcessorChain ProcessorChain = new JdtProcessorChain();

        private JdtProcessor successor;

        /// <summary>
        /// Gets the JDT verb corresponding to this transformation.
        /// Can be null or empty.
        /// Does not include the preffix(<see cref="JsonUtilities.JdtSyntaxPrefix"/>)
        /// </summary>
        internal abstract string Verb { get; }

        /// <summary>
        /// Gets the successor of the current transformation
        /// </summary>
        protected JdtProcessor Successor
        {
            get
            {
                // Defaults to the end of chain processor
                return this.successor ?? JdtEndOfChain.Instance;
            }

            private set
            {
                this.successor = value;
            }
        }

        /// <summary>
        /// Executes the entire transformation with the given objects
        /// </summary>
        /// <param name="source">Object to be transformed</param>
        /// <param name="transform">Object that specifies the transformation</param>
        /// <param name="context">The context of the transformation</param>
        internal static void ProcessTransform(JObject source, JObject transform, JsonTransformContext context)
        {
            ProcessorChain.Start(source, transform, context);
        }

        /// <summary>
        /// Executes the transformation
        /// </summary>
        /// <param name="source">Object to be transformed</param>
        /// <param name="transform">Object specifying the transformation</param>
        /// <param name="context">The context of the transformation</param>
        internal abstract void Process(JObject source, JObject transform, JsonTransformContext context);
    }
}
