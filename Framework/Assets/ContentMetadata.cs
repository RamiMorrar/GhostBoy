﻿namespace Macabresoft.Macabre2D.Framework {
    using System;
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
        private readonly HashSet<IAsset> _assets = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentMetadata" /> class.
        /// </summary>
        public ContentMetadata() : this(Guid.NewGuid()) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentMetadata" /> class.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        public ContentMetadata(Guid contentId) {
            this.ContentId = contentId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentMetadata" /> class.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="assets">The assets which belong to this content.</param>
        public ContentMetadata(Guid contentId, params IAsset[] assets) : this(contentId) {
            foreach (var asset in assets) {
                this._assets.Add(asset);
            }
        }

        /// <summary>
        /// Gets the assets associated with this content.
        /// </summary>
        public IReadOnlyCollection<IAsset> Assets => this._assets;

        /// <summary>
        /// Gets the content identifier.
        /// </summary>
        [DataMember]
        public Guid ContentId { get; }

        /// <summary>
        /// Adds an asset.
        /// </summary>
        /// <returns>A value indicating whether or not the asset was added.</returns>
        public bool AddAsset(IAsset asset) {
            return this._assets.Add(asset);
        }

        /// <summary>
        /// Removes an asset.
        /// </summary>
        /// <returns>A value indicating whether or not the asset was removed.</returns>
        public bool RemoveAsset(IAsset asset) {
            return this._assets.Remove(asset);
        }
    }
}