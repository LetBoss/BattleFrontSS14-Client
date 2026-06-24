// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Lobby.UI.CivLoadoutWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.UserInterface.MainMenu;
using Content.Client._PUBG.UserInterface.MainMenu.Tabs;
using Content.Shared._PUBG.Skin;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Lobby.UI;

public sealed class CivLoadoutWindow : DefaultWindow
{
  private readonly SkinsTab _skinsTab;
  private readonly CivLoadoutEditorControl _loadoutEditor;

  public event Action<Dictionary<string, string>>? OnApplyOutfit;

  public CivLoadoutWindow()
  {
    this.Title = Loc.GetString("civ-lobby-loadout-title");
    ((Control) this).MinSize = new Vector2(1250f, 850f);
    ((Control) this).SetSize = new Vector2(1250f, 850f);
    SkinsTab skinsTab = new SkinsTab();
    skinsTab.AllowedCategories = new HashSet<SkinCategory>()
    {
      SkinCategory.Neck,
      SkinCategory.Ghost
    };
    skinsTab.ShowCollectionProgress = false;
    ((Control) skinsTab).HorizontalExpand = true;
    ((Control) skinsTab).VerticalExpand = true;
    this._skinsTab = skinsTab;
    this._skinsTab.OnApply += (Action<Dictionary<string, string>>) (outfit =>
    {
      Action<Dictionary<string, string>> onApplyOutfit = this.OnApplyOutfit;
      if (onApplyOutfit == null)
        return;
      onApplyOutfit(outfit);
    });
    this._loadoutEditor = new CivLoadoutEditorControl();
    TabContainer tabContainer1 = new TabContainer();
    ((Control) tabContainer1).HorizontalExpand = true;
    ((Control) tabContainer1).VerticalExpand = true;
    TabContainer tabContainer2 = tabContainer1;
    ((Control) tabContainer2).AddChild((Control) this._skinsTab);
    ((Control) tabContainer2).AddChild((Control) this._loadoutEditor);
    tabContainer2.SetTabTitle(0, Loc.GetString("civ-loadout-tab-cosmetics"));
    tabContainer2.SetTabTitle(1, Loc.GetString("civ-loadout-tab-gear"));
    this.Contents.AddChild((Control) tabContainer2);
    this._skinsTab.LoadSubcategory(SkinsSubcategory.MySkins);
  }

  public void UpdateFromSkinState(SkinStateMessage msg)
  {
    this._skinsTab.UpdateData(msg.AllItems, msg.UnlockedItems, msg.ItemExpiresAt, msg.RecipePrices, msg.ShopItems, msg.CurrentOutfit, msg.PlayerCoins, msg.PlayerScrap, msg.PlayerPremiumCoins, msg.AllEmotes, msg.UnlockedEmotes, msg.EquippedEmotes, msg.MaxEmoteSlots, msg.TotalCaseDropSkins, msg.UnlockedCaseDropSkins, msg.TotalUniqueSkins, msg.TotalEmotes, msg.AvailableEmotes);
  }

  public virtual void Close()
  {
    this._skinsTab.Cleanup();
    this._loadoutEditor.Cleanup();
    ((BaseWindow) this).Close();
  }
}
