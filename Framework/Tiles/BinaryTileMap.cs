namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// A tile map that is either "on" or "off". The on tiles will show the selected sprite.
/// </summary>
[Display(Name = "Binary Tile Map")]
[Category(CommonCategories.TileMap)]
public sealed class BinaryTileMap : RenderableTileMap {
    [DataMember]
    private readonly HashSet<Point> _activeTiles = new();

    private Vector2 _tileScale;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryTileMap" /> class.
    /// </summary>
    public BinaryTileMap() : base() {
        this.SpriteReference.PropertyChanged += this.SpriteReference_PropertyChanged;
    }

    /// <inheritdoc />
    public override IReadOnlyCollection<Point> ActiveTiles => this._activeTiles;

    /// <summary>
    /// Gets the sprite reference.
    /// </summary>
    [DataMember(Order = 0)]
    [Display(Name = "Sprite")]
    public SpriteReference SpriteReference { get; } = new();

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>The color.</value>
    [DataMember(Order = 1)]
    public Color Color { get; set; } = Color.White;

    /// <inheritdoc />
    public override bool HasActiveTileAt(Point tilePosition) {
        return this._activeTiles.Contains(tilePosition);
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.SpriteReference.Initialize(this.Scene.Assets);
        this.ResetSpriteScale();
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this.SpriteBatch is { } spriteBatch && this.SpriteReference.Asset is { } spriteSheet && this._activeTiles.Any()) {
            var tileBoundingAreas = this._activeTiles
                .Select(this.GetTileBoundingArea)
                .Where(boundingArea => boundingArea.Overlaps(viewBoundingArea));

            if (!this.Scene.BoundingArea.IsEmpty) {
                tileBoundingAreas = tileBoundingAreas.Where(boundingArea => boundingArea.OverlapsExclusive(this.Scene.BoundingArea));
            }

            foreach (var boundingArea in tileBoundingAreas) {
                spriteBatch.Draw(
                    this.Settings.PixelsPerUnit,
                    spriteSheet,
                    this.SpriteReference.SpriteIndex,
                    boundingArea.Minimum,
                    this._tileScale,
                    this.Color);
            }
        }
    }

    /// <inheritdoc />
    protected override void ClearActiveTiles() {
        this._activeTiles.Clear();
    }

    /// <inheritdoc />
    protected override Point GetMaximumTile() {
        return new Point(this._activeTiles.Select(t => t.X).Max(), this._activeTiles.Select(t => t.Y).Max());
    }

    /// <inheritdoc />
    protected override Point GetMinimumTile() {
        return new Point(this._activeTiles.Select(t => t.X).Min(), this._activeTiles.Select(t => t.Y).Min());
    }

    /// <inheritdoc />
    protected override bool HasActiveTiles() {
        return this._activeTiles.Any();
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();

        if (this.SpriteReference.Asset is { } spriteSheet) {
            this._tileScale = this.GetTileScale(spriteSheet.SpriteSize);
        }
    }

    /// <inheritdoc />
    protected override void ResetBoundingArea() {
        base.ResetBoundingArea();
        this.ResetSpriteScale();
    }

    /// <inheritdoc />
    protected override bool TryAddTile(Point tile) {
        return this._activeTiles.Add(tile);
    }

    /// <inheritdoc />
    protected override bool TryRemoveTile(Point tile) {
        return this._activeTiles.Remove(tile);
    }

    private void ResetSpriteScale() {
        if (this.SpriteReference.Asset is { } spriteSheet) {
            this._tileScale = this.GetTileScale(spriteSheet.SpriteSize);
        }
    }

    private void SpriteReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(SpriteSheet.SpriteSize)) {
            this.ResetBoundingArea();
        }
    }
}