﻿namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Serializable metadata for content loaded by MonoGame. Contains the assets built on top of content.
    /// </summary>
    [DataContract]
    public class ContentMetadata {
        /// <summary>
        /// The file extension for metadata.
        /// </summary>
        public const string FileExtension = ".meta";

        [DataMember]
        private readonly List<IAsset> _assets = new();

        /// <summary>
        /// Gets the assets associated with this content.
        /// </summary>
        public IReadOnlyCollection<IAsset> Assets => this._assets;
    }
}