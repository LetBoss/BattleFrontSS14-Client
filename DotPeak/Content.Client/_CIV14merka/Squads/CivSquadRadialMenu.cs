// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Squads.CivSquadRadialMenu
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Squads;

public sealed class CivSquadRadialMenu : RadialMenu
{
  private readonly RadialContainer _options;
  private readonly PanelContainer _infoPanel;
  private readonly RichTextLabel _titleLabel;
  private readonly RichTextLabel _descriptionLabel;
  private string _defaultTitle = string.Empty;
  private string _defaultDescription = string.Empty;

  public event Action<CivSquadRadialAction>? OnActionSelected;

  public CivSquadRadialMenu()
  {
    ((Control) this).MinSize = new Vector2(560f, 560f);
    ((Control) this).HorizontalExpand = true;
    ((Control) this).VerticalExpand = true;
    this.CloseButtonStyleClass = "RadialMenuCloseButton";
    this.BackButtonStyleClass = "RadialMenuBackButton";
    RadialContainer radialContainer = new RadialContainer();
    ((Control) radialContainer).HorizontalExpand = true;
    ((Control) radialContainer).VerticalExpand = true;
    radialContainer.InitialRadius = 148f;
    radialContainer.InnerRadiusMultiplier = 0.62f;
    radialContainer.OuterRadiusMultiplier = 1.55f;
    radialContainer.ReserveSpaceForHiddenChildren = false;
    radialContainer.RadialAlignment = RadialContainer.RAlignment.Clockwise;
    this._options = radialContainer;
    ((Control) this).AddChild((Control) this._options);
    PanelContainer panelContainer = new PanelContainer();
    ((Control) panelContainer).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) panelContainer).VerticalAlignment = (Control.VAlignment) 3;
    ((Control) panelContainer).Margin = new Thickness(0.0f, 0.0f, 0.0f, 34f);
    ((Control) panelContainer).MinSize = new Vector2(276f, 92f);
    ((Control) panelContainer).MouseFilter = (Control.MouseFilterMode) 2;
    this._infoPanel = panelContainer;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(2);
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    ((Control) boxContainer1).MouseFilter = (Control.MouseFilterMode) 2;
    BoxContainer boxContainer2 = boxContainer1;
    RichTextLabel richTextLabel1 = new RichTextLabel();
    ((Control) richTextLabel1).HorizontalExpand = true;
    ((Control) richTextLabel1).MouseFilter = (Control.MouseFilterMode) 2;
    this._titleLabel = richTextLabel1;
    RichTextLabel richTextLabel2 = new RichTextLabel();
    ((Control) richTextLabel2).HorizontalExpand = true;
    ((Control) richTextLabel2).MouseFilter = (Control.MouseFilterMode) 2;
    this._descriptionLabel = richTextLabel2;
    ((Control) boxContainer2).AddChild((Control) this._titleLabel);
    ((Control) boxContainer2).AddChild((Control) this._descriptionLabel);
    ((Control) this._infoPanel).AddChild((Control) boxContainer2);
    ((Control) this).AddChild((Control) this._infoPanel);
  }

  public void SetOptions(
    int teamId,
    string title,
    string description,
    IReadOnlyList<CivSquadRadialOption> options)
  {
    ((Control) this._options).RemoveAllChildren();
    Color accent = CivSquadRadialMenu.GetTeamAccent(teamId);
    this._infoPanel.PanelOverride = (StyleBox) CivSquadRadialMenu.CreateInfoStyle(accent);
    this._defaultTitle = title;
    this._defaultDescription = description;
    this.SetInfoText(this._defaultTitle, this._defaultDescription, accent);
    foreach (CivSquadRadialOption option1 in (IEnumerable<CivSquadRadialOption>) options)
    {
      CivSquadRadialOption option = option1;
      CivSquadRadialButton squadRadialButton1 = new CivSquadRadialButton(option.Action, option.Title, option.Description, option.AccentColor);
      ((Control) squadRadialButton1).ToolTip = option.Tooltip;
      CivSquadRadialButton squadRadialButton2 = squadRadialButton1;
      ((BaseButton) squadRadialButton2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        Action<CivSquadRadialAction> onActionSelected = this.OnActionSelected;
        if (onActionSelected == null)
          return;
        onActionSelected(option.Action);
      });
      ((Control) squadRadialButton2).OnMouseEntered += (Action<GUIMouseHoverEventArgs>) (_ => this.SetInfoText(option.Title, option.Description, option.AccentColor));
      ((Control) squadRadialButton2).OnMouseExited += (Action<GUIMouseHoverEventArgs>) (_ => this.SetInfoText(this._defaultTitle, this._defaultDescription, accent));
      ((Control) this._options).AddChild((Control) squadRadialButton2);
    }
  }

  public bool TryGetHoveredAction(Control? hovered, out CivSquadRadialAction action)
  {
    if (hovered != null)
    {
      foreach (Control andLogicalAncestor in LogicalExtensions.GetSelfAndLogicalAncestors(hovered))
      {
        if (andLogicalAncestor is CivSquadRadialButton squadRadialButton)
        {
          action = squadRadialButton.Action;
          return true;
        }
      }
    }
    action = CivSquadRadialAction.Attack;
    return false;
  }

  private void SetInfoText(string title, string description, Color accent)
  {
    string str1 = FormattedMessage.EscapeText(title);
    string str2 = FormattedMessage.EscapeText(description);
    this._titleLabel.Text = $"[font size=16][color={((Color) ref accent).ToHex()}][bold]{str1}[/bold][/color][/font]";
    this._descriptionLabel.Text = $"[font size=11][color=#D4DEE8]{str2}[/color][/font]";
  }

  private static StyleBoxFlat CreateInfoStyle(Color accent)
  {
    StyleBoxFlat infoStyle = new StyleBoxFlat();
    Color color = Color.FromHex((ReadOnlySpan<char>) "#0F1821", new Color?());
    infoStyle.BackgroundColor = ((Color) ref color).WithAlpha(0.94f);
    infoStyle.BorderColor = ((Color) ref accent).WithAlpha(0.85f);
    infoStyle.BorderThickness = new Thickness(2f);
    ((StyleBox) infoStyle).SetContentMarginOverride((StyleBox.Margin) 8, 12f);
    ((StyleBox) infoStyle).SetContentMarginOverride((StyleBox.Margin) 1, 10f);
    ((StyleBox) infoStyle).SetContentMarginOverride((StyleBox.Margin) 4, 12f);
    ((StyleBox) infoStyle).SetContentMarginOverride((StyleBox.Margin) 2, 10f);
    return infoStyle;
  }

  private static Color GetTeamAccent(int teamId)
  {
    Color teamAccent;
    switch (teamId)
    {
      case 1:
        teamAccent = Color.FromHex((ReadOnlySpan<char>) "#5EA7FF", new Color?());
        break;
      case 2:
        teamAccent = Color.FromHex((ReadOnlySpan<char>) "#FF6F63", new Color?());
        break;
      default:
        teamAccent = Color.FromHex((ReadOnlySpan<char>) "#D8B775", new Color?());
        break;
    }
    return teamAccent;
  }
}
