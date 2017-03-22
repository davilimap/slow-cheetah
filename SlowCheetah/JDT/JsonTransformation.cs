// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.JDT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            TransformCore(this.transform, document.GetObject());
        }

        /// <summary>
        /// Core transformation loop for JDT
        /// </summary>
        /// <param name="transformNode">The node that specifies transformation for the original Node</param>
        /// <param name="originalNode">The node to be transformed</param>
        private static void TransformCore(JObject transformNode, JObject originalNode)
        {
            Queue<string> nodesToTransform = new Queue<string>();
            Dictionary<JdtVerbs, List<string>> verbsToExecute = new Dictionary<JdtVerbs, List<string>>();

            foreach (JProperty child in transformNode.Properties())
            {
                if (JsonUtilities.IsJdtSyntax(child.Name))
                {
                    // If the reserved syntax is used, try to perform the corresponding transformation
                    JdtVerbs verb = JsonUtilities.GetJdtVerb(child.Name);
                    if (verb == JdtVerbs.Invalid)
                    {
                        // TO DO: Inform error with line number and file
                        throw new JdtException(child.Name + " is not a valid JDT verb");
                    }

                    // TO DO: Warning if duplicate transforms are found
                    if (!verbsToExecute.ContainsKey(verb))
                    {
                        verbsToExecute.Add(verb, new List<string>());
                    }

                    // Adds the node to a list of transforms of that verb
                    verbsToExecute[verb].Add(child.Name);
                }
                else
                {
                    // If it is not a transformation verb
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
                                    TransformCore((JObject)child.Value, (JObject)originalChild);
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
            }

            // First, execute explicit transforms
            ExecuteVerbTransforms(verbsToExecute, originalNode, transformNode);

            if (originalNode.Parent == null)
            {
                // If the node that was being transformed was removed,
                // don't execute any more transformations
                return;
            }

            // Then, execute the default trasnforms
            ExecuteDefaultTransforms(nodesToTransform, originalNode, transformNode);
        }

        /// <summary>
        /// Executes the default transformation based on the given nodes
        /// </summary>
        /// <param name="nodesToTransform">Nodes that should be merged from the transform object to the original object</param>
        /// <param name="original">Object being trasnformed</param>
        /// <param name="transform">Object specifying the transformations</param>
        private static void ExecuteDefaultTransforms(Queue<string> nodesToTransform, JObject original, JObject transform)
        {
            while (nodesToTransform.Count() > 0)
            {
                // Execute the transforms in queue order
                string nodeName = nodesToTransform.Dequeue();
                JToken nodeToTransform;
                if (original.TryGetValue(nodeName, out nodeToTransform))
                {
                    if (nodeToTransform.Type == JTokenType.Array && transform[nodeName].Type == JTokenType.Array)
                    {
                        // If the original and transform are arrays, merge the contents together
                        MergeArray((JArray)nodeToTransform, (JArray)transform[nodeName]);
                    }
                    else
                    {
                        // If the contents are different, execute the replace
                        original[nodeName] = transform[nodeName].DeepClone();
                    }
                }
                else
                {
                    // If the node is not present in the original, add it
                    original.Add(nodeName, transform[nodeName].DeepClone());
                }
            }
        }

        /// <summary>
        /// Execute explicit trasformations based on JDT verbs
        /// </summary>
        /// <param name="nodesToTransform">Names of the nodes that correspond to each transformation</param>
        /// <param name="original">Node being transformed</param>
        /// <param name="transform">Node specifying the transformations</param>
        private static void ExecuteVerbTransforms(Dictionary<JdtVerbs, List<string>> nodesToTransform, JObject original, JObject transform)
        {
            List<string> nodes;

            // Perform all removes that were registered
            if (nodesToTransform.TryGetValue(JdtVerbs.Remove, out nodes))
            {
                foreach (string nodeName in nodes)
                {
                    RemoveCore(transform[nodeName], original, allowArray: true);

                    if (original.Parent == null)
                    {
                        // If the entire node has been removed, do not perform other transformations on this node
                        return;
                    }
                }
            }

            // TO DO: Other transformations
        }

        /// <summary>
        /// Core logic for the removal transformation
        /// Verifies the transformation value and transforms accordingly
        /// </summary>
        /// <param name="removeValue">Value of the removal transformation</param>
        /// <param name="original">Node being transformed</param>
        /// <param name="allowArray">True if the transformation value can be an array</param>
        private static void RemoveCore(JToken removeValue, JObject original, bool allowArray)
        {
            switch (removeValue.Type)
            {
                case JTokenType.String:
                    // TO DO: warning if unable to remove
                    // If the value is just a string, remove that node
                    original.Remove(removeValue.ToObject<string>());
                    break;
                case JTokenType.Boolean:
                    if (removeValue.ToObject<bool>())
                    {
                        // If the transform value is true, remove the entire node
                        RemoveNode(original);
                    }

                    break;
                case JTokenType.Object:
                    // If the value is an object, verify the attributes within and perform the remove
                    RemoveWithAttributes((JObject)removeValue, original);
                    break;
                case JTokenType.Array:
                    if (allowArray)
                    {
                        // If the value is an array, perform the remove for each object in the array
                        // Do not allow array values from here
                        foreach (JToken token in (JArray)removeValue)
                        {
                            RemoveCore(token, original, false);
                        }
                    }
                    else
                    {
                        throw new JdtException(removeValue.Type.ToString() + " is not a valid transform value");
                    }

                    break;
                default:
                    throw new JdtException(removeValue.Type.ToString() + " is not a valid transform value");
            }
        }

        /// <summary>
        /// Performs the Remove transform with JDT attributes
        /// Verifies if the properties are valid JDT attributes
        /// </summary>
        /// <param name="removeObject">Transformation object containg the attributes</param>
        /// <param name="original">Node being transformed</param>
        private static void RemoveWithAttributes(JObject removeObject, JObject original)
        {
            // The @JDT.Path attribute
            string path = null;

            // The @JDT.Value attribute
            JToken value = null;

            foreach (JProperty attribute in removeObject.Properties())
            {
                if (!JsonUtilities.IsJdtSyntax(attribute.Name))
                {
                    throw new JdtException(attribute.Name + " is not a valid JDT attribute");
                }

                // If the property is valid JDT syntax,
                // see if it corresponds to a proper value
                switch (JsonUtilities.GetJdtProperty(attribute.Name))
                {
                    case JdtProperties.Path:
                        if (attribute.Type != JTokenType.String)
                        {
                            throw new JdtException("Path attribute must be a string");
                        }

                        if (path != null)
                        {
                            // Alternative: Register a warning and overwrite the value
                            throw new JdtException("Path attribute is defined more than once");
                        }

                        path = attribute.Value.ToObject<string>();
                        break;
                    case JdtProperties.Value:
                        if (value != null)
                        {
                            // Alternative: Register a warning and overwrite the value
                            throw new JdtException("Value attribute is defined more than once");
                        }

                        value = attribute.Value;
                        break;
                    case JdtProperties.Invalid:
                        throw new JdtException(attribute.Name + " is not a valid JDT attribute");
                    default:
                        throw new JdtException(attribute.Name + " is not supported in Remove");
                }
            }

            if (path != null)
            {
                // TO DO: Log warning if value is not null
                JToken startingNode = original.Root;
                if (path.StartsWith("@"))
                {
                    path = "$" + path.Substring(1);
                    startingNode = original;
                }

                IEnumerable<JToken> tokensToRemove = startingNode.SelectTokens(path, true);

                foreach (JToken token in tokensToRemove)
                {
                    // TO DO: Verify if this can cause errors with multiple nodes
                    token.Remove();
                }
            }
            else if (value != null)
            {
                switch (value.Type)
                {
                    case JTokenType.Boolean:
                        if (value.ToObject<bool>())
                        {
                            // If the value is true, remove the entire node
                            RemoveNode(original);
                        }

                        break;
                    case JTokenType.String:
                        original.Remove(value.ToObject<string>());
                        break;
                    default:
                        throw new JdtException(value.Type.ToString() + " as a path attribute is not allowed for Remove");
                }
            }
            else
            {
                throw new JdtException("No valid attributes for Remove");
            }
        }

        /// <summary>
        /// Removes the node from its parents and replaces it with null value
        /// </summary>
        /// <param name="nodeToRemove">Node to be removed</param>
        private static void RemoveNode(JObject nodeToRemove)
        {
            // If the value is true, everything from the node and set it to null
            if (nodeToRemove.Root.Equals(nodeToRemove))
            {
                throw new JdtException("You cannot remove the root!");
            }

            JProperty parent = (JProperty)nodeToRemove.Parent;
            if (parent == null)
            {
                // TO DO: Log error line
                throw new JdtException("Could not remove");
            }
            else
            {
                parent.Value = null;
            }
        }

        private static void MergeArray(JArray original, JArray arrayToMerge)
        {
            foreach (JToken token in arrayToMerge)
            {
                original.Add(token.DeepClone());
            }
        }
    }
}
