namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

public class CollisionMapEditor : ValueEditorControl<CollisionMap> {
    private readonly Dictionary<Layers, CheckBox> _layersToCheckBox = new();
    private readonly IProjectService _projectService;
    private readonly IUndoService _undoService;

    public CollisionMapEditor() : this(null, Resolver.Resolve<IProjectService>(), Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public CollisionMapEditor(ValueControlDependencies dependencies, IProjectService projectService, IUndoService undoService) : base(dependencies) {
        this._projectService = projectService;
        this._undoService = undoService;
        this.InitializeComponent();
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
        base.OnAttachedToLogicalTree(e);

        this.CreateMap();
    }

    private void CreateMap() {
        if (this.Value == null) {
            return;
        }

        var enabledLayers = this._projectService.CurrentProject?.Settings.LayerSettings.EnabledLayers ?? Layers.Default;

        var viewBox = this.LogicalChildren.OfType<Viewbox>().First();
        var grid = new Grid();
        viewBox.Child = grid;

        var rowValues = Enum.GetValues<Layers>().Where(x => (x & enabledLayers) != Layers.None).ToList();
        rowValues.Remove(Layers.Default);
        rowValues.Remove(Layers.None);
        var columnValues = rowValues.ToList();

        for (var i = 0; i <= rowValues.Count + 1; i++) {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        var row = 1;
        var column = columnValues.Count + 1;

        foreach (var columnValue in columnValues) {
            var textControl = new TextBlock {
                Text = ToDisplayNameConverter.Instance.Convert(columnValue, typeof(string), null, CultureInfo.CurrentCulture) as string,
                [ToolTip.TipProperty] = columnValue
            };

            textControl.Classes.Add("SmallLabel");

            var container = new Decorator {
                Child = textControl
            };

            var layoutTransform = new LayoutTransformControl {
                ClipToBounds = false,
                LayoutTransform = new RotateTransform(90),
                [Grid.RowProperty] = 0,
                [Grid.ColumnProperty] = column,
                Child = container
            };

            grid.Children.Add(layoutTransform);
            column--;
        }

        foreach (var rowValue in rowValues) {
            column = columnValues.Count + 1;

            var textControl = new TextBlock {
                Text = ToDisplayNameConverter.Instance.Convert(rowValue, typeof(string), null, CultureInfo.CurrentCulture) as string,
                [ToolTip.TipProperty] = rowValue,
                [Grid.RowProperty] = row,
                [Grid.ColumnProperty] = 0
            };

            textControl.Classes.Add("SmallLabel");
            grid.Children.Add(textControl);

            foreach (var columnValue in columnValues) {
                var checkBox = new CheckBox {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Command = ReactiveCommand.Create(() => this.ToggleCheckedWithUndo(rowValue, columnValue)),
                    [Grid.RowProperty] = row,
                    [Grid.ColumnProperty] = column
                };

                checkBox.Classes.Add("NoText");

                grid.Children.Add(checkBox);
                checkBox.IsChecked = this.Value.GetShouldCollide(rowValue, columnValue);
                this._layersToCheckBox[rowValue | columnValue] = checkBox;
                column--;
            }

            columnValues.Remove(rowValue);
            row++;
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void ToggleChecked(Layers firstLayer, Layers secondLayer) {
        var isChecked = this.Value.ToggleShouldCollide(firstLayer, secondLayer);

        if (this._layersToCheckBox.TryGetValue(firstLayer | secondLayer, out var checkBox)) {
            checkBox.IsChecked = isChecked;
        }
    }

    private void ToggleCheckedWithUndo(Layers firstLayer, Layers secondLayer) {
        this._undoService.Do(() => { this.ToggleChecked(firstLayer, secondLayer); }, () => { this.ToggleChecked(firstLayer, secondLayer); });
    }
}