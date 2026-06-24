// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Gulag.GulagQueueHud
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Gulag;

public sealed class GulagQueueHud : PanelContainer
{
  private Label _titleLabel;
  private Label _positionLabel;
  private Label _infoLabel;
  private Button _hideButton;

  public event Action? HideRequested;

  public GulagQueueHud()
  {
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#000000CC", new Color?()),
      BorderColor = Color.FromHex((ReadOnlySpan<char>) "#FFA500", new Color?()),
      BorderThickness = new Thickness(3f)
    };
    ((StyleBox) styleBoxFlat).SetContentMarginOverride((StyleBox.Margin) 15, 20f);
    this.PanelOverride = (StyleBox) styleBoxFlat;
    ((Control) this).MinWidth = 400f;
    ((Control) this).MinHeight = 200f;
    BoxContainer boxContainer = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1,
      SeparationOverride = new int?(15)
    };
    Label label1 = new Label();
    label1.Text = Loc.GetString("gulag-ui-title");
    label1.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFA500", new Color?()));
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    this._titleLabel = label1;
    Label label2 = new Label();
    label2.Text = Loc.GetString("gulag-ui-queue-position", new (string, object)[2]
    {
      ("position", (object) "-"),
      ("total", (object) "-")
    });
    label2.FontColorOverride = new Color?(Color.White);
    ((Control) label2).HorizontalAlignment = (Control.HAlignment) 2;
    this._positionLabel = label2;
    Label label3 = new Label();
    label3.Text = Loc.GetString("gulag-ui-queue-info").Replace("\\n", "\n");
    label3.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#AAAAAA", new Color?()));
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
    this._infoLabel = label3;
    Button button = new Button();
    button.Text = Loc.GetString("gulag-ui-hide");
    ((Control) button).HorizontalAlignment = (Control.HAlignment) 2;
    this._hideButton = button;
    ((BaseButton) this._hideButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action hideRequested = this.HideRequested;
      if (hideRequested == null)
        return;
      hideRequested();
    });
    ((Control) boxContainer).AddChild((Control) this._titleLabel);
    ((Control) boxContainer).AddChild((Control) this._positionLabel);
    ((Control) boxContainer).AddChild((Control) this._infoLabel);
    ((Control) boxContainer).AddChild((Control) this._hideButton);
    ((Control) this).AddChild((Control) boxContainer);
  }

  public void UpdatePosition(int position, int total)
  {
    this._positionLabel.Text = Loc.GetString("gulag-ui-queue-position", new (string, object)[2]
    {
      (nameof (position), (object) position),
      (nameof (total), (object) total)
    });
  }
}
