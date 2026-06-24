// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.HeliSupply.CivHeliRoutePanel
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.HeliSupply;

public sealed class CivHeliRoutePanel : PanelContainer
{
  private readonly Label _pointsLabel;
  private readonly Label _etaLabel;
  private readonly Button _launch;

  public event Action? LaunchPressed;

  public event Action? BackPressed;

  public CivHeliRoutePanel()
  {
    this.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = new Color((byte) 17, (byte) 24, (byte) 39, (byte) 230)
    };
    ((Control) this).Visible = false;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).Margin = new Thickness(10f, 8f);
    BoxContainer boxContainer2 = boxContainer1;
    ((Control) boxContainer2).AddChild((Control) new Label()
    {
      Text = Loc.GetString("civ-heli-route-hint"),
      FontColorOverride = new Color?(new Color((byte) 229, (byte) 231, (byte) 235, byte.MaxValue))
    });
    this._pointsLabel = new Label()
    {
      FontColorOverride = new Color?(new Color((byte) 156, (byte) 163, (byte) 175, byte.MaxValue))
    };
    ((Control) boxContainer2).AddChild((Control) this._pointsLabel);
    this._etaLabel = new Label()
    {
      FontColorOverride = new Color?(new Color((byte) 74, (byte) 222, (byte) 128 /*0x80*/, byte.MaxValue))
    };
    ((Control) boxContainer2).AddChild((Control) this._etaLabel);
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer3).Margin = new Thickness(0.0f, 6f, 0.0f, 0.0f);
    BoxContainer boxContainer4 = boxContainer3;
    Button button1 = new Button();
    button1.Text = Loc.GetString("civ-heli-route-launch");
    ((Control) button1).MinSize = new Vector2(120f, 32f);
    this._launch = button1;
    ((BaseButton) this._launch).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action launchPressed = this.LaunchPressed;
      if (launchPressed == null)
        return;
      launchPressed();
    });
    ((Control) boxContainer4).AddChild((Control) this._launch);
    Button button2 = new Button();
    button2.Text = Loc.GetString("civ-heli-route-back");
    ((Control) button2).MinSize = new Vector2(120f, 32f);
    ((Control) button2).Margin = new Thickness(6f, 0.0f, 0.0f, 0.0f);
    Button button3 = button2;
    ((BaseButton) button3).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action backPressed = this.BackPressed;
      if (backPressed == null)
        return;
      backPressed();
    });
    ((Control) boxContainer4).AddChild((Control) button3);
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    ((Control) this).AddChild((Control) boxContainer2);
  }

  public void SetPointCount(int count)
  {
    this._pointsLabel.Text = Loc.GetString("civ-heli-route-points", new (string, object)[1]
    {
      (nameof (count), (object) count)
    });
  }

  public void SetEta(float? seconds)
  {
    ((Control) this._etaLabel).Visible = seconds.HasValue;
    if (!seconds.HasValue)
      return;
    this._etaLabel.Text = Loc.GetString("civ-heli-route-eta", new (string, object)[1]
    {
      ("eta", (object) (int) MathF.Round(seconds.Value))
    });
  }

  public void SetCost(int cost)
  {
    Button launch = this._launch;
    string str;
    if (cost <= 0)
      str = Loc.GetString("civ-heli-route-launch");
    else
      str = Loc.GetString("civ-heli-route-launch-cost", new (string, object)[1]
      {
        (nameof (cost), (object) cost)
      });
    launch.Text = str;
  }
}
