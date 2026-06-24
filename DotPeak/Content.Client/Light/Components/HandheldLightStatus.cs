// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.Components.HandheldLightStatus
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Light.Components;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System.Numerics;

#nullable enable
namespace Content.Client.Light.Components;

public sealed class HandheldLightStatus : Control
{
  private const float TimerCycle = 1f;
  private readonly HandheldLightComponent _parent;
  private readonly PanelContainer[] _sections = new PanelContainer[5];
  private float _timer;
  private static readonly StyleBoxFlat StyleBoxLit = new StyleBoxFlat()
  {
    BackgroundColor = Color.LimeGreen
  };
  private static readonly StyleBoxFlat StyleBoxUnlit = new StyleBoxFlat()
  {
    BackgroundColor = Color.Black
  };

  public HandheldLightStatus(HandheldLightComponent parent)
  {
    this._parent = parent;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer1.SeparationOverride = new int?(4);
    ((Control) boxContainer1).HorizontalAlignment = (Control.HAlignment) 2;
    BoxContainer boxContainer2 = boxContainer1;
    this.AddChild((Control) boxContainer2);
    for (int index = 0; index < this._sections.Length; ++index)
    {
      PanelContainer panelContainer1 = new PanelContainer();
      ((Control) panelContainer1).MinSize = new Vector2(20f, 20f);
      PanelContainer panelContainer2 = panelContainer1;
      ((Control) boxContainer2).AddChild((Control) panelContainer2);
      this._sections[index] = panelContainer2;
    }
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    this._timer += ((FrameEventArgs) ref args).DeltaSeconds;
    this._timer %= 1f;
    byte? level = this._parent.Level;
    byte? nullable1;
    int? nullable2;
    for (int index = 0; index < this._sections.Length; ++index)
    {
      if (index == 0)
      {
        nullable1 = level;
        nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
        int num = 0;
        if (nullable2.GetValueOrDefault() == num & nullable2.HasValue || !level.HasValue)
        {
          this._sections[0].PanelOverride = (StyleBox) HandheldLightStatus.StyleBoxUnlit;
        }
        else
        {
          nullable1 = level;
          int? nullable3;
          if (!nullable1.HasValue)
          {
            nullable2 = new int?();
            nullable3 = nullable2;
          }
          else
            nullable3 = new int?((int) nullable1.GetValueOrDefault());
          nullable2 = nullable3;
          this._sections[0].PanelOverride = nullable2.GetValueOrDefault() != 1 ? (StyleBox) HandheldLightStatus.StyleBoxLit : ((double) this._timer > 0.5 ? (StyleBox) HandheldLightStatus.StyleBoxLit : (StyleBox) HandheldLightStatus.StyleBoxUnlit);
        }
      }
      else
      {
        PanelContainer section = this._sections[index];
        nullable1 = level;
        nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
        int num = index + 2;
        StyleBoxFlat styleBoxFlat = nullable2.GetValueOrDefault() >= num & nullable2.HasValue ? HandheldLightStatus.StyleBoxLit : HandheldLightStatus.StyleBoxUnlit;
        section.PanelOverride = (StyleBox) styleBoxFlat;
      }
    }
  }
}
