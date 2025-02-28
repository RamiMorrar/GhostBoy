﻿namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Threading;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

/// <summary>
/// A view model for editing auto tile sets.
/// </summary>
public class AutoTileSetEditorViewModel : BaseViewModel {
    private const double MaxTileSize = 128;

    private static readonly byte[] TileIndexToAutoSpriteIndex = {
        15, 11, 14, 10,
        12, 8, 13, 9,
        3, 7, 2, 6,
        0, 4, 1, 5
    };

    private readonly IUndoService _undoService;
    private SpriteDisplayModel _selectedSprite;
    private ThumbnailSize _selectedThumbnailSize;
    private AutoTileIndexModel _selectedTile;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoTileSetEditorViewModel" /> class.
    /// </summary>
    public AutoTileSetEditorViewModel() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoTileSetEditorViewModel" /> class.
    /// </summary>
    /// <param name="undoService">The parent undo service.</param>
    /// <param name="tileSet">The tile set being edited.</param>
    /// <param name="spriteSheet">The sprite sheet.</param>
    /// <param name="file">The content file.</param>
    [InjectionConstructor]
    public AutoTileSetEditorViewModel(
        IUndoService undoService,
        AutoTileSet tileSet,
        SpriteSheet spriteSheet,
        ContentFile file) : base() {
        this._undoService = undoService;
        this.ClearSpriteCommand = ReactiveCommand.Create(
            this.ClearSprite,
            this.WhenAny(x => x.SelectedTile, x => x.Value != null));
        this.SelectTileCommand = ReactiveCommand.Create<AutoTileIndexModel>(this.SelectTile);
        this.SpriteCollection = new SpriteDisplayCollection(spriteSheet, file);

        var tiles = new AutoTileIndexModel[tileSet.Size];
        for (byte i = 0; i < tiles.Length; i++) {
            tiles[i] = new AutoTileIndexModel(tileSet, i);
        }

        this.Tiles = tiles;
        this.SelectedTile = this.Tiles.First();
        this.TileSize = this.GetTileSize();

        this.CanPerformAutoLayout = spriteSheet.Rows * spriteSheet.Columns == tileSet.Size;
        this.AutoLayoutCommand = ReactiveCommand.Create(this.PerformAutoLayout, this.WhenAnyValue(x => x.CanPerformAutoLayout));
    }

    /// <summary>
    /// Gets the auto layout command.
    /// </summary>
    public ICommand AutoLayoutCommand { get; }

    /// <summary>
    /// Clears the selected sprite from the selected tile.
    /// </summary>
    public ICommand ClearSpriteCommand { get; }

    /// <summary>
    /// Gets or sets the selected sprite.
    /// </summary>
    public SpriteDisplayModel SelectedSprite {
        get => this._selectedSprite;
        set {
            if (this._selectedTile is AutoTileIndexModel selectedTile) {
                var previousSprite = this._selectedSprite;
                this._undoService.Do(() =>
                {
                    this.RaiseAndSetIfChanged(ref this._selectedSprite, value);
                    selectedTile.SpriteIndex = this._selectedSprite?.Index;
                }, () =>
                {
                    this.RaiseAndSetIfChanged(ref this._selectedSprite, previousSprite);
                    selectedTile.SpriteIndex = this._selectedSprite?.Index;
                });
            }
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
    /// Gets or sets the selected tile.
    /// </summary>
    public AutoTileIndexModel SelectedTile {
        get => this._selectedTile;
        private set {
            if (this._selectedTile != value) {
                this.RaiseAndSetIfChanged(ref this._selectedTile, value);
                this._selectedSprite = this._selectedTile != null ? this.SpriteCollection.FirstOrDefault(x => x.Index == this._selectedTile.SpriteIndex) : null;
                this.RaisePropertyChanged(nameof(this.SelectedSprite));
            }
        }
    }

    /// <summary>
    /// Gets the select tile command.
    /// </summary>
    public ICommand SelectTileCommand { get; }

    /// <summary>
    /// Gets the sprite collection.
    /// </summary>
    public SpriteDisplayCollection SpriteCollection { get; }

    /// <summary>
    /// Gets the tiles.
    /// </summary>
    public IReadOnlyCollection<AutoTileIndexModel> Tiles { get; }

    /// <summary>
    /// Gets the size of a tile in pixels.
    /// </summary>
    public Size TileSize { get; }

    /// <summary>
    /// Gets a value indicating whether or not auto layout can be performed for this tile set.
    /// </summary>
    private bool CanPerformAutoLayout { get; }

    private void ClearSprite() {
        if (this.SelectedTile is { SpriteIndex: { } }) {
            this.SelectedSprite = null;
        }
    }

    private Size GetTileSize() {
        var size = Size.Empty;
        if (this.SpriteCollection.Sprites.FirstOrDefault() is SpriteDisplayModel { Size: { Width: > 0, Height: > 0 } spriteSize }) {
            if (spriteSize.Width == spriteSize.Height) {
                size = new Size(MaxTileSize, MaxTileSize);
            }
            else if (spriteSize.Width > spriteSize.Height) {
                var ratio = spriteSize.Height / (double)spriteSize.Width;
                size = new Size(MaxTileSize, MaxTileSize * ratio);
            }
            else {
                var ratio = spriteSize.Width / (double)spriteSize.Height;
                size = new Size(MaxTileSize * ratio, MaxTileSize);
            }
        }

        return size;
    }

    private void PerformAutoLayout() {
        if (this.CanPerformAutoLayout) {
            var tiles = this.Tiles.ToList();
            var spriteIndexes = tiles.Select(x => x.SpriteIndex).ToList();

            this._undoService.Do(() =>
            {
                foreach (var tile in tiles) {
                    tile.SpriteIndex = TileIndexToAutoSpriteIndex[tile.TileIndex];
                }
            }, () =>
            {
                for (var i = 0; i < tiles.Count; i++) {
                    tiles[i].SpriteIndex = spriteIndexes[i];
                }
            });
        }
    }

    private void SelectTile(AutoTileIndexModel tile) {
        if (tile != null) {
            this.SelectedTile = tile;
            // HACK: this makes things work well in the UI, just trust me.
            Dispatcher.UIThread.Post(() => this.RaisePropertyChanged(nameof(this.SelectedTile)), DispatcherPriority.ApplicationIdle);
        }
    }
}