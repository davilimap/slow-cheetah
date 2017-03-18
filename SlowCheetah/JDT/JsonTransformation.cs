// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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
            this.TransformCore(this.transform, document.GetObject());
        }

        private void TransformCore(JObject transformNode, JObject originalNode)
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
                                this.TransformCore((JObject)child.Value, (JObject)originalChild);
                            }
                            else
                            {
                                // If it exists and is not an object, it is a replace transformation, so queue it
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
                        // Any other types consitutues a non-recursive merge
                        nodesToTransform.Enqueue(child.Name);
                        break;
                }
            }

            while (nodesToTransform.Count() > 0)
            {
                // Execute the transforms in queue order
                string nodeName = nodesToTransform.Dequeue();
                JToken nodeToTransform;
                if (originalNode.TryGetValue(nodeName, out nodeToTransform))
                {
                    if (nodeToTransform.Type == JTokenType.Array && transformNode[nodeName].Type == JTokenType.Array)
                    {
                        // If the original and transform are arrays, merge the contents together
                        this.MergeArray((JArray)nodeToTransform, (JArray)transformNode[nodeName]);
                    }
                    else
                    {
                        // If the contents are different, execute the replace
                        originalNode[nodeName] = transformNode[nodeName].DeepClone();
                    }
                }
                else
                {
                    // If the node is not present in the original, add it
                    originalNode.Add(nodeName, transformNode[nodeName].DeepClone());
                }
            }
        }

        private void MergeArray(JArray original, JArray arrayToMerge)
        {
            foreach (JToken token in arrayToMerge)
            {
                original.Add(token.DeepClone());
            }
        }
    }
}
