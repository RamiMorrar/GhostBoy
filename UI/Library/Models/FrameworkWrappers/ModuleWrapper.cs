﻿namespace Macabre2D.UI.Library.Models.FrameworkWrappers {

    using Macabre2D.Framework;
    using Macabre2D.UI.Library.Common;

    public sealed class ModuleWrapper : NotifyPropertyChanged {

        public ModuleWrapper(BaseModule module) {
            this.Module = module;
        }

        public BaseModule Module { get; }

        public string Name {
            get {
                return this.Module.Name;
            }
        }

        public void UpdateProperty(string pathToProperty, object newValue) {
            this.Module.SetProperty(pathToProperty, newValue);
            this.RaisePropertyChanged(pathToProperty);
        }
    }
}