// Decompiled with JetBrains decompiler
// Type: Content.Client.Medical.CrewMonitoring.CrewMonitoringNavMapControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Pinpointer.UI;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Medical.CrewMonitoring;

public sealed class CrewMonitoringNavMapControl : NavMapControl
{
  public NetEntity? Focus;
  public Dictionary<NetEntity, string> LocalizedNames = new Dictionary<NetEntity, string>();
  private Label _trackedEntityLabel;
  private PanelContainer _trackedEntityPanel;

  public CrewMonitoringNavMapControl()
  {
    this.WallColor = new Color((byte) 192 /*0xC0*/, (byte) 122, (byte) 196, byte.MaxValue);
    this.TileColor = new Color((byte) 71, (byte) 42, (byte) 72, byte.MaxValue);
    this.BackgroundColor = Color.FromSrgb(((Color) ref this.TileColor).WithAlpha(this.BackgroundOpacity));
    Label label = new Label();
    ((Control) label).Margin = new Thickness(10f, 8f);
    ((Control) label).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) label).Modulate = Color.White;
    this._trackedEntityLabel = label;
    PanelContainer panelContainer = new PanelContainer();
    panelContainer.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = this.BackgroundColor
    };
    ((Control) panelContainer).Margin = new Thickness(5f, 10f);
    ((Control) panelContainer).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) panelContainer).VerticalAlignment = (Control.VAlignment) 3;
    ((Control) panelContainer).Visible = false;
    this._trackedEntityPanel = panelContainer;
    ((Control) this._trackedEntityPanel).AddChild((Control) this._trackedEntityLabel);
    ((Control) this).AddChild((Control) this._trackedEntityPanel);
  }

  protected override void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (!this.Focus.HasValue)
    {
      this._trackedEntityLabel.Text = string.Empty;
      ((Control) this._trackedEntityPanel).Visible = false;
    }
    else
    {
      foreach ((NetEntity key1, NavMapBlip navMapBlip1) in this.TrackedEntities)
      {
        NetEntity key2 = key1;
        NavMapBlip navMapBlip2 = navMapBlip1;
        key1 = key2;
        NetEntity? focus = this.Focus;
        if ((focus.HasValue ? (NetEntity.op_Inequality(key1, focus.GetValueOrDefault()) ? 1 : 0) : 1) == 0)
        {
          string str;
          if (!this.LocalizedNames.TryGetValue(key2, out str))
            str = "Unknown";
          this._trackedEntityLabel.Text = $"{str}\nLocation: [x = {MathF.Round(((EntityCoordinates) ref navMapBlip2.Coordinates).X).ToString()}, y = {MathF.Round(((EntityCoordinates) ref navMapBlip2.Coordinates).Y).ToString()}]";
          ((Control) this._trackedEntityPanel).Visible = true;
          return;
        }
      }
      this._trackedEntityLabel.Text = string.Empty;
      ((Control) this._trackedEntityPanel).Visible = false;
    }
  }
}
