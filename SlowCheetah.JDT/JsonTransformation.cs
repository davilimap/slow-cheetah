// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
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
        // TO DO: Constructor that takes in a logger

        /// <summary>
        /// Transforms a JSON object
        /// </summary>
        /// <param name="sourceFile">The full path to the json file to be transformed</param>
        /// <param name="transformFile">The full path to the json file containing the transformation</param>
        /// <returns>A stream containing the results of the transforms</returns>
        public Stream Apply(string sourceFile, string transformFile)
        {
            // Open the file as streams and apply the transforms
            // TO DO: Save the source and transform file to the context for logging
            Stream sourceStream = File.Open(sourceFile, FileMode.Open);
            Stream transformStream = File.Open(transformFile, FileMode.Open);
            return this.Apply(sourceStream, transformStream);
        }

        /// <summary>
        /// Transforms a JSON object
        /// </summary>
        /// <param name="source">The object to be transformed</param>
        /// <param name="transform">The object specifying the transforms</param>
        /// <returns>A stream containing the results of the transforms</returns>
        public Stream Apply(Stream source, Stream transform)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            using (StreamReader sourceStreamReader = new StreamReader(source))
            using (StreamReader transformStreamReader = new StreamReader(transform))
            using (JsonTextReader sourceReader = new JsonTextReader(sourceStreamReader))
            using (JsonTextReader transformReader = new JsonTextReader(transformStreamReader))
            {
                JsonLoadSettings loadSettings = new JsonLoadSettings()
                {
                    CommentHandling = CommentHandling.Ignore,

                    // Obs: LineInfo is handled on Ignore and not Load
                    // See https://github.com/JamesNK/Newtonsoft.Json/issues/1249
                    LineInfoHandling = LineInfoHandling.Ignore
                };

                // The JObject corresponding to the streams with line info
                var sourceObject = JObject.Load(sourceReader, loadSettings);
                var transformObject = JObject.Load(transformReader, loadSettings);

                // Execute the transforms
                JdtProcessor.ProcessTransform(sourceObject, transformObject);

                // Save the result to a memory stream
                // Don't close the stream of the streamwriter so data isn't lost
                // User should handle the close
                Stream result = new MemoryStream();
                StreamWriter streamWriter = new StreamWriter(result);
                JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter);

                // Writes the changes in the source object to the stream
                // and resets it so the user can read the stream
                sourceObject.WriteTo(jsonWriter);
                streamWriter.Flush();
                result.Position = 0;

                return result;
            }
        }
    }
}
