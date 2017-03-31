﻿namespace SlowCheetah.JDT
{
    using System;
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
        /// Does not include the preffix (@jdt.)
        /// </summary>
        public abstract string Verb { get; }

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
        /// Mutates the source object
        /// </summary>
        /// <param name="source">Object to be transformed</param>
        /// <param name="transform">Object that specifies the transformation</param>
        public static void ProcessTransform(JObject source, JObject transform)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            ProcessorChain.Start(source, (JObject)transform.DeepClone());
        }

        /// <summary>
        /// Executes the transformation
        /// </summary>
        /// <param name="source">Object to be transformed</param>
        /// <param name="transform">Object specifying the transformation</param>
        public abstract void Process(JObject source, JObject transform);
    }
}
