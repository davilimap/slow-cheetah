namespace SlowCheetah.JDT
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
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
        /// <param name="transform">Object that corresponds to the transformation file</param>
        public JsonTransformation(JObject transform)
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            this.transform = transform;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTransformation"/> class.
        /// </summary>
        /// <param name="transformFile">File that defines the trasnformation</param>
        public JsonTransformation(string transformFile)
        {
            if (string.IsNullOrEmpty(transformFile))
            {
                throw new ArgumentNullException(nameof(transformFile));
            }

            using (StreamReader file = File.OpenText(transformFile))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JsonLoadSettings loadSettings = new JsonLoadSettings()
                {
                    CommentHandling = CommentHandling.Ignore,
                    LineInfoHandling = LineInfoHandling.Ignore
                };

                JObject trn = (JObject)JToken.ReadFrom(reader, loadSettings);
                this.transform = trn;
            }
        }

        /// <summary>
        /// Apply the specified transformation
        /// </summary>
        /// <param name="document">Document to be transformed</param>
        public void Apply(JObject document)
        {
            this.TransformLoop(this.transform, document);
        }

        private void TransformLoop(JObject transformNode, JObject originalNode)
        {
            Queue<string> nodesToTransform = new Queue<string>();

            foreach (JProperty child in transformNode.Properties())
            {
                switch (child.Value.Type)
                {
                    case JTokenType.Object:
                        // If the child is an object, verifies if it exists in the original file
                        JToken originalChild;
                        if (originalNode.TryGetValue(child.Name, out originalChild))
                        {
                            if (originalChild.Type == JTokenType.Object)
                            {
                                // If it exists and is and is also an object, the recursive transform must be executed
                                this.TransformLoop((JObject)child.Value, (JObject)originalChild);
                            }
                            else
                            {
                                // If it exists and is not an object, it is a replace trasnformation, so queue it
                                nodesToTransform.Enqueue(child.Name);
                            }
                        }
                        else
                        {
                            // If it doesn't exist, then queue the add transform
                            nodesToTransform.Enqueue(child.Name);
                        }

                        break;
                    default:
                        // Any other types consitutues a non-recursive transform
                        nodesToTransform.Enqueue(child.Name);
                        break;
                }
            }
        }
    }
}
