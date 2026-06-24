// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Lobby.UI.CivRosterWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Lobby.UI;

public sealed class CivRosterWindow : FancyWindow
{
  private static readonly Color DarkBg = Color.FromHex((ReadOnlySpan<char>) "#13161A", new Color?());

  public CivRosterControl RosterControl { get; }

  public CivRosterWindow()
  {
    this.Title = Loc.GetString("civ-lobby-roster-title");
    ((Control) this).MinSize = new Vector2(1000f, 650f);
    this.Resizable = true;
    PanelContainer panelContainer1 = new PanelContainer();
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = CivRosterWindow.DarkBg;
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(10f);
    panelContainer1.PanelOverride = (StyleBox) styleBoxFlat;
    ((Control) panelContainer1).HorizontalExpand = true;
    ((Control) panelContainer1).VerticalExpand = true;
    PanelContainer panelContainer2 = panelContainer1;
    CivRosterControl civRosterControl = new CivRosterControl();
    ((Control) civRosterControl).HorizontalExpand = true;
    ((Control) civRosterControl).VerticalExpand = true;
    this.RosterControl = civRosterControl;
    ((Control) panelContainer2).AddChild((Control) this.RosterControl);
    this.ContentsContainer.AddChild((Control) panelContainer2);
  }
}
