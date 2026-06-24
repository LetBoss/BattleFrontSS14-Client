// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Gulag.GulagSpectatorHud
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Gulag;

public sealed class GulagSpectatorHud : PanelContainer
{
  private Label _fighter1Label;
  private Label _vsLabel;
  private Label _fighter2Label;
  private Label _timerLabel;
  private Label _queueLabel;

  public GulagSpectatorHud()
  {
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#000000DD", new Color?()),
      BorderColor = Color.FromHex((ReadOnlySpan<char>) "#6c757d", new Color?()),
      BorderThickness = new Thickness(0.0f, 0.0f, 0.0f, 3f)
    };
    ((StyleBox) styleBoxFlat).SetContentMarginOverride((StyleBox.Margin) 15, 15f);
    this.PanelOverride = (StyleBox) styleBoxFlat;
    ((Control) this).MinWidth = 400f;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(10);
    ((Control) boxContainer1).HorizontalAlignment = (Control.HAlignment) 2;
    BoxContainer boxContainer2 = boxContainer1;
    Label label1 = new Label();
    label1.Text = "\uD83D\uDC41 ГУЛАГ - РЕЖИМ НАБЛЮДЕНИЯ \uD83D\uDC41";
    label1.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#6c757d", new Color?()));
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    Label label2 = label1;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer3.SeparationOverride = new int?(15);
    ((Control) boxContainer3).HorizontalAlignment = (Control.HAlignment) 2;
    BoxContainer boxContainer4 = boxContainer3;
    this._fighter1Label = new Label()
    {
      Text = "Fighter1",
      FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FF0000", new Color?()))
    };
    this._vsLabel = new Label()
    {
      Text = "VS",
      FontColorOverride = new Color?(Color.White)
    };
    this._fighter2Label = new Label()
    {
      Text = "Fighter2",
      FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#0000FF", new Color?()))
    };
    ((Control) boxContainer4).AddChild((Control) this._fighter1Label);
    ((Control) boxContainer4).AddChild((Control) this._vsLabel);
    ((Control) boxContainer4).AddChild((Control) this._fighter2Label);
    Label label3 = new Label();
    label3.Text = "00:60";
    label3.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFA500", new Color?()));
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
    this._timerLabel = label3;
    Label label4 = new Label();
    label4.Text = "В очереди: 0";
    label4.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#AAAAAA", new Color?()));
    ((Control) label4).HorizontalAlignment = (Control.HAlignment) 2;
    this._queueLabel = label4;
    ((Control) boxContainer2).AddChild((Control) label2);
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    ((Control) boxContainer2).AddChild((Control) this._timerLabel);
    ((Control) boxContainer2).AddChild((Control) this._queueLabel);
    ((Control) this).AddChild((Control) boxContainer2);
  }

  public void UpdateFight(
    string fighter1Username,
    string fighter1Rank,
    string fighter2Username,
    string fighter2Rank,
    float timeRemaining,
    int queueCount)
  {
    this._fighter1Label.Text = $"{fighter1Username} (#{fighter1Rank})";
    this._fighter2Label.Text = $"{fighter2Username} (#{fighter2Rank})";
    this._timerLabel.Text = $"{(int) timeRemaining / 60:D2}:{(int) timeRemaining % 60:D2}";
    this._queueLabel.Text = $"В очереди: {queueCount}";
    if ((double) timeRemaining <= 10.0)
      this._timerLabel.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#DC3545", new Color?()));
    else
      this._timerLabel.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFA500", new Color?()));
  }
}
