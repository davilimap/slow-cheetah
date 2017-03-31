namespace SlowCheetah.JDT
{
    using System;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Base for a processor that handles array values
    /// </summary>
    internal abstract class JdtArrayProcessor : JdtProcessor
    {
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

            JToken transformValue;
            if (transform.TryGetValue(this.FullVerb, out transformValue))
            {
                if (!this.Transform(source, transformValue, context))
                {
                    // If the transformation returns false,
                    // it performed an operation that halts transforms
                    return;
                }
            }

            this.Successor.Process(source, transform, context);
        }

        /// <summary>
        /// The core transformation logic. Arrays are treated as the transform values
        /// </summary>
        /// <param name="source">Object to be transformed</param>
        /// <param name="transformValue">Value of the transform</param>
        /// <param name="context">The transformation context</param>
        /// <returns>True if transforms should continue</returns>
        protected abstract bool ProcessCore(JObject source, JToken transformValue, JsonTransformContext context);

        /// <summary>
        /// Performs the initial logic of processing arrays.
        /// Arrays cause the transform to be applied to each value in them
        /// </summary>
        /// <param name="source">Object to be transformed</param>
        /// <param name="transformValue">Value of the transform</param>
        /// <param name="context">The transformation context</param>
        /// <returns>True if transforms should continue</returns>
        private bool Transform(JObject source, JToken transformValue, JsonTransformContext context)
        {
            if (transformValue.Type == JTokenType.Array)
            {
                // If the value is an array, perform the transformation for each object in the array
                // From here, arrays are handled as the transformation value
                foreach (JToken arrayValue in (JArray)transformValue)
                {
                    if (!this.ProcessCore(source, arrayValue, context))
                    {
                        // If the core transformation indicates a halt, we halt
                        return true;
                    }
                }

                // If we are not told to stop, we continue with transformations
                return true;
            }
            else
            {
                // If it is not an array, perform the transformation as normal
                return this.ProcessCore(source, transformValue, context);
            }
        }
    }
}
