// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Defines extension methods used in JDT
    /// </summary>
    internal static class JdtExtensions
    {
        /// <summary>
        /// Merges an array into another.
        /// Merges a clone of the array.
        /// </summary>
        /// <param name="original">Array to merge into</param>
        /// <param name="arrayToMerge">Array to be merged</param>
        internal static void MergeInto(this JArray original, JArray arrayToMerge)
        {
            if (original == null)
            {
                throw new ArgumentNullException(nameof(original));
            }

            if (arrayToMerge == null)
            {
                throw new ArgumentNullException(nameof(arrayToMerge));
            }

            foreach (JToken token in arrayToMerge)
            {
                original.Add(token.DeepClone());
            }
        }

        /// <summary>
        /// Throws a <see cref="JdtException"/> if the given node is the root
        /// </summary>
        /// <param name="token">The token to verify</param>
        /// <param name="errorMessage">Error message</param>
        internal static void ThrowIfRoot(this JToken token, string errorMessage)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (token.Root.Equals(token))
            {
                throw new JdtException(errorMessage);
            }
        }
    }
}
