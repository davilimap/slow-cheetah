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

        private JsonTransformationLogger logger = null;

        private JsonTransformContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTransformation"/> class.
        /// </summary>
        /// <param name="transformObject">Object that corresponds to the transformation file</param>
        public JsonTransformation(JObject transformObject)
        {
            // TO DO: Evalute usefulness of this constructor. Potentially replace with constructor that takes in JSON string
            if (transformObject == null)
            {
                throw new ArgumentNullException(nameof(transformObject));
            }

            this.transform = (JObject)transformObject.DeepClone();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTransformation"/> class.
        /// </summary>
        /// <param name="transformFile">File that defines the transformation</param>
        public JsonTransformation(string transformFile)
            : this(transformFile, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTransformation"/> class
        /// with an external logger
        /// </summary>
        /// <param name="transformFile">File that defines the transformation</param>
        /// <param name="logger">External logger</param>
        public JsonTransformation(string transformFile, IJsonTransformationLogger logger)
        {
            this.logger = new JsonTransformationLogger(logger);

            this.transform = JsonUtilities.LoadObjectFromFile(transformFile);

            this.context = new JsonTransformContext()
            {
                TransformFile = transformFile,
                Logger = this.logger
            };
        }

        /// <summary>
        /// Apply the specified transformation
        /// </summary>
        /// <param name="document">Document to be transformed</param>
        /// <returns>True if the transformation was successfully applied</returns>
        public bool Apply(JsonDocument document)
        {
            try
            {
                this.logger.HasLoggedErrors = false;
                // this.context.SourceFile = document
                JdtProcessor.ProcessTransform(document.DocumentObject, (JObject)this.transform.DeepClone());
            }
            catch (Exception ex)
            {
                this.logger.LogErrorFromException(ex);
            }

            return this.logger.HasLoggedErrors;
        }
    }
}
