﻿namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// User customizable settings.
/// </summary>
[DataContract]
[Category("User Settings")]
public class UserSettings : VersionedData {
    /// <summary>
    /// The settings file name.
    /// </summary>
    public const string FileName = "UserSettings.m2d";

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettings" /> class.
    /// </summary>
    public UserSettings() : this(new AudioSettings(), new GraphicsSettings(), new InputBindings()) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettings" /> class.
    /// </summary>
    /// <param name="settings">The game settings.</param>
    public UserSettings(IGameSettings settings) {
        this.Audio = settings.DefaultAudioSettings.Clone();
        this.Graphics = settings.DefaultGraphicsSettings.Clone();
        this.InputBindings = settings.InputSettings.DefaultBindings.Clone();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettings" /> class.
    /// </summary>
    /// <param name="audio">The audio settings.</param>
    /// <param name="graphics">The graphics settings.</param>
    /// <param name="inputBindings">The input bindings.</param>
    public UserSettings(AudioSettings audio, GraphicsSettings graphics, InputBindings inputBindings) {
        this.Audio = audio;
        this.Graphics = graphics;
        this.InputBindings = inputBindings;
    }

    /// <summary>
    /// Gets the audio settings.
    /// </summary>
    public AudioSettings Audio { get; }

    /// <summary>
    /// Gets the graphics settings.
    /// </summary>
    public GraphicsSettings Graphics { get; }

    /// <summary>
    /// Gets the input bindings.
    /// </summary>
    public InputBindings InputBindings { get; }
}