﻿namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// Settings for the editor. What a novel idea!
/// </summary>
[DataContract]
public class EditorSettings {
    /// <summary>
    /// It's the editor settings file name.
    /// </summary>
    public const string FileName = "settings.m2deditor";

    /// <summary>
    /// Gets or sets the animation preview framerate.
    /// </summary>
    [DataMember]
    public byte AnimationPreviewFramerate { get; set; } = 8;

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    [DataMember]
    public Color BackgroundColor { get; set; } = DefinedColors.MacabresoftPurple;

    /// <summary>
    /// Gets or sets the position of the editor camera.
    /// </summary>
    [DataMember]
    public Vector2 CameraPosition { get; set; } = Vector2.Zero;

    /// <summary>
    /// Gets or sets the view height of the editor camera.
    /// </summary>
    [DataMember]
    public float CameraViewHeight { get; set; } = 10f;

    /// <summary>
    /// Gets or sets the last gizmo opened.
    /// </summary>
    [DataMember]
    public GizmoKind LastGizmoSelected { get; set; }

    /// <summary>
    /// Gets or sets the last scene opened.
    /// </summary>
    [DataMember]
    public Guid LastSceneOpened { get; set; }

    /// <summary>
    /// Gets or sets the last tab selected.
    /// </summary>
    [DataMember]
    public EditorTabs LastTabSelected { get; set; } = EditorTabs.Scene;

    /// <summary>
    /// Gets or sets a value indicating whether or not content should be rebuilt on next load.
    /// </summary>
    [DataMember]
    public bool ShouldRebuildContent { get; set; }
}