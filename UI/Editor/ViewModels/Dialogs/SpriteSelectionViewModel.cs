namespace Macabresoft.Macabre2D.UI.Editor;

using System.Collections.Generic;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using ReactiveUI;
using Unity;

/// <summary>
/// A view model for the sprite selection dialog.
/// </summary>
public sealed class SpriteSelectionViewModel : BaseDialogViewModel {
    private readonly ObservableCollectionExtended<SpriteDisplayCollection> _spriteSheets = new();
    private FilteredContentWrapper _selectedContentNode;
    private SpriteDisplayModel _selectedSprite;
    private ThumbnailSize _selectedThumbnailSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSelectionViewModel" /> class.
    /// </summary>
    public SpriteSelectionViewModel() : base() {
        this.IsOkEnabled = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSelectionViewModel" /> class.
    /// </summary>
    /// <param name="contentService">The content service.</param>
    [InjectionConstructor]
    public SpriteSelectionViewModel(IContentService contentService) : this() {
        this.RootContentDirectory = new FilteredContentWrapper(contentService.RootContentDirectory, typeof(SpriteSheet), false);
        this.SelectedContentNode = this.RootContentDirectory;
    }

    /// <summary>
    /// Gets the root content directory as a <see cref="FilteredContentWrapper" />.
    /// </summary>
    public FilteredContentWrapper RootContentDirectory { get; }

    /// <summary>
    /// Gets the selected content node as a <see cref="FilteredContentWrapper" />.
    /// </summary>
    public FilteredContentWrapper SelectedContentNode {
        get => this._selectedContentNode;
        set {
            if (this._selectedContentNode != value) {
                this.RaiseAndSetIfChanged(ref this._selectedContentNode, value);
                this.ResetSpriteSheets();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected sprite.
    /// </summary>
    public SpriteDisplayModel SelectedSprite {
        get => this._selectedSprite;
        set {
            this.RaiseAndSetIfChanged(ref this._selectedSprite, value);
            this.IsOkEnabled = this.SelectedSprite != null;
        }
    }

    /// <summary>
    /// Gets or sets the selected thumbnail size.
    /// </summary>
    public ThumbnailSize SelectedThumbnailSize {
        get => this._selectedThumbnailSize;
        set => this.RaiseAndSetIfChanged(ref this._selectedThumbnailSize, value);
    }

    /// <summary>
    /// Gets the sprite sheets via <see cref="SpriteDisplayCollection" />.
    /// </summary>
    public IReadOnlyCollection<SpriteDisplayCollection> SpriteSheets => this._spriteSheets;

    private void ResetSpriteSheets() {
        this._spriteSheets.Clear();

        if (this.SelectedContentNode != null) {
            if (this.SelectedContentNode.Node is ContentDirectory directory) {
                var spriteCollections = new List<SpriteDisplayCollection>();
                foreach (var file in directory.GetAllContentFiles()) {
                    if (file.Asset is SpriteSheet spriteSheet) {
                        spriteCollections.Add(new SpriteDisplayCollection(spriteSheet, file));
                    }
                }

                this._spriteSheets.Reset(spriteCollections);
            }
            else if (this.SelectedContentNode.Node is ContentFile { Asset: SpriteSheet spriteSheet } file) {
                var spriteCollection = new SpriteDisplayCollection(spriteSheet, file);
                this._spriteSheets.Add(spriteCollection);
            }
        }
    }
}