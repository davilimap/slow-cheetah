﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
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
    /// Validator for JDT attributes within a verb object
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
        /// Validates the object and returns the appropriate attributes contained within it.
        /// Throws if an invalid jdt attribute is found.
        /// </summary>
        /// <param name="transformObject">The object to validade</param>
        /// <returns>A dictionary with the JToken attributes of each valid attribute.
        /// An empty dictionary is returned if no valid properties are found</returns>
        internal Dictionary<JdtAttributes, JToken> ValidateAndReturnAttributes(JObject transformObject)
        {
            Dictionary<JdtAttributes, JToken> attributes = new Dictionary<JdtAttributes, JToken>();

            // First, we look through all of the properties that have JDT syntax
            foreach (JProperty property in transformObject.GetJdtProperties())
            {
                JdtAttributes attribute = this.validAttributes.GetByName(property.Name);
                if (attribute == JdtAttributes.None)
                {
                    // TO DO: Specify the transformation in the error
                    // If the attribute is not supported in this transformation, throw
                    throw JdtException.FromLineInfo($"{property.Name} is not a valid attribute for this transformation", ErrorLocation.Transform, property);
                }
                else
                {
                    // If it is a valid JDT attribute, add its value to the dictionary
                    attributes.Add(attribute, property.Value);
                }
            }

            // If the object has attributes, it should not have any other properties in it
            if (attributes.Count > 0 && attributes.Count != transformObject.Properties().Count())
            {
                throw JdtException.FromLineInfo("Invalid transformation attributes", ErrorLocation.Transform, transformObject);
            }

            return attributes;
        }
    }
}
