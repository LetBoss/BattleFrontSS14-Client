// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Connection.QueueOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client._PUBG.Connection;

public sealed class QueueOverlay : Control
{
  private readonly PanelContainer _panel;
  private readonly Label _titleLabel;
  private readonly Label _positionLabel;
  private readonly Label _infoLabel;
  private bool _accepted;
  private TimeSpan _acceptedTime;

  public QueueOverlay()
  {
    this.VerticalAlignment = (Control.VAlignment) 2;
    this.HorizontalAlignment = (Control.HAlignment) 2;
    PanelContainer panelContainer = new PanelContainer();
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.85f);
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(40f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(40f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(30f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(30f);
    panelContainer.PanelOverride = (StyleBox) styleBoxFlat;
    this._panel = panelContainer;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalAlignment = (Control.HAlignment) 2;
    BoxContainer boxContainer2 = boxContainer1;
    Label label1 = new Label();
    label1.Text = Loc.GetString("pubg-queue-title");
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).StyleClasses.Add("LabelHeadingBigger");
    ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 20f);
    this._titleLabel = label1;
    Label label2 = new Label();
    label2.Text = "0 / 0";
    ((Control) label2).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label2).StyleClasses.Add("LabelHeadingBigger");
    ((Control) label2).Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
    this._positionLabel = label2;
    Label label3 = new Label();
    label3.Text = Loc.GetString("pubg-queue-waiting");
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label3).Margin = new Thickness(0.0f, 10f, 0.0f, 0.0f);
    this._infoLabel = label3;
    ((Control) boxContainer2).AddChild((Control) this._titleLabel);
    ((Control) boxContainer2).AddChild((Control) this._positionLabel);
    ((Control) boxContainer2).AddChild((Control) this._infoLabel);
    ((Control) this._panel).AddChild((Control) boxContainer2);
    this.AddChild((Control) this._panel);
  }

  public void UpdatePosition(int position, int total)
  {
    if (this._accepted)
      return;
    this._positionLabel.Text = $"{position} / {total}";
  }

  public void ShowAccepted()
  {
    this._accepted = true;
    this._acceptedTime = IoCManager.Resolve<IGameTiming>().CurTime;
    this._titleLabel.Text = Loc.GetString("pubg-queue-accepted-title");
    ((Control) this._positionLabel).Visible = false;
    this._infoLabel.Text = Loc.GetString("pubg-queue-accepted");
    ((Control) this._infoLabel).Modulate = Color.LimeGreen;
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (!this._accepted || !(IoCManager.Resolve<IGameTiming>().CurTime - this._acceptedTime > TimeSpan.FromSeconds(3L)))
      return;
    this.Orphan();
  }
}
