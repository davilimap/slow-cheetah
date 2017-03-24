﻿// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a JSON file to be transformed
    /// </summary>
    public class JsonDocument : IEquatable<JsonDocument>
    {
        private readonly string documentPath;
        private readonly JObject documentObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDocument"/> class.
        /// </summary>
        /// <param name="filePath">Path to the JSON file</param>
        public JsonDocument(string filePath)
        {
            this.documentPath = filePath;
            this.documentObject = JsonUtilities.LoadObjectFromFile(this.documentPath);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDocument"/> class.
        /// </summary>
        /// <param name="docObject">The object to be transformed</param>
        public JsonDocument(JObject docObject)
        {
            this.documentPath = string.Empty;
            this.documentObject = (JObject)docObject.DeepClone();
        }

        /// <inheritdoc/>
        public bool Equals(JsonDocument other)
        {
            return JToken.DeepEquals(this.documentObject, other.documentObject);
        }

        /// <summary>
        /// Saves the document to a file
        /// </summary>
        /// <param name="destinationFile">File to save </param>
        public void Save(string destinationFile)
        {
            if (string.IsNullOrEmpty(destinationFile))
            {
                throw new ArgumentNullException(nameof(destinationFile));
            }

            using (StreamWriter file = File.CreateText(destinationFile))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                writer.Formatting = Formatting.Indented;
                this.documentObject.WriteTo(writer);
            }
        }

        /// <summary>
        /// Gets the object corresponding to the JSON document
        /// </summary>
        /// <returns>The object corresponding to the root of the document</returns>
        internal JObject GetObject()
        {
            return this.documentObject;
        }
    }
}
