// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Squads.CivSquadRadialButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Squads;

public sealed class CivSquadRadialButton : RadialMenuTextureButtonWithSector
{
  private string _title = string.Empty;
  private string _description = string.Empty;
  private readonly Color _accentColor;
  private RichTextLabel? _label;

  public CivSquadRadialAction Action { get; }

  public CivSquadRadialButton(
    CivSquadRadialAction action,
    string title,
    string description,
    Color accentColor)
  {
    this.Action = action;
    this._title = title;
    this._description = description;
    this._accentColor = accentColor;
    ((Control) this).SetSize = new Vector2(136f, 86f);
    this.DrawBorder = true;
    this.DrawBackground = true;
    Color color = Color.FromHex((ReadOnlySpan<char>) "#101923", new Color?());
    this.BackgroundColor = ((Color) ref color).WithAlpha(0.94f);
    this.HoverBackgroundColor = ((Color) ref accentColor).WithAlpha(0.26f);
    this.BorderColor = ((Color) ref accentColor).WithAlpha(0.45f);
    this.HoverBorderColor = accentColor;
    this.SeparatorColor = ((Color) ref accentColor).WithAlpha(0.28f);
    ((Control) this).MouseFilter = (Control.MouseFilterMode) 0;
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).HorizontalExpand = true;
    ((Control) richTextLabel).VerticalExpand = true;
    ((Control) richTextLabel).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) richTextLabel).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) richTextLabel).Margin = new Thickness(10f, 8f);
    ((Control) richTextLabel).MouseFilter = (Control.MouseFilterMode) 2;
    this._label = richTextLabel;
    ((Control) this).AddChild((Control) this._label);
    this.UpdateLabel();
  }

  protected virtual void DrawModeChanged()
  {
    base.DrawModeChanged();
    this.UpdateLabel();
  }

  private void UpdateLabel()
  {
    if (this._label == null)
      return;
    int num = ((BaseButton) this).DrawMode - 1 > 1 ? (false ? 1 : 0) : (true ? 1 : 0);
    string str1 = num != 0 ? "#FFFFFF" : ((Color) ref this._accentColor).ToHex();
    string str2 = num != 0 ? "#F3F7FB" : "#C5D0DB";
    this._label.Text = $"[font size=13][color={str1}][bold]{FormattedMessage.EscapeText(this._title)}[/bold][/color][/font]\n[font size=10][color={str2}]{FormattedMessage.EscapeText(this._description)}[/color][/font]";
  }
}
