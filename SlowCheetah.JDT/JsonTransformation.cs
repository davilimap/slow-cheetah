// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a transformation based on a JSON file using JDT
    /// </summary>
    public class JsonTransformation
    {
        private readonly JsonTransformationContextLogger logger;

        private JObject transformObject;
        private JsonLoadSettings loadSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTransformation"/> class.
        /// </summary>
        /// <param name="transformFile">The path to the file that specifies the transformation</param>
        public JsonTransformation(string transformFile)
            : this(transformFile, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTransformation"/> class with an external logger.
        /// </summary>
        /// <param name="transformFile">The path to the file that specifies the transformation</param>
        /// <param name="logger">The external logger</param>
        public JsonTransformation(string transformFile, IJsonTransformationLogger logger)
        {
            if (string.IsNullOrEmpty(transformFile))
            {
                throw new ArgumentNullException(nameof(transformFile));
            }

            this.logger = new JsonTransformationContextLogger(transformFile, logger);

            using (FileStream transformStream = File.Open(transformFile, FileMode.Open))
            {
                this.SetTransform(transformStream);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTransformation"/> class.
        /// </summary>
        /// <param name="transform">The stream containing the JSON that specifies the transformation</param>
        public JsonTransformation(Stream transform)
            : this(transform, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTransformation"/> class with an external logger.
        /// </summary>
        /// <param name="transform">The stream containing the JSON that specifies the transformation</param>
        /// /// <param name="logger">The external logger</param>
        public JsonTransformation(Stream transform, IJsonTransformationLogger logger)
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            this.logger = new JsonTransformationContextLogger(logger);

            this.SetTransform(transform);
        }

        /// <summary>
        /// Transforms a JSON object
        /// </summary>
        /// <param name="sourceFile">The path to the json file to be transformed</param>
        /// <param name="result">The stream to write the result into</param>
        /// <returns>True if the transformations were completed</returns>
        public bool TryApply(string sourceFile, out Stream result)
        {
            if (string.IsNullOrEmpty(sourceFile))
            {
                throw new ArgumentNullException(nameof(sourceFile));
            }

            this.logger.SourceFile = sourceFile;

            // Open the file as streams and apply the transforms
            using (Stream sourceStream = File.Open(sourceFile, FileMode.Open))
            {
                return this.TryApply(sourceStream, out result);
            }
        }

        /// <summary>
        /// Transforms a JSON object
        /// </summary>
        /// <param name="source">The object to be transformed</param>
        /// <param name="result">The stream to write the result into</param>
        /// <returns>True if the transformations were completed</returns>
        public bool TryApply(Stream source, out Stream result)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            this.logger.SourceFile = null;

            using (StreamReader sourceStreamReader = new StreamReader(source))
            using (JsonTextReader sourceReader = new JsonTextReader(sourceStreamReader))
            {
                result = null;
                JObject sourceObject = null;

                try
                {
                    // The JObject corresponding to the streams with line info
                    sourceObject = JObject.Load(sourceReader, this.loadSettings);

                    // Execute the transforms
                    JdtProcessor.ProcessTransform(sourceObject, this.transformObject, this.logger);
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    this.logger.LogErrorFromException(ex);
                }
                finally
                {
                    if (!this.logger.HasLoggedErrors)
                    {
                        // Save the result to a memory stream
                        // Don't close the stream of the streamwriter so data isn't lost
                        // User should handle the close
                        result = new MemoryStream();
                        StreamWriter streamWriter = new StreamWriter(result);
                        JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter);

                        // Writes the changes in the source object to the stream
                        // and resets it so the user can read the stream
                        sourceObject.WriteTo(jsonWriter);
                        streamWriter.Flush();
                        result.Position = 0;
                    }
                }

                return !this.logger.HasLoggedErrors;
            }
        }

        private void SetTransform(Stream transformStream)
        {
            this.loadSettings = new JsonLoadSettings()
            {
                CommentHandling = CommentHandling.Ignore,
                LineInfoHandling = LineInfoHandling.Load
            };

            using (StreamReader transformStreamReader = new StreamReader(transformStream))
            using (JsonTextReader transformReader = new JsonTextReader(transformStreamReader))
            {
                this.transformObject = JObject.Load(transformReader, this.loadSettings);
            }
        }
    }
}
