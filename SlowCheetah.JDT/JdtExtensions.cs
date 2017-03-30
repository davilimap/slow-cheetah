// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Defines extension methods used in JDT
    /// </summary>
    internal static class JdtExtensions
    {
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

        /// <summary>
        /// Gets all the properties within the object that correspond to JDT syntax
        /// </summary>
        /// <param name="objectToSearch">The object to search</param>
        /// <returns>An enumerable of properties that start with the JDT prefix</returns>
        internal static IEnumerable<JProperty> GetJdtProperties(this JObject objectToSearch)
        {
            return objectToSearch.Properties().Where(p => JsonUtilities.IsJdtSyntax(p.Name));
        }
    }
}
