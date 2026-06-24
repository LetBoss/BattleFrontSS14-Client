using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client._PUBG.UserInterface.MainMenu;
using Content.Client._PUBG.UserInterface.MainMenu.Tabs;
using Content.Shared._PUBG.Skin;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;

namespace Content.Client._CIV14merka.Lobby.UI;

public sealed class CivLoadoutWindow : DefaultWindow
{
	private readonly SkinsTab _skinsTab;

	private readonly CivLoadoutEditorControl _loadoutEditor;

	public event Action<Dictionary<string, string>>? OnApplyOutfit;

	public CivLoadoutWindow()
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		((DefaultWindow)this).Title = Loc.GetString("civ-lobby-loadout-title");
		((Control)this).MinSize = new Vector2(1250f, 850f);
		((Control)this).SetSize = new Vector2(1250f, 850f);
		SkinsTab obj = new SkinsTab
		{
			AllowedCategories = new HashSet<SkinCategory>
			{
				SkinCategory.Neck,
				SkinCategory.Ghost
			},
			ShowCollectionProgress = false
		};
		((Control)obj).HorizontalExpand = true;
		((Control)obj).VerticalExpand = true;
		_skinsTab = obj;
		_skinsTab.OnApply += delegate(Dictionary<string, string> outfit)
		{
			this.OnApplyOutfit?.Invoke(outfit);
		};
		_loadoutEditor = new CivLoadoutEditorControl();
		TabContainer val = new TabContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)val).AddChild((Control)(object)_skinsTab);
		((Control)val).AddChild((Control)(object)_loadoutEditor);
		val.SetTabTitle(0, Loc.GetString("civ-loadout-tab-cosmetics"));
		val.SetTabTitle(1, Loc.GetString("civ-loadout-tab-gear"));
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
		_skinsTab.LoadSubcategory(SkinsSubcategory.MySkins);
	}

	public void UpdateFromSkinState(SkinStateMessage msg)
	{
		_skinsTab.UpdateData(msg.AllItems, msg.UnlockedItems, msg.ItemExpiresAt, msg.RecipePrices, msg.ShopItems, msg.CurrentOutfit, msg.PlayerCoins, msg.PlayerScrap, msg.PlayerPremiumCoins, msg.AllEmotes, msg.UnlockedEmotes, msg.EquippedEmotes, msg.MaxEmoteSlots, msg.TotalCaseDropSkins, msg.UnlockedCaseDropSkins, msg.TotalUniqueSkins, msg.TotalEmotes, msg.AvailableEmotes);
	}

	public override void Close()
	{
		_skinsTab.Cleanup();
		_loadoutEditor.Cleanup();
		((BaseWindow)this).Close();
	}
}
