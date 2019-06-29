﻿namespace Macabre2D.UI.Common {

    using GalaSoft.MvvmLight.CommandWpf;
    using System;

    public interface INamedValueEditor {
        object Owner { get; set; }

        string PropertyName { get; set; }

        string Title { get; set; }
    }

    public interface INamedValueEditor<T> : INamedValueEditor {
        T Value { get; set; }

        RelayCommand<EditableValueChangedEventArgs<T>> ValueChangedCommand { get; }
    }

    public sealed class EditableValueChangedEventArgs<T> : EventArgs {

        public EditableValueChangedEventArgs(T newValue, T oldValue, string propertyName) {
            this.NewValue = newValue;
            this.OldValue = oldValue;
            this.PropertyName = propertyName;
        }

        public T NewValue { get; }

        public T OldValue { get; }

        public string PropertyName { get; }
    }
}