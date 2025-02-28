﻿namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Input;

public class InputActionControl : UserControl {
    public static readonly DirectProperty<InputActionControl, string> ActionNameProperty =
        AvaloniaProperty.RegisterDirect<InputActionControl, string>(
            nameof(ActionName),
            editor => editor.ActionName,
            (editor, value) => editor.ActionName = value,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<InputActionControl, Buttons> SelectedGamePadButtonsProperty =
        AvaloniaProperty.RegisterDirect<InputActionControl, Buttons>(
            nameof(SelectedGamePadButtons),
            editor => editor.SelectedGamePadButtons,
            (editor, value) => editor.SelectedGamePadButtons = value,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<InputActionControl, Keys> SelectedKeyProperty =
        AvaloniaProperty.RegisterDirect<InputActionControl, Keys>(
            nameof(SelectedKey),
            editor => editor.SelectedKey,
            (editor, value) => editor.SelectedKey = value,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<InputActionControl, MouseButton> SelectedMouseButtonProperty =
        AvaloniaProperty.RegisterDirect<InputActionControl, MouseButton>(
            nameof(SelectedMouseButton),
            editor => editor.SelectedMouseButton,
            (editor, value) => editor.SelectedMouseButton = value,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<InputActionControl, bool> IsPredefinedProperty =
        AvaloniaProperty.RegisterDirect<InputActionControl, bool>(
            nameof(IsPredefined),
            editor => editor.IsPredefined,
            (editor, value) => editor.IsPredefined = value,
            defaultBindingMode: BindingMode.OneWay);
    
    public static readonly StyledProperty<InputAction> ActionProperty =
        AvaloniaProperty.Register<InputActionControl, InputAction>(
            nameof(Action),
            defaultBindingMode: BindingMode.OneTime,
            notifying: OnSettingsOrActionChanging);

    public static readonly StyledProperty<InputSettings> InputSettingsProperty =
        AvaloniaProperty.Register<InputActionControl, InputSettings>(
            nameof(InputSettings),
            defaultBindingMode: BindingMode.OneTime,
            notifying: OnSettingsOrActionChanging);


    private readonly IUndoService _undoService;
    private string _actionName;
    private Buttons _selectedGamePadButtons;
    private Keys _selectedKey;
    private MouseButton _selectedMouseButton;
    private bool _isPredefined;

    static InputActionControl() {
        var gamePadButtons = Enum.GetValues<Buttons>().ToList();
        gamePadButtons.Remove(Buttons.None);
        AvailableGamePadButtons = gamePadButtons;
        AvailableKeys = Enum.GetValues<Keys>().ToList();
        AvailableMouseButtons = Enum.GetValues<MouseButton>().ToList();
    }

    public InputActionControl() : this(Resolver.Resolve<IUndoService>()) {
    }

    public InputActionControl(IUndoService undoService) {
        this._undoService = undoService;
        this.InitializeComponent();
    }
    
    public bool IsPredefined {
        get => this._isPredefined;
        set => this.SetAndRaise(IsPredefinedProperty, ref this._isPredefined, value);
    }

    public static IReadOnlyCollection<Buttons> AvailableGamePadButtons { get; }

    public static IReadOnlyCollection<Keys> AvailableKeys { get; }

    public static IReadOnlyCollection<MouseButton> AvailableMouseButtons { get; }

    public InputAction Action {
        get => this.GetValue(ActionProperty);
        set => this.SetValue(ActionProperty, value);
    }

    public string ActionName {
        get => this._actionName;
        set {
            if (value != this._actionName && this.InputSettings is { } inputSettings) {
                var originalValue = inputSettings.GetName(this.Action);
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(ActionNameProperty, ref this._actionName, value);
                        inputSettings.SetName(this.Action, value);
                    },
                    () =>
                    {
                        this.SetAndRaise(ActionNameProperty, ref this._actionName, originalValue);
                        inputSettings.SetName(this.Action, originalValue);
                    });
            }
        }
    }

    public InputSettings InputSettings {
        get => this.GetValue(InputSettingsProperty);
        set => this.SetValue(InputSettingsProperty, value);
    }

    public Buttons SelectedGamePadButtons {
        get => this._selectedGamePadButtons;
        set {
            if (value != this._selectedGamePadButtons && this.InputSettings is { } inputSettings) {
                inputSettings.DefaultBindings.TryGetBindings(this.Action, out var originalValue, out _, out _);
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(SelectedGamePadButtonsProperty, ref this._selectedGamePadButtons, value);
                        inputSettings.DefaultBindings.SetGamePadBinding(this.Action, value);
                    },
                    () =>
                    {
                        this.SetAndRaise(SelectedGamePadButtonsProperty, ref this._selectedGamePadButtons, originalValue);
                        inputSettings.DefaultBindings.SetGamePadBinding(this.Action, originalValue);
                    });
            }
        }
    }

    public Keys SelectedKey {
        get => this._selectedKey;
        set {
            if (value != this._selectedKey && this.InputSettings is { } inputSettings) {
                inputSettings.DefaultBindings.TryGetBindings(this.Action, out _, out var originalValue, out _);
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(SelectedKeyProperty, ref this._selectedKey, value);
                        inputSettings.DefaultBindings.SetKeyBinding(this.Action, value);
                    },
                    () =>
                    {
                        this.SetAndRaise(SelectedKeyProperty, ref this._selectedKey, originalValue);
                        inputSettings.DefaultBindings.SetKeyBinding(this.Action, originalValue);
                    });
            }
        }
    }

    public MouseButton SelectedMouseButton {
        get => this._selectedMouseButton;
        set {
            if (value != this._selectedMouseButton && this.InputSettings is { } inputSettings) {
                inputSettings.DefaultBindings.TryGetBindings(this.Action, out _, out _, out var originalValue);
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(SelectedMouseButtonProperty, ref this._selectedMouseButton, value);
                        inputSettings.DefaultBindings.SetMouseBinding(this.Action, value);
                    },
                    () =>
                    {
                        this.SetAndRaise(SelectedMouseButtonProperty, ref this._selectedMouseButton, originalValue);
                        inputSettings.DefaultBindings.SetMouseBinding(this.Action, originalValue);
                    });
            }
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private static void OnSettingsOrActionChanging(IAvaloniaObject control, bool isBeforeChange) {
        if (control is InputActionControl inputActionControl) {
            if (!isBeforeChange) {
                inputActionControl.Reset();
            }
        }
    }

    private void RaisePropertyChanged<T>(AvaloniaProperty<T> property, T value) {
        this.RaisePropertyChanged(property, Optional<T>.Empty, new BindingValue<T>(value));
    }

    private void Reset() {
        if (this.InputSettings is { } inputSettings && this.Action != InputAction.None) {
            this._actionName = inputSettings.GetName(this.Action);
            inputSettings.DefaultBindings.TryGetBindings(this.Action, out this._selectedGamePadButtons, out this._selectedKey, out this._selectedMouseButton);

            this.RaisePropertyChanged(ActionNameProperty, this._actionName);
            this.RaisePropertyChanged(SelectedGamePadButtonsProperty, this._selectedGamePadButtons);
            this.RaisePropertyChanged(SelectedKeyProperty, this._selectedKey);
            this.RaisePropertyChanged(SelectedMouseButtonProperty, this._selectedMouseButton);

            this.IsPredefined = InputSettings.PredefinedActions.Contains(this.Action);
        }
    }
}