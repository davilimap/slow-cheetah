// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
    }
}
