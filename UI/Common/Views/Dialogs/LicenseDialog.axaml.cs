namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

public class LicenseDialog : BaseDialog {
    private readonly ObservableCollectionExtended<LicenseDefinition> _filteredLicenses = new();

    [InjectionConstructor]
    public LicenseDialog() {
        this.CollapseCommand = ReactiveCommand.Create(() => this.AdjustGroupBoxes(false));
        this.ExpandCommand = ReactiveCommand.Create(() => this.AdjustGroupBoxes(true));
        
        this.FilterLicenses(string.Empty);
        this.InitializeComponent();
    }

    public ICommand CollapseCommand { get; }

    public ICommand ExpandCommand { get; }

    public IReadOnlyCollection<LicenseDefinition> FilteredLicenses => this._filteredLicenses;

    public IReadOnlyCollection<LicenseDefinition> Licenses => LicenseHelper.Definitions;

    private void AdjustGroupBoxes(bool showContent) {
        foreach (var groupBox in this.GetVisualDescendants().OfType<GroupBox>()) {
            groupBox.ShowContent = showContent;
        }
    }

    private void AutoCompleteBox_OnTextChanged(object sender, EventArgs e) {
        if (sender is AutoCompleteBox autoCompleteBox) {
            this.FilterLicenses(autoCompleteBox.Text);
        }
    }

    private void FilterLicenses(string filterText) {
        this._filteredLicenses.Reset(!string.IsNullOrEmpty(filterText) ? this.Licenses.Where(x => x.Product.Contains(filterText, StringComparison.OrdinalIgnoreCase)) : this.Licenses);
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}