// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Lobby.UI.CivSponsorWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.UserInterface.MainMenu;
using Content.Client._PUBG.UserInterface.MainMenu.Tabs;
using Content.Shared._PUBG.Skin;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Lobby.UI;

public sealed class CivSponsorWindow : DefaultWindow
{
  private readonly ProfileTab _profileTab;

  public CivSponsorWindow()
  {
    this.Title = "✦ СПОНСОРКА ✦";
    ((Control) this).MinSize = new Vector2(1100f, 900f);
    ((Control) this).SetSize = new Vector2(1100f, 900f);
    this._profileTab = new ProfileTab();
    this._profileTab.ShowDisplaySettings = false;
    ((Control) this._profileTab).HorizontalExpand = true;
    ((Control) this._profileTab).VerticalExpand = true;
    this.Contents.AddChild((Control) this._profileTab);
    this._profileTab.LoadSubcategory(ProfileSubcategory.Sponsors);
  }

  public void UpdateFromSkinState(SkinStateMessage msg)
  {
    this._profileTab.UpdateSponsorData(msg.SponsorPermissions, msg.SponsorPermissionDetails, msg.SponsorDisplayTier, msg.SponsorActiveTiers, msg.SponsorDisplayMode, msg.SponsorPreferredTierKey);
  }

  public void UpdateSponsorData(
    Dictionary<string, int> permissions,
    List<SponsorPermissionDetailInfo> permissionDetails,
    SponsorTierInfo? displayTier,
    List<SponsorActiveTierInfo> activeTiers,
    SponsorDisplayMode displayMode,
    string? preferredTierKey)
  {
    this._profileTab.UpdateSponsorData(permissions, permissionDetails, displayTier, activeTiers, displayMode, preferredTierKey);
  }
}
