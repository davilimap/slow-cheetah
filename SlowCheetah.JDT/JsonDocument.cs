// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
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

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDocument"/> class.
        /// </summary>
        /// <param name="filePath">Path to the JSON file</param>
        public JsonDocument(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            this.documentPath = filePath;
            this.DocumentObject = JsonUtilities.LoadObjectFromFile(this.documentPath);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDocument"/> class.
        /// </summary>
        /// <param name="docObject">The object to be transformed</param>
        internal JsonDocument(JObject docObject)
        {
            if (docObject == null)
            {
                throw new ArgumentNullException(nameof(docObject));
            }

            this.documentPath = string.Empty;
            this.DocumentObject = (JObject)docObject.DeepClone();
        }

        /// <summary>
        /// Gets the JObject corresponding to the root of the document
        /// </summary>
        internal JObject DocumentObject { get; private set; }

        /// <inheritdoc/>
        public bool Equals(JsonDocument other)
        {
            return JToken.DeepEquals(this.DocumentObject, other.DocumentObject);
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
                this.DocumentObject.WriteTo(writer);
            }
        }
    }
}
