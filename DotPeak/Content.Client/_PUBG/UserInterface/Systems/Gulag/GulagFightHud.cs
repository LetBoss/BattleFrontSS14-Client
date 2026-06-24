// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Gulag.GulagFightHud
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

public sealed class GulagFightHud : PanelContainer
{
  private Label _opponentLabel;
  private Label _timerLabel;

  public GulagFightHud()
  {
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#000000DD", new Color?()),
      BorderColor = Color.FromHex((ReadOnlySpan<char>) "#DC3545", new Color?()),
      BorderThickness = new Thickness(0.0f, 0.0f, 0.0f, 3f)
    };
    ((StyleBox) styleBoxFlat).SetContentMarginOverride((StyleBox.Margin) 15, 15f);
    this.PanelOverride = (StyleBox) styleBoxFlat;
    ((Control) this).MinWidth = 350f;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(10);
    ((Control) boxContainer1).HorizontalAlignment = (Control.HAlignment) 2;
    BoxContainer boxContainer2 = boxContainer1;
    Label label1 = new Label();
    label1.Text = "⚔ БОЙ В ГУЛАГ ⚔";
    label1.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#DC3545", new Color?()));
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    Label label2 = label1;
    Label label3 = new Label();
    label3.Text = "Противник: -";
    label3.FontColorOverride = new Color?(Color.White);
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
    this._opponentLabel = label3;
    Label label4 = new Label();
    label4.Text = "00:60";
    label4.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFA500", new Color?()));
    ((Control) label4).HorizontalAlignment = (Control.HAlignment) 2;
    this._timerLabel = label4;
    ((Control) boxContainer2).AddChild((Control) label2);
    ((Control) boxContainer2).AddChild((Control) this._opponentLabel);
    ((Control) boxContainer2).AddChild((Control) this._timerLabel);
    ((Control) this).AddChild((Control) boxContainer2);
  }

  public void UpdateFight(string opponentUsername, string opponentRank, float timeRemaining)
  {
    this._opponentLabel.Text = $"Противник: {opponentUsername} (#{opponentRank})";
    this._timerLabel.Text = $"{(int) timeRemaining / 60:D2}:{(int) timeRemaining % 60:D2}";
    if ((double) timeRemaining <= 10.0)
      this._timerLabel.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#DC3545", new Color?()));
    else
      this._timerLabel.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFA500", new Color?()));
  }
}
