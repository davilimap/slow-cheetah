﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Implements extensions for <see cref="JdtAttributes"/>
    /// </summary>
    internal static class AttributeExtensions
    {
        /// <summary>
        /// Gets the description (name) of the attribute
        /// </summary>
        /// <param name="attribute">The attribute</param>
        /// <returns>The name of the attribute</returns>
        internal static string GetDescription(this JdtAttributes attribute)
        {
            var type = attribute.GetType();
            var name = Enum.GetName(type, attribute);
            var description = type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<DescriptionAttribute>()
                .SingleOrDefault();

            if (description == null)
            {
                throw new NotImplementedException(attribute.ToString() + " does not have a corresponding name");
            }

            return description.Description;
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
