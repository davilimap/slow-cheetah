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
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Valid JDT attributes
    /// </summary>
    internal enum JdtAttributes
    {
        /// <summary>
        /// The JDT path attribute
        /// </summary>
        [Description("path")]
        Path,

        /// <summary>
        /// The JDT path attribute
        /// </summary>
        [Description("value")]
        Value,
    }

    /// <summary>
    /// Validator for JDT attributes
    /// </summary>
    internal class JdtAttributeValidator
    {
        private List<JdtAttributes> validAttributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="JdtAttributeValidator"/> class.
        /// </summary>
        /// <param name="validAttributes">The attributes that are valid</param>
        internal JdtAttributeValidator(params JdtAttributes[] validAttributes)
        {
            this.validAttributes = validAttributes.ToList();
        }

        /// <summary>
        /// Get the full name of an attribute, with the JDT prefix
        /// </summary>
        /// <param name="attribute">The attribute</param>
        /// <returns>A string with the full name of the requested attribute</returns>
        internal static string FullName(JdtAttributes attribute)
        {
            return JsonUtilities.JdtSyntaxPrefix + attribute.GetDescription();
        }

        /// <summary>
        /// Validates the object and returns the appropriate attributes contained within it
        /// </summary>
        /// <param name="transformObject">The object to validade</param>
        /// <returns>A dictionary with the JToken attributes of each valid attribute</returns>
        internal Dictionary<JdtAttributes, JToken> ValidateAndReturnAttributes(JObject transformObject)
        {
            Dictionary<JdtAttributes, JToken> attributes = new Dictionary<JdtAttributes, JToken>();

            foreach (JdtAttributes attribute in this.validAttributes)
            {
                JToken attributeToken;
                if (transformObject.TryGetValue(FullName(attribute), out attributeToken))
                {
                    attributes.Add(attribute, attributeToken);
                }
            }

            // If the object has attributes, it should not have any other properties in it
            bool objectHasInvalidProperties = transformObject.Properties().Any(p => !this.validAttributes.Select(a => FullName(a)).Contains(p.Name));
            if (attributes.Count > 0 && objectHasInvalidProperties)
            {
                throw new JdtException("Invalid transformation attributes");
            }

            return attributes;
        }
    }
}
