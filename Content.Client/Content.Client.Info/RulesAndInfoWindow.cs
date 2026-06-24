using System.Numerics;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.Info;

public sealed class RulesAndInfoWindow : DefaultWindow
{
	[Dependency]
	private IResourceManager _resourceManager;

	public RulesAndInfoWindow()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<RulesAndInfoWindow>(this);
		((DefaultWindow)this).Title = Loc.GetString("ui-info-title");
		TabContainer val = new TabContainer();
		RulesControl rulesControl = new RulesControl();
		((Control)rulesControl).Margin = new Thickness(10f);
		RulesControl rulesControl2 = rulesControl;
		Info info = new Info();
		((Control)info).Margin = new Thickness(10f);
		Info info2 = info;
		((Control)val).AddChild((Control)(object)rulesControl2);
		((Control)val).AddChild((Control)(object)info2);
		TabContainer.SetTabTitle((Control)(object)rulesControl2, Loc.GetString("ui-info-tab-rules"));
		TabContainer.SetTabTitle((Control)(object)info2, Loc.GetString("ui-info-tab-tutorial"));
		PopulateTutorial(info2);
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
		((Control)this).SetSize = new Vector2(650f, 650f);
	}

	private void PopulateTutorial(Info tutorialList)
	{
		InfoControlsSection infoControlsSection = new InfoControlsSection();
		((Control)tutorialList.InfoContainer).AddChild((Control)(object)infoControlsSection);
		AddSection(tutorialList, Loc.GetString("ui-info-header-gameplay"), "Gameplay.txt", markup: true);
		AddSection(tutorialList, Loc.GetString("ui-info-header-sandbox"), "Sandbox.txt", markup: true);
		((BaseButton)infoControlsSection.ControlsButton).OnPressed += delegate
		{
			((Control)this).UserInterfaceManager.GetUIController<OptionsUIController>().OpenWindow();
		};
	}

	private static void AddSection(Info info, Control control)
	{
		((Control)info.InfoContainer).AddChild(control);
	}

	private void AddSection(Info info, string title, string path, bool markup = false)
	{
		AddSection(info, MakeSection(title, path, markup, _resourceManager));
	}

	private static Control MakeSection(string title, string path, bool markup, IResourceManager res)
	{
		return (Control)(object)new InfoSection(title, res.ContentFileReadAllText("/ServerInfo/" + path), markup);
	}
}
