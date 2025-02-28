namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Interface for an system which runs operations for a <see cref="IScene" />.
/// </summary>
public interface ILoop : IUpdateableGameObject, INameable, IIdentifiable {
    /// <summary>
    /// Gets the loop.
    /// </summary>
    /// <value>The loop.</value>
    LoopKind Kind { get; }

    /// <summary>
    /// Initializes this service as a descendent of <paramref name="scene" />.
    /// </summary>
    /// <param name="scene">The scene.</param>
    void Initialize(IScene scene);
}

/// <summary>
/// Base class for a system which runs operations for a <see cref="IScene" />.
/// </summary>
[DataContract]
[Category("Loop")]
public abstract class Loop : PropertyChangedNotifier, ILoop {
    private bool _isEnabled = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Loop" /> class.
    /// </summary>
    protected Loop() {
        this.Name = this.GetType().Name;
    }

    /// <inheritdoc />
    public abstract LoopKind Kind { get; }

    /// <inheritdoc />
    [DataMember]
    public bool IsEnabled {
        get => this._isEnabled;
        set => this.Set(ref this._isEnabled, value);
    }

    /// <inheritdoc />
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// Gets the game.
    /// </summary>
    protected IGame Game => this.Scene.Game;

    /// <summary>
    /// Gets the scene.
    /// </summary>
    /// <value>The scene.</value>
    protected IScene Scene { get; private set; } = Framework.Scene.Empty;

    /// <inheritdoc />
    public virtual void Initialize(IScene scene) {
        this.Scene = scene;
    }

    /// <inheritdoc />
    public abstract void Update(FrameTime frameTime, InputState inputState);

    /// <inheritdoc />
    [DataMember]
    [EditorExclude]
    public Guid Id { get; set; } = Guid.NewGuid();
}