using Content.Client.Credits;
using Content.Shared.CCVar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Info;

public sealed class DevInfoBanner : BoxContainer
{
	public DevInfoBanner()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		((Control)this).AddChild((Control)(object)val);
		IUriOpener uriOpener = IoCManager.Resolve<IUriOpener>();
		IConfigurationManager val2 = IoCManager.Resolve<IConfigurationManager>();
		string bugReport = val2.GetCVar<string>(CCVars.InfoLinksBugReport);
		if (bugReport != "")
		{
			Button val3 = new Button
			{
				Text = Loc.GetString("server-info-report-button")
			};
			((BaseButton)val3).OnPressed += delegate
			{
				uriOpener.OpenUri(bugReport);
			};
			((Control)val).AddChild((Control)(object)val3);
		}
		Button val4 = new Button
		{
			Text = Loc.GetString("server-info-credits-button")
		};
		((BaseButton)val4).OnPressed += delegate
		{
			((BaseWindow)new CreditsWindow()).Open();
		};
		((Control)val).AddChild((Control)(object)val4);
	}
}
