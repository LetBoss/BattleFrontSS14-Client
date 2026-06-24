using System.Collections.Generic;
using System.Numerics;
using Content.Client._PUBG.UserInterface.MainMenu;
using Content.Client._PUBG.UserInterface.MainMenu.Tabs;
using Content.Shared._PUBG.Skin;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client._CIV14merka.Lobby.UI;

public sealed class CivSponsorWindow : DefaultWindow
{
	private readonly ProfileTab _profileTab;

	public CivSponsorWindow()
	{
		((DefaultWindow)this).Title = "✦ СПОНСОРКА ✦";
		((Control)this).MinSize = new Vector2(1100f, 900f);
		((Control)this).SetSize = new Vector2(1100f, 900f);
		_profileTab = new ProfileTab();
		_profileTab.ShowDisplaySettings = false;
		((Control)_profileTab).HorizontalExpand = true;
		((Control)_profileTab).VerticalExpand = true;
		((DefaultWindow)this).Contents.AddChild((Control)(object)_profileTab);
		_profileTab.LoadSubcategory(ProfileSubcategory.Sponsors);
	}

	public void UpdateFromSkinState(SkinStateMessage msg)
	{
		_profileTab.UpdateSponsorData(msg.SponsorPermissions, msg.SponsorPermissionDetails, msg.SponsorDisplayTier, msg.SponsorActiveTiers, msg.SponsorDisplayMode, msg.SponsorPreferredTierKey);
	}

	public void UpdateSponsorData(Dictionary<string, int> permissions, List<SponsorPermissionDetailInfo> permissionDetails, SponsorTierInfo? displayTier, List<SponsorActiveTierInfo> activeTiers, SponsorDisplayMode displayMode, string? preferredTierKey)
	{
		_profileTab.UpdateSponsorData(permissions, permissionDetails, displayTier, activeTiers, displayMode, preferredTierKey);
	}
}
