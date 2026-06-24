// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.SharedRgbLightControllerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Light.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Light;

public abstract class SharedRgbLightControllerSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RgbLightControllerComponent, ComponentGetState>(new ComponentEventRefHandler<RgbLightControllerComponent, ComponentGetState>(this.OnGetState));
  }

  private void OnGetState(
    EntityUid uid,
    RgbLightControllerComponent component,
    ref ComponentGetState args)
  {
    args.State = (IComponentState) new RgbLightControllerState(component.CycleRate, component.Layers);
  }

  public void SetLayers(EntityUid uid, List<int>? layers, RgbLightControllerComponent? rgb = null)
  {
    if (!this.Resolve<RgbLightControllerComponent>(uid, ref rgb))
      return;
    rgb.Layers = layers;
    this.Dirty(uid, (IComponent) rgb);
  }

  public void SetCycleRate(EntityUid uid, float rate, RgbLightControllerComponent? rgb = null)
  {
    if (!this.Resolve<RgbLightControllerComponent>(uid, ref rgb))
      return;
    rgb.CycleRate = Math.Clamp(0.01f, rate, 1f);
    this.Dirty(uid, (IComponent) rgb);
  }
}
