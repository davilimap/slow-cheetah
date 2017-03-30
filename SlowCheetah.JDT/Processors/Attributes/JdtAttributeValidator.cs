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
        /// Represents an non existant attribute
        /// </summary>
        [Description(null)]
        None = 0,

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
        /// Validates the object and returns the appropriate attributes contained within it
        /// </summary>
        /// <param name="transformObject">The object to validade</param>
        /// <returns>A dictionary with the JToken attributes of each valid attribute</returns>
        internal Dictionary<JdtAttributes, JToken> ValidateAndReturnAttributes(JObject transformObject)
        {
            Dictionary<JdtAttributes, JToken> attributes = new Dictionary<JdtAttributes, JToken>();

            foreach (JProperty property in transformObject.GetJdtProperties())
            {
                JdtAttributes attribute = this.validAttributes.GetByName(property.Name);
                if (attribute == JdtAttributes.None)
                {
                    // If the attribute is not supported in this transformation, throw
                    // TO DO: Specify the transformation in the error
                    throw new JdtException($"{property.Name} is not a valid attribute for this transformation");
                }
                else
                {
                    attributes.Add(attribute, property.Value);
                }
            }

            // If the object has attributes, it should not have any other properties in it
            if (attributes.Count > 0 && attributes.Count != transformObject.Properties().Count())
            {
                throw new JdtException("Invalid transformation attributes");
            }

            return attributes;
        }
    }
}
