namespace Macabresoft.Macabre2D.UI.Editor;

using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using Unity;

public class ShaderReferenceEditor : BaseAssetReferenceEditor<ShaderReference, ShaderAsset> {
    public ShaderReferenceEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ILocalDialogService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public ShaderReferenceEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ILocalDialogService dialogService,
        IUndoService undoService) : base(dependencies, assetManager, dialogService, undoService) {
        this.InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}