// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivBotOrderRadialMenu
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Content.Shared._CIV14merka.Commander;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivBotOrderRadialMenu : RadialMenu
{
  private readonly RadialContainer _options;
  private readonly PanelContainer _infoPanel;
  private readonly RichTextLabel _titleLabel;

  public event Action<CivBotOrderType>? OnOrderSelected;

  public CivBotOrderRadialMenu()
  {
    ((Control) this).MinSize = new Vector2(420f, 420f);
    ((Control) this).HorizontalExpand = true;
    ((Control) this).VerticalExpand = true;
    this.CloseButtonStyleClass = "RadialMenuCloseButton";
    this.BackButtonStyleClass = "RadialMenuBackButton";
    RadialContainer radialContainer = new RadialContainer();
    ((Control) radialContainer).HorizontalExpand = true;
    ((Control) radialContainer).VerticalExpand = true;
    radialContainer.InitialRadius = 110f;
    radialContainer.InnerRadiusMultiplier = 0.62f;
    radialContainer.OuterRadiusMultiplier = 1.55f;
    radialContainer.ReserveSpaceForHiddenChildren = false;
    radialContainer.RadialAlignment = RadialContainer.RAlignment.Clockwise;
    this._options = radialContainer;
    ((Control) this).AddChild((Control) this._options);
    PanelContainer panelContainer = new PanelContainer();
    ((Control) panelContainer).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) panelContainer).VerticalAlignment = (Control.VAlignment) 3;
    ((Control) panelContainer).Margin = new Thickness(0.0f, 0.0f, 0.0f, 24f);
    ((Control) panelContainer).MinSize = new Vector2(200f, 40f);
    ((Control) panelContainer).MouseFilter = (Control.MouseFilterMode) 2;
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    Color color1 = Color.FromHex((ReadOnlySpan<char>) "#0F1821", new Color?());
    styleBoxFlat.BackgroundColor = ((Color) ref color1).WithAlpha(0.94f);
    Color color2 = Color.FromHex((ReadOnlySpan<char>) "#D8B775", new Color?());
    styleBoxFlat.BorderColor = ((Color) ref color2).WithAlpha(0.85f);
    styleBoxFlat.BorderThickness = new Thickness(2f);
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(12f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(8f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(12f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(8f);
    panelContainer.PanelOverride = (StyleBox) styleBoxFlat;
    this._infoPanel = panelContainer;
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).HorizontalExpand = true;
    ((Control) richTextLabel).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) richTextLabel).MouseFilter = (Control.MouseFilterMode) 2;
    this._titleLabel = richTextLabel;
    ((Control) this._infoPanel).AddChild((Control) this._titleLabel);
    ((Control) this).AddChild((Control) this._infoPanel);
    this.SetInfoText(Loc.GetString("civ-cmd-bot-radial-default"), (string) null);
    this.BuildOptions();
  }

  private void BuildOptions()
  {
    this.AddOption(CivBotOrderType.Move, Loc.GetString("civ-cmd-bot-radial-move-title"), Loc.GetString("civ-cmd-bot-radial-move-desc"), Color.FromHex((ReadOnlySpan<char>) "#4caf50", new Color?()));
    this.AddOption(CivBotOrderType.AttackMove, Loc.GetString("civ-cmd-bot-radial-attack-title"), Loc.GetString("civ-cmd-bot-radial-attack-desc"), Color.FromHex((ReadOnlySpan<char>) "#f44336", new Color?()));
    this.AddOption(CivBotOrderType.HoldPosition, Loc.GetString("civ-cmd-bot-radial-hold-title"), Loc.GetString("civ-cmd-bot-radial-hold-desc"), Color.FromHex((ReadOnlySpan<char>) "#2196f3", new Color?()));
    this.AddOption(CivBotOrderType.Follow, Loc.GetString("civ-cmd-bot-radial-follow-title"), Loc.GetString("civ-cmd-bot-radial-follow-desc"), Color.FromHex((ReadOnlySpan<char>) "#9c27b0", new Color?()));
    this.AddOption(CivBotOrderType.Defend, Loc.GetString("civ-cmd-bot-radial-defend-title"), Loc.GetString("civ-cmd-bot-radial-defend-desc"), Color.FromHex((ReadOnlySpan<char>) "#00bcd4", new Color?()));
    this.AddOption(CivBotOrderType.Patrol, Loc.GetString("civ-cmd-bot-radial-patrol-title"), Loc.GetString("civ-cmd-bot-radial-patrol-desc"), Color.FromHex((ReadOnlySpan<char>) "#ff9800", new Color?()));
  }

  private void AddOption(CivBotOrderType order, string title, string desc, Color accent)
  {
    RadialMenuTextureButtonWithSector buttonWithSector1 = new RadialMenuTextureButtonWithSector();
    ((Control) buttonWithSector1).SetSize = new Vector2(100f, 64f);
    buttonWithSector1.DrawBorder = true;
    buttonWithSector1.DrawBackground = true;
    Color color = Color.FromHex((ReadOnlySpan<char>) "#101923", new Color?());
    buttonWithSector1.BackgroundColor = ((Color) ref color).WithAlpha(0.94f);
    buttonWithSector1.HoverBackgroundColor = ((Color) ref accent).WithAlpha(0.26f);
    buttonWithSector1.BorderColor = ((Color) ref accent).WithAlpha(0.45f);
    buttonWithSector1.HoverBorderColor = accent;
    buttonWithSector1.SeparatorColor = ((Color) ref accent).WithAlpha(0.28f);
    ((Control) buttonWithSector1).MouseFilter = (Control.MouseFilterMode) 0;
    RadialMenuTextureButtonWithSector buttonWithSector2 = buttonWithSector1;
    RichTextLabel richTextLabel1 = new RichTextLabel();
    ((Control) richTextLabel1).HorizontalExpand = true;
    ((Control) richTextLabel1).VerticalExpand = true;
    ((Control) richTextLabel1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) richTextLabel1).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) richTextLabel1).Margin = new Thickness(8f, 4f);
    ((Control) richTextLabel1).MouseFilter = (Control.MouseFilterMode) 2;
    RichTextLabel richTextLabel2 = richTextLabel1;
    richTextLabel2.Text = $"[font size=13][color={((Color) ref accent).ToHex()}][bold]{FormattedMessage.EscapeText(title)}[/bold][/color][/font]";
    ((Control) buttonWithSector2).AddChild((Control) richTextLabel2);
    ((BaseButton) buttonWithSector2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action<CivBotOrderType> onOrderSelected = this.OnOrderSelected;
      if (onOrderSelected == null)
        return;
      onOrderSelected(order);
    });
    ((Control) buttonWithSector2).OnMouseEntered += (Action<GUIMouseHoverEventArgs>) (_ => this.SetInfoText(title, desc, new Color?(accent)));
    ((Control) buttonWithSector2).OnMouseExited += (Action<GUIMouseHoverEventArgs>) (_ => this.SetInfoText(Loc.GetString("civ-cmd-bot-radial-default"), (string) null));
    ((Control) this._options).AddChild((Control) buttonWithSector2);
  }

  private void SetInfoText(string title, string? desc, Color? accent = null)
  {
    Color color = accent ?? Color.FromHex((ReadOnlySpan<char>) "#D8B775", new Color?());
    string str1 = FormattedMessage.EscapeText(title);
    string str2 = $"[font size=14][color={((Color) ref color).ToHex()}][bold]{str1}[/bold][/color][/font]";
    if (desc != null)
      str2 = $"{str2}\n[font size=11][color=#aaaaaa]{FormattedMessage.EscapeText(desc)}[/color][/font]";
    this._titleLabel.Text = str2;
  }
}
