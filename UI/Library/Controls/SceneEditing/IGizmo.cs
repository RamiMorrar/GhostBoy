﻿namespace Macabre2D.UI.Library.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.UI.Library.Models.FrameworkWrappers;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public interface IGizmo {
        string EditingPropertyName { get; }
        bool OverrideSelectionDisplay { get; }

        void Draw(FrameTime frameTime, BoundingArea viewBoundingArea, BaseComponent selectedComponent);

        void Initialize(SceneEditor game);

        bool Update(FrameTime frameTime, MouseState mouseState, KeyboardState keyboardState, Vector2 mousePosition, ComponentWrapper selectedComponent);
    }
}