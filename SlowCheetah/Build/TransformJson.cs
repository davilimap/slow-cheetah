// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Build.Framework;

    /// <summary>
    /// Task that performs the transformation of the JSON file
    /// </summary>
    public class TransformJson : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// Gets or sets the source file path for the transformation
        /// </summary>
        [Required]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the transformation file path
        /// </summary>
        [Required]
        public string Transform { get; set; }

        /// <summary>
        /// Gets or sets the destination path for the transformation
        /// </summary>
        [Required]
        public string Destination { get; set; }

        /// <inheritdoc/>
        public override bool Execute()
        {
            ITransformer transformer = new JsonTransformer();

            this.Log.LogMessage("Beginning transformation.");

            bool success = transformer.Transform(this.Source, this.Transform, this.Destination);

            success = success && !this.Log.HasLoggedErrors;

            this.Log.LogMessage(success ?
                    "Transformation succeeded." :
                    "Transformation failed.");

            return success;
        }
    }
}
