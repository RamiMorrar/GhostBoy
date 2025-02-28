namespace Macabresoft.Macabre2D.UI.AvaloniaInterop;

using System.Collections.Generic;
using Avalonia.Input;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// A map between <see cref="Key" /> and <see cref="Keys" />.
/// </summary>
internal static class KeyMap {
    private static readonly IReadOnlyDictionary<Key, Keys> _map = new Dictionary<Key, Keys> {
        { Key.A, Keys.A },
        { Key.Add, Keys.Add },
        { Key.B, Keys.B },
        { Key.Back, Keys.Back },
        { Key.C, Keys.C },
        { Key.CapsLock, Keys.CapsLock },
        { Key.D, Keys.D },
        { Key.D0, Keys.D0 },
        { Key.D1, Keys.D1 },
        { Key.D2, Keys.D2 },
        { Key.D3, Keys.D3 },
        { Key.D4, Keys.D4 },
        { Key.D5, Keys.D5 },
        { Key.D6, Keys.D6 },
        { Key.D7, Keys.D7 },
        { Key.D8, Keys.D8 },
        { Key.D9, Keys.D9 },
        { Key.Delete, Keys.Delete },
        { Key.Divide, Keys.Divide },
        { Key.Down, Keys.Down },
        { Key.E, Keys.E },
        { Key.End, Keys.End },
        { Key.Enter, Keys.Enter },
        { Key.Escape, Keys.Escape },
        { Key.Execute, Keys.Execute },
        { Key.F, Keys.F },
        { Key.F1, Keys.F1 },
        { Key.F2, Keys.F2 },
        { Key.F3, Keys.F3 },
        { Key.F4, Keys.F4 },
        { Key.F5, Keys.F5 },
        { Key.F6, Keys.F6 },
        { Key.F7, Keys.F7 },
        { Key.F8, Keys.F8 },
        { Key.F9, Keys.F9 },
        { Key.F10, Keys.F10 },
        { Key.F11, Keys.F11 },
        { Key.F12, Keys.F12 },
        { Key.G, Keys.G },
        { Key.H, Keys.H },
        { Key.Help, Keys.Help },
        { Key.Home, Keys.Home },
        { Key.I, Keys.I },
        { Key.Insert, Keys.Insert },
        { Key.J, Keys.J },
        { Key.K, Keys.K },
        { Key.L, Keys.L },
        { Key.Left, Keys.Left },
        { Key.LeftAlt, Keys.LeftAlt },
        { Key.LeftCtrl, Keys.LeftControl },
        { Key.LeftShift, Keys.LeftShift },
        { Key.LWin, Keys.LeftWindows },
        { Key.M, Keys.M },
        { Key.Multiply, Keys.Multiply },
        { Key.N, Keys.N },
        { Key.NumPad0, Keys.NumPad0 },
        { Key.NumPad1, Keys.NumPad1 },
        { Key.NumPad2, Keys.NumPad2 },
        { Key.NumPad3, Keys.NumPad3 },
        { Key.NumPad4, Keys.NumPad4 },
        { Key.NumPad5, Keys.NumPad5 },
        { Key.NumPad6, Keys.NumPad6 },
        { Key.NumPad7, Keys.NumPad7 },
        { Key.NumPad8, Keys.NumPad8 },
        { Key.NumPad9, Keys.NumPad9 },
        { Key.O, Keys.O },
        { Key.P, Keys.P },
        { Key.PageDown, Keys.PageDown },
        { Key.PageUp, Keys.PageUp },
        { Key.Q, Keys.Q },
        { Key.R, Keys.R },
        { Key.Right, Keys.Right },
        { Key.RightAlt, Keys.RightAlt },
        { Key.RightCtrl, Keys.RightControl },
        { Key.RightShift, Keys.RightShift },
        { Key.RWin, Keys.RightWindows },
        { Key.S, Keys.S },
        { Key.Scroll, Keys.Scroll },
        { Key.Select, Keys.Select },
        { Key.Separator, Keys.Separator },
        { Key.Space, Keys.Space },
        { Key.Subtract, Keys.Subtract },
        { Key.T, Keys.T },
        { Key.Tab, Keys.Tab },
        { Key.U, Keys.U },
        { Key.Up, Keys.Up },
        { Key.V, Keys.V },
        { Key.W, Keys.W },
        { Key.X, Keys.X },
        { Key.Y, Keys.Y },
        { Key.Z, Keys.Z },
        { Key.Zoom, Keys.Zoom }
    };

    /// <summary>
    /// Tries to convert an Avalonia <see cref="Key" /> to a MonoGame <see cref="Keys" />.
    /// </summary>
    /// <param name="avaloniaKey">The avalonia key.</param>
    /// <param name="monoGameKey">The mono game key.</param>
    /// <returns>A value indicating whether or not a conversion was found.</returns>
    public static bool TryConvertKey(this Key avaloniaKey, out Keys monoGameKey) {
        return _map.TryGetValue(avaloniaKey, out monoGameKey);
    }
}