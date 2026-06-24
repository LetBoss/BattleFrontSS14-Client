using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;

namespace Content.Client.Stylesheets;

public sealed class StylesheetManager : IStylesheetManager
{
	[Dependency]
	private IUserInterfaceManager _userInterfaceManager;

	[Dependency]
	private IResourceCache _resourceCache;

	public Stylesheet SheetNano { get; private set; }

	public Stylesheet SheetSpace { get; private set; }

	public void Initialize()
	{
		SheetNano = new StyleNano(_resourceCache).Stylesheet;
		SheetSpace = new StyleSpace(_resourceCache).Stylesheet;
		_userInterfaceManager.Stylesheet = SheetNano;
	}
}
