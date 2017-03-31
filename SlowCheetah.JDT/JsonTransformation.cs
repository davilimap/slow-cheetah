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
        private JObject transformObject;
        private JsonLoadSettings loadSettings;

        private JsonTransformContext context;

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

            this.context = new JsonTransformContext()
            {
                TransformFile = transformFile,
                Logger = new JsonTransformationLogger(logger),
            };

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

            this.context = new JsonTransformContext()
            {
                TransformFile = null,
                Logger = new JsonTransformationLogger(logger),
            };

            this.SetTransform(transform);
        }

        /// <summary>
        /// Transforms a JSON object
        /// </summary>
        /// <param name="sourceFile">The path to the json file to be transformed</param>
        /// <returns>A stream containing the results of the transforms</returns>
        public Stream Apply(string sourceFile)
        {
            if (string.IsNullOrEmpty(sourceFile))
            {
                throw new ArgumentNullException(nameof(sourceFile));
            }

            this.context.SourceFile = sourceFile;

            // Open the file as streams and apply the transforms
            Stream sourceStream = File.Open(sourceFile, FileMode.Open);
            return this.Apply(sourceStream);
        }

        /// <summary>
        /// Transforms a JSON object
        /// </summary>
        /// <param name="source">The object to be transformed</param>
        /// <returns>A stream containing the results of the transforms. Returns null if the transformation was not completed</returns>
        public Stream Apply(Stream source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (StreamReader sourceStreamReader = new StreamReader(source))
            using (JsonTextReader sourceReader = new JsonTextReader(sourceStreamReader))
            {
                JsonLoadSettings loadSettings = new JsonLoadSettings()
                {
                    CommentHandling = CommentHandling.Ignore,

                    // Obs: LineInfo is handled on Ignore and not Load
                    // See https://github.com/JamesNK/Newtonsoft.Json/issues/1249
                    LineInfoHandling = LineInfoHandling.Ignore
                };

                Stream result = null;
                JObject sourceObject = null;

                try
                {
                    // The JObject corresponding to the streams with line info
                    sourceObject = JObject.Load(sourceReader, loadSettings);

                    // Execute the transforms
                    JdtProcessor.ProcessTransform(sourceObject, this.transformObject, this.context);
                }
                catch (Exception ex)
                {
                    if (!this.context.Logger.LogErrorFromException(ex))
                    {
                        throw;
                    }
                }
                finally
                {
                    if (!this.context.Logger.HasLoggedErrors)
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

                return result;
            }
        }

        private void SetTransform(Stream transformStream)
        {
            this.loadSettings = new JsonLoadSettings()
            {
                CommentHandling = CommentHandling.Ignore,

                // Obs: LineInfo is handled on Ignore and not Load
                // See https://github.com/JamesNK/Newtonsoft.Json/issues/1249
                LineInfoHandling = LineInfoHandling.Ignore
            };

            using (StreamReader transformStreamReader = new StreamReader(transformStream))
            using (JsonTextReader transformReader = new JsonTextReader(transformStreamReader))
            {
                this.transformObject = JObject.Load(transformReader, this.loadSettings);
            }
        }
    }
}
