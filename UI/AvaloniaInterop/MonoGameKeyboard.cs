namespace Macabresoft.Macabre2D.UI.AvaloniaInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// A wrapper for MonoGame's <see cref="KeyboardState" />.
/// </summary>
public sealed class MonoGameKeyboard {
    private readonly IInputElement _focusElement;
    private readonly HashSet<Keys> _pressedKeys = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MonoGameKeyboard" /> class.
    /// </summary>
    /// <param name="focusElement">The focus element.</param>
    /// <exception cref="ArgumentNullException">focusElement</exception>
    public MonoGameKeyboard(IInputElement focusElement) {
        this._focusElement = focusElement ?? throw new ArgumentNullException(nameof(focusElement));
        this._focusElement.KeyDown += this.OnKeyDown;
        this._focusElement.KeyUp += this.OnKeyUp;
    }

    /// <summary>
    /// Gets the state.
    /// </summary>
    /// <returns>The keyboard state.</returns>
    public KeyboardState GetState() {
        if (this._focusElement.IsPointerOver && KeyboardDevice.Instance.FocusedElement != this._focusElement && ActiveWindowHelper.IsControlOnActiveWindow(this._focusElement)) {
            // we assume the user wants keyboard input into the control when his mouse is over
            // it in order for the events to register we must focus it
            this._focusElement.Focus();
        }

        return this._focusElement.IsFocused ? new KeyboardState(this._pressedKeys.ToArray()) : new KeyboardState();
    }

    private void OnKeyDown(object sender, KeyEventArgs e) {
        if (this._focusElement.IsFocused && e.Key.TryConvertKey(out var key)) {
            this._pressedKeys.Add(key);
        }
    }

    private void OnKeyUp(object sender, KeyEventArgs e) {
        if (e.Key.TryConvertKey(out var key)) {
            this._pressedKeys.Remove(key);
        }
    }
}