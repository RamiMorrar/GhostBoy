namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// An abstract base entity that renders a single sprite, given a sprite sheet and a sprite index.
/// </summary>
[Category(CommonCategories.Rendering)]
public abstract class BaseSpriteEntity : RenderableEntity, IRotatable {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly ResettableLazy<Transform> _pixelTransform;
    private readonly ResettableLazy<Transform> _rotatableTransform;
    private Color _color = Color.White;
    private Rotation _rotation;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSpriteEntity" /> class.
    /// </summary>
    protected BaseSpriteEntity() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
        this._pixelTransform = new ResettableLazy<Transform>(this.CreatePixelTransform);
        this._rotatableTransform = new ResettableLazy<Transform>(this.CreateRotatableTransform);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;

    /// <summary>
    /// Gets the sprite index.
    /// </summary>
    public abstract byte? SpriteIndex { get; }

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>The color.</value>
    [DataMember(Order = 1)]
    public Color Color {
        get => this._color;
        set => this.Set(ref this._color, value);
    }

    /// <summary>
    /// Gets or sets the render settings.
    /// </summary>
    /// <value>The render settings.</value>
    [DataMember(Order = 4, Name = "Render Settings")]
    public RenderSettings RenderSettings { get; private set; } = new();

    /// <inheritdoc />
    [DataMember(Order = 3)]
    [Category(CommonCategories.Transform)]
    public Rotation Rotation {
        get => this.ShouldSnapToPixels(this.Settings) ? Rotation.Zero : this._rotation;
        set {
            if (this.Set(ref this._rotation, value) && !this.ShouldSnapToPixels(this.Settings)) {
                this._boundingArea.Reset();
                this._rotatableTransform.Reset();
            }
        }
    }

    /// <summary>
    /// Gets the sprite sheet.
    /// </summary>
    protected abstract SpriteSheetAsset? SpriteSheet { get; }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.RenderSettings.PropertyChanged += this.RenderSettings_PropertyChanged;
        this.RenderSettings.Initialize(this.CreateSize);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this.SpriteIndex.HasValue && this.SpriteBatch is { } spriteBatch && this.SpriteSheet is { } spriteSheet) {
            spriteSheet.Draw(
                spriteBatch,
                this.Settings.PixelsPerUnit,
                this.SpriteIndex.Value,
                this.GetRenderTransform(),
                this.Color,
                this.RenderSettings.Orientation);
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(IEntity.Transform)) {
            this.Reset();
        }
        else if (e.PropertyName == nameof(IEntity.IsEnabled) && this.IsEnabled) {
            this.RaisePropertyChanged(nameof(this.IsVisible));
        }
    }

    /// <summary>
    /// Resets the render settings, bounding area, and render transform.
    /// </summary>
    protected void Reset() {
        this.RenderSettings.InvalidateSize();
        this._boundingArea.Reset();
        this._pixelTransform.Reset();
        this._rotatableTransform.Reset();
    }

    private BoundingArea CreateBoundingArea() {
        BoundingArea result;
        if (this.SpriteSheet is { } spriteSheet) {
            var inversePixelsPerUnit = this.Settings.InversePixelsPerUnit;
            var width = spriteSheet.SpriteSize.X * inversePixelsPerUnit;
            var height = spriteSheet.SpriteSize.Y * inversePixelsPerUnit;
            var offset = this.RenderSettings.Offset * inversePixelsPerUnit;

            var points = new List<Vector2> {
                this.GetWorldTransform(offset, this.Rotation).Position,
                this.GetWorldTransform(offset + new Vector2(width, 0f), this.Rotation).Position,
                this.GetWorldTransform(offset + new Vector2(width, height), this.Rotation).Position,
                this.GetWorldTransform(offset + new Vector2(0f, height), this.Rotation).Position
            };

            var minimumX = points.Min(x => x.X);
            var minimumY = points.Min(x => x.Y);
            var maximumX = points.Max(x => x.X);
            var maximumY = points.Max(x => x.Y);

            if (this.ShouldSnapToPixels(this.Settings)) {
                minimumX = minimumX.ToPixelSnappedValue(this.Settings);
                minimumY = minimumY.ToPixelSnappedValue(this.Settings);
                maximumX = maximumX.ToPixelSnappedValue(this.Settings);
                maximumY = maximumY.ToPixelSnappedValue(this.Settings);
            }

            result = new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
        }
        else {
            result = new BoundingArea();
        }

        return result;
    }

    private Transform CreatePixelTransform() {
        var worldTransform =
            this.GetWorldTransform(this.RenderSettings.Offset * this.Settings.InversePixelsPerUnit)
                .ToPixelSnappedValue(this.Settings);
        return worldTransform;
    }

    private Transform CreateRotatableTransform() {
        return this.GetWorldTransform(this.RenderSettings.Offset * this.Settings.InversePixelsPerUnit, this.Rotation);
    }

    private Vector2 CreateSize() {
        var result = Vector2.Zero;
        if (this.SpriteSheet is { } spriteSheet) {
            return new Vector2(spriteSheet.SpriteSize.X, spriteSheet.SpriteSize.Y);
        }

        return result;
    }

    private Transform GetRenderTransform() {
        return this.ShouldSnapToPixels(this.Settings) ? this._pixelTransform.Value : this._rotatableTransform.Value;
    }

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderSettings.Offset)) {
            this._pixelTransform.Reset();
            this._rotatableTransform.Reset();
            this._boundingArea.Reset();
        }
    }
}