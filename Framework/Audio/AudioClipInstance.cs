﻿namespace Macabresoft.Macabre2D.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Runtime.Serialization;

/// <summary>
/// Interface for an instance of a <see cref="AudioClipAsset" />.
/// </summary>
public interface IAudioClipInstance {
    /// <summary>
    /// Gets the state.
    /// </summary>
    SoundState State { get; }

    /// <summary>
    /// Gets or sets the pan.
    /// </summary>
    float Pan { get; set; }

    /// <summary>
    /// Gets or sets the pitch.
    /// </summary>
    float Pitch { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="AudioPlayer" /> should loop.
    /// </summary>
    bool ShouldLoop { get; set; }

    /// <summary>
    /// Gets or sets the volume.
    /// </summary>
    float Volume { get; set; }

    /// <summary>
    /// Pauses this instance.
    /// </summary>
    void Pause();

    /// <summary>
    /// Play this instance.
    /// </summary>
    void Play();

    /// <summary>
    /// Stop this instance.
    /// </summary>
    void Stop();

    /// <summary>
    /// Stop this instance.
    /// </summary>
    /// <param name="isImmediate">If set to <c>true</c> is immediate.</param>
    void Stop(bool isImmediate);
}

/// <summary>
/// An instance of a <see cref="AudioClipAsset" /> that can handle operations for a <see cref="SoundEffectInstance" />.
/// </summary>
public sealed class AudioClipInstance : IAudioClipInstance {
    private readonly SoundEffectInstance _instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundEffectInstance" /> class.
    /// </summary>
    public AudioClipInstance(SoundEffectInstance instance) {
        this._instance = instance;
    }

    /// <summary>
    /// Gets an empty instance of <see cref="IAudioClipInstance" />.
    /// </summary>
    public static IAudioClipInstance Empty => new EmptyAudioClipInstance();

    /// <inheritdoc />
    public SoundState State => this._instance.State;

    /// <inheritdoc />
    public float Pan {
        get => this._instance.Pan;
        set => this._instance.Pan = MathHelper.Clamp(value, -1f, 1f);
    }

    /// <inheritdoc />
    public float Pitch {
        get => this._instance.Pitch;
        set => this._instance.Pitch = MathHelper.Clamp(value, -1f, 1f);
    }

    /// <inheritdoc />
    [DataMember(Order = 1, Name = "Should Loop")]
    public bool ShouldLoop {
        get => this._instance.IsLooped;
        set => this._instance.IsLooped = value;
    }

    /// <inheritdoc />
    [DataMember(Order = 2, Name = "Volume")]
    public float Volume {
        get => this._instance.Volume;
        set => this._instance.Volume = MathHelper.Clamp(value, 0f, 1f);
    }

    /// <inheritdoc />
    public void Pause() {
        if (this._instance.State == SoundState.Playing) {
            this._instance.Pause();
        }
    }

    /// <inheritdoc />
    public void Play() {
        this._instance.IsLooped = this.ShouldLoop;
        this._instance.Play();
    }

    /// <inheritdoc />
    public void Stop() {
        this.Stop(true);
    }

    /// <inheritdoc />
    public void Stop(bool isImmediate) {
        this._instance.Stop(isImmediate);
    }

    private sealed class EmptyAudioClipInstance : IAudioClipInstance {
        public SoundState State => SoundState.Stopped;
        public float Pan { get; set; }
        public float Pitch { get; set; }
        public bool ShouldLoop { get; set; }
        public float Volume { get; set; }

        public void Pause() {
        }

        public void Play() {
        }

        public void Stop() {
        }

        public void Stop(bool isImmediate) {
        }
    }
}