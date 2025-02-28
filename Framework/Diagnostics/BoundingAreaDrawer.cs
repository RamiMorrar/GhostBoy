namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel.DataAnnotations;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Draws bounding areas from colliders for debugging purposes.
/// </summary>
[Display(Name = "Bounding Area Drawer (Diagnostics)")]
public class BoundingAreaDrawer : BaseDrawer, IUpdateableEntity {
    private BoundingArea _boundingArea;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea;

    /// <inheritdoc />
    public int UpdateOrder => 0;

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this.PrimitiveDrawer == null || this.LineThickness <= 0f || this.Color == Color.Transparent) {
            return;
        }

        if (this.SpriteBatch is { } spriteBatch && !this._boundingArea.IsEmpty) {
            var minimum = this._boundingArea.Minimum;
            var maximum = this._boundingArea.Maximum;
            var lineThickness = this.GetLineThickness(viewBoundingArea.Height);

            var points = new[] { minimum, new Vector2(minimum.X, maximum.Y), maximum, new Vector2(maximum.X, minimum.Y) };
            this.PrimitiveDrawer.DrawPolygon(
                spriteBatch,
                this.Settings.PixelsPerUnit,
                this.Color,
                lineThickness,
                true,
                points);
        }
    }

    public void Update(FrameTime frameTime, InputState inputState) {
        if (this.Parent is IBoundable boundable) {
            if (boundable.BoundingArea != this._boundingArea) {
                this._boundingArea = boundable.BoundingArea;
                this.BoundingAreaChanged.SafeInvoke(this);
            }
        }
        else {
            this._boundingArea = BoundingArea.Empty;
        }
    }
}