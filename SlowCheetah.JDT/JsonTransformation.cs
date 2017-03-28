// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a transformation based on a JSON file using JDT
    /// </summary>
    public class JsonTransformation
    {
        private readonly JObject transform;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTransformation"/> class.
        /// </summary>
        /// <param name="transformObject">Object that corresponds to the transformation file</param>
        public JsonTransformation(JObject transformObject)
        {
            if (transformObject == null)
            {
                throw new ArgumentNullException(nameof(transformObject));
            }

            this.transform = (JObject)transformObject.DeepClone();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTransformation"/> class.
        /// </summary>
        /// <param name="transformFile">File that defines the trasnformation</param>
        public JsonTransformation(string transformFile)
        {
            this.transform = JsonUtilities.LoadObjectFromFile(transformFile);
        }

        /// <summary>
        /// Apply the specified transformation
        /// </summary>
        /// <param name="document">Document to be transformed</param>
        public void Apply(JsonDocument document)
        {
            JdtProcessor.ProcessTransform(document.GetObject(), (JObject)this.transform.DeepClone());
        }
    }
}
