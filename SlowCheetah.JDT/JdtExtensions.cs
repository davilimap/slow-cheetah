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

        /// <summary>
        /// Get the full name of an attribute, with the JDT prefix
        /// </summary>
        /// <param name="attribute">The attribute</param>
        /// <returns>A string with the full name of the requested attribute</returns>
        internal static string FullName(this JdtAttributes attribute)
        {
            return JsonUtilities.JdtSyntaxPrefix + attribute.GetDescription();
        }

        /// <summary>
        /// Gets a <see cref="JdtAttributes"/> from an enumerable based on name
        /// </summary>
        /// <param name="collection">The enumerable to search</param>
        /// <param name="name">The name of the attribute</param>
        /// <returns>The attribute with that name of <see cref="JdtAttributes.None"/> if no attribute was found</returns>
        internal static JdtAttributes GetByName(this IEnumerable<JdtAttributes> collection, string name)
        {
            return collection.SingleOrDefault(a => a.FullName().Equals(name));
        }
    }
}
