namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

/// <summary>
/// Interface to manage assets.
/// </summary>
public interface IAssetManager : IDisposable {
    /// <summary>
    /// Gets the loaded metadata.
    /// </summary>
    IReadOnlyCollection<ContentMetadata> LoadedMetadata { get; }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="contentManager">The content manager.</param>
    /// <param name="serializer">The serializer.</param>
    void Initialize(ContentManager contentManager, ISerializer serializer);

    /// <summary>
    /// Loads content for an ass
    /// </summary>
    /// <param name="asset"></param>
    /// <typeparam name="TContent"></typeparam>
    void LoadContentForAsset<TContent>(IAsset asset) where TContent : class;

    /// <summary>
    /// Registers the content meta data into the cache.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    void RegisterMetadata(ContentMetadata metadata);

    /// <summary>
    /// Resolves an asset reference and loads required content.
    /// </summary>
    /// <param name="assetReference">The asset reference to resolve.</param>
    /// <param name="asset">The asset if found.</param>
    /// <typeparam name="TAsset">The type of asset.</typeparam>
    /// <typeparam name="TContent">The type of content.</typeparam>
    /// <returns>A value indicating whether or not the asset was resolved.</returns>
    bool TryGetAsset<TAsset, TContent>(AssetReference<TAsset, TContent> assetReference, out TAsset? asset)
        where TAsset : class, IAsset, IAsset<TContent> where TContent : class;

    /// <summary>
    /// Tries sto get metadata.
    /// </summary>
    /// <param name="contentId">The content identifier.</param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    bool TryGetMetadata(Guid contentId, out ContentMetadata? metadata);

    /// <summary>
    /// Loads the asset at the specified path.
    /// </summary>
    /// <typeparam name="T">The type of asset to load.</typeparam>
    /// <param name="contentPath">The path.</param>
    /// <param name="loaded">The loaded content.</param>
    /// <returns>The asset.</returns>
    bool TryLoadContent<T>(string contentPath, out T? loaded) where T : class;

    /// <summary>
    /// Loads the asset with the specified identifier.
    /// </summary>
    /// <typeparam name="T">The type of asset to load.</typeparam>
    /// <param name="id">The identifier.</param>
    /// <param name="loaded">The loaded content.</param>
    /// <returns>The asset.</returns>
    bool TryLoadContent<T>(Guid id, out T? loaded) where T : class;

    /// <summary>
    /// Unloads the content manager.
    /// </summary>
    void Unload();
}

/// <summary>
/// Maps content with identifiers. This should be the primary way that content is accessed.
/// </summary>
public sealed class AssetManager : IAssetManager {
    private const string MonoGameNamespace = nameof(Microsoft) + "." + nameof(Microsoft.Xna) + "." + nameof(Microsoft.Xna.Framework);

    /// <summary>
    /// The default empty <see cref="IAssetManager" /> that is present before initialization.
    /// </summary>
    public static readonly IAssetManager Empty = new EmptyAssetManager();

    private readonly HashSet<ContentMetadata> _loadedMetadata = new();
    private ContentManager? _contentManager;
    private ISerializer? _serializer;

    /// <inheritdoc />
    public IReadOnlyCollection<ContentMetadata> LoadedMetadata => this._loadedMetadata;

    /// <inheritdoc />
    public void Dispose() {
        this.Unload();
        this._loadedMetadata.Clear();
        this._serializer = null;
        this._contentManager?.Dispose();
        this._contentManager = null;
    }

    /// <inheritdoc />
    public void Initialize(ContentManager contentManager, ISerializer serializer) {
        this._contentManager = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
        this._serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    /// <inheritdoc />
    public void LoadContentForAsset<TContent>(IAsset asset) where TContent : class {
        if (asset is IAsset<TContent> { Content: null } contentAsset &&
            this.TryLoadContent<TContent>(asset.ContentId, out var content) &&
            content != null) {
            contentAsset.LoadContent(content);
        }
    }

    /// <inheritdoc />
    public void RegisterMetadata(ContentMetadata metadata) {
        this._loadedMetadata.Add(metadata);
    }

    /// <inheritdoc />
    public bool TryGetAsset<TAsset, TContent>(AssetReference<TAsset, TContent> assetReference, out TAsset? asset)
        where TAsset : class, IAsset, IAsset<TContent>
        where TContent : class {
        asset = assetReference.Asset ?? this.GetAsset<TAsset>(assetReference.ContentId);
        return asset != null;
    }

    /// <inheritdoc />
    public bool TryGetMetadata(Guid contentId, out ContentMetadata? metadata) {
        if (contentId != Guid.Empty) {
            if (this._loadedMetadata.FirstOrDefault(x => x.ContentId == contentId) is { } cachedMetadata) {
                metadata = cachedMetadata;
            }
            else if (this.TryLoadContent(ContentMetadata.GetMetadataPath(contentId), out ContentMetadata? foundMetadata) && foundMetadata != null) {
                metadata = foundMetadata;
                this.RegisterMetadata(foundMetadata);
            }
            else {
                metadata = null;
            }
        }
        else {
            metadata = null;
        }

        return metadata != null;
    }

    /// <inheritdoc />
    public bool TryLoadContent<T>(string contentPath, out T? loaded) where T : class {
        loaded = null;
        if (this._contentManager != null) {
            if (typeof(T).Namespace?.StartsWith(MonoGameNamespace) == true) {
                try {
                    loaded = this._contentManager.Load<T?>(contentPath);
                }
                catch (ContentLoadException) {
                    // TODO: log this
                }
            }
            else if (this._serializer != null) {
                try {
                    var fileStream = TitleContainer.OpenStream(Path.Combine(this._contentManager.RootDirectory, contentPath));
                    loaded = this._serializer.Deserialize<T>(fileStream);
                }
                catch (FileNotFoundException) {
                    // TODO: log this
                }
                catch (JsonSerializationException) {
                    // TODO: log this
                }
            }
        }

        return loaded != null;
    }

    /// <inheritdoc />
    public bool TryLoadContent<T>(Guid id, out T? loaded) where T : class {
        if (this.TryGetContentPath(id, out var path) && !string.IsNullOrWhiteSpace(path)) {
            this.TryLoadContent(path, out loaded);
        }
        else {
            loaded = null;
        }

        return loaded != null;
    }


    /// <inheritdoc />
    public void Unload() {
        this._contentManager?.Unload();
        this._loadedMetadata.Clear();
    }

    private TAsset? GetAsset<TAsset>(Guid contentId) where TAsset : class, IAsset {
        TAsset? result = null;
        if (contentId != Guid.Empty) {
            if (this._loadedMetadata.FirstOrDefault(x => x.ContentId == contentId)?.Asset is TAsset asset) {
                result = asset;
            }
            else if (this.TryGetMetadata(contentId, out var metadata) && metadata != null) {
                result = metadata.Asset as TAsset;
            }
        }

        return result;
    }

    private bool TryGetContentPath(Guid contentId, out string? contentPath) {
        if (this.TryGetMetadata(contentId, out var metadata) && metadata != null) {
            contentPath = metadata.GetContentPath();
        }
        else {
            contentPath = null;
        }

        return !string.IsNullOrEmpty(contentPath);
    }

    private sealed class EmptyAssetManager : IAssetManager {
        public IReadOnlyCollection<ContentMetadata> LoadedMetadata => Array.Empty<ContentMetadata>();

        public void Dispose() {
        }

        public void Initialize(ContentManager contentManager, ISerializer serializer) {
        }

        public void LoadContentForAsset<TContent>(IAsset asset) where TContent : class {
        }

        public void RegisterMetadata(ContentMetadata metadata) {
        }

        public bool TryGetAsset<TAsset, TContent>(AssetReference<TAsset, TContent> assetReference, out TAsset? asset)
            where TAsset : class, IAsset, IAsset<TContent> where TContent : class {
            asset = null;
            return false;
        }

        public bool TryGetMetadata(Guid contentId, out ContentMetadata? metadata) {
            metadata = null;
            return false;
        }

        public bool TryLoadContent<T>(string contentPath, out T? loaded) where T : class {
            loaded = null;
            return false;
        }

        public bool TryLoadContent<T>(Guid id, out T? loaded) where T : class {
            loaded = null;
            return false;
        }

        public void Unload() {
        }
    }
}