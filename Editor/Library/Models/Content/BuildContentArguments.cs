﻿namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    
    /// <summary>
    /// Arguments for building content using MGCB.
    /// </summary>
    public class BuildContentArguments {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildContentArguments" /> struct.
        /// </summary>
        /// <param name="contentFilePath">The content file path.</param>
        /// <param name="projectDirectoryPath">The project directory path.</param>
        /// <param name="platform">The platform.</param>
        /// <param name="performCompression">if set to <c>true</c> MGCB will perform compression.</param>
        public BuildContentArguments(
            string contentFilePath,
            string projectDirectoryPath,
            string platform,
            bool performCompression) {
            this.ContentFilePath = contentFilePath;
            this.ProjectDirectoryPath = projectDirectoryPath;
            this.Platform = platform;
            this.PerformCompression = performCompression;
        }

        /// <summary>
        /// Gets the content file path.
        /// </summary>
        /// <value>The content file path.</value>
        public string ContentFilePath { get; }

        /// <summary>
        /// Gets a value indicating whether or not MGCB should perform compression.
        /// </summary>
        /// <value><c>true</c> if MGCB should perform compression; otherwise, <c>false</c>.</value>
        public bool PerformCompression { get; }

        /// <summary>
        /// Gets the platform.
        /// </summary>
        /// <value>The platform.</value>
        public string Platform { get; }
        
        /// <summary>
        /// Gets the project directory path.
        /// </summary>
        /// <value>The project directory path.</value>
        public string ProjectDirectoryPath { get; }

        /// <summary>
        /// Converts to console arguments used by MGCB.
        /// </summary>
        /// <returns>The console arguments.</returns>
        public string ToConsoleArguments() {
            var arguments = this.GetConsoleArguments();
            return arguments.Aggregate((first, second) => $"{first} {second}");
        }

        /// <summary>
        /// Gets the console arguments as an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <returns>The console arguments.</returns>
        public IEnumerable<string> GetConsoleArguments() {
            var arguments = this.GetMGCBFileArguments(this.ProjectDirectoryPath);
            arguments.Add("/rebuild");
            arguments.Add($"/@:\"{this.ContentFilePath}\"");
            return arguments;
        }

        /// <summary>
        /// Gets the MGCB arguments as an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <returns>The MGCB arguments.</returns>
        public IEnumerable<string> GetMGCBFileArguments() {
            return this.GetMGCBFileArguments(this.ProjectDirectoryPath);
        }

        private IList<string> GetMGCBFileArguments(string projectDirectoryPath) {
            return new List<string>() {
                $"/outputDir:\"{Path.Combine(projectDirectoryPath, "bin", this.Platform)}\"",
                $"/intermediateDir:\"{Path.Combine(projectDirectoryPath, "obj", this.Platform)}\"",
                $"/platform:{this.Platform}",
                "/config:",
                "/profile:Reach",
                $"/compress:{this.PerformCompression}"
            };
        }
    }
}