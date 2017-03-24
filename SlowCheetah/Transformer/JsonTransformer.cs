// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SlowCheetah.JDT;

    /// <summary>
    /// Transforms JSON files utilizing JSON Document Transformations
    /// </summary>
    public class JsonTransformer : ITransformer
    {
        /// <inheritdoc/>
        public bool Transform(string source, string transform, string destination)
        {
            // Parameter validation
            Contract.Requires(!string.IsNullOrWhiteSpace(source));
            Contract.Requires(!string.IsNullOrWhiteSpace(transform));
            Contract.Requires(!string.IsNullOrWhiteSpace(destination));

            // File validation
            if (!File.Exists(source))
            {
                throw new FileNotFoundException("File to transform not found", source);
            }

            if (!File.Exists(transform))
            {
                throw new FileNotFoundException("Transform file not found", transform);
            }

            JsonDocument document = new JsonDocument(source);

            JsonTransformation transformation = new JsonTransformation(transform);

            transformation.Apply(document);

            document.Save(destination);

            return true;
        }
    }
}
