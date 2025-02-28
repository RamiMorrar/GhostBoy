namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// The display mode.
/// </summary>
public enum DisplayModes : byte {
    Borderless = 0,
    Fullscreen = 1,
    Windowed = 2
}

/// <summary>
/// DefaultGraphics settings such as resolution and display mode.
/// </summary>
[DataContract]
[Category(CommonCategories.DefaultGraphics)]
public sealed class GraphicsSettings {
    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsSettings" /> class.
    /// </summary>
    public GraphicsSettings() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsSettings" /> class.
    /// </summary>
    /// <param name="displayMode">The display mode.</param>
    /// <param name="resolution">The resolution.</param>
    public GraphicsSettings(DisplayModes displayMode, Point resolution) {
        this.DisplayMode = displayMode;
        this.Resolution = resolution;
    }

    /// <summary>
    /// Gets or sets the display mode.
    /// </summary>
    /// <value>The display mode.</value>
    [DataMember]
    public DisplayModes DisplayMode { get; set; } = DisplayModes.Windowed;

    /// <summary>
    /// Gets or sets the resolution.
    /// </summary>
    /// <value>The resolution.</value>
    [DataMember]
    public Point Resolution { get; set; } = new(1920, 1080);

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A clone of this instance.</returns>
    public GraphicsSettings Clone() {
        return new GraphicsSettings(this.DisplayMode, this.Resolution);
    }
}