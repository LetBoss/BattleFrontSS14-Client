// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Vehicle.VehicleHardpointVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Vehicle;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Vehicle;

public sealed class VehicleHardpointVisualizerSystem : EntitySystem
{
  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<VehicleHardpointVisualsComponent, ComponentStartup>(new ComponentEventRefHandler<VehicleHardpointVisualsComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<VehicleHardpointVisualsComponent, ComponentHandleState>(new ComponentEventRefHandler<VehicleHardpointVisualsComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnStartup(
    EntityUid uid,
    VehicleHardpointVisualsComponent component,
    ref ComponentStartup args)
  {
    this.ApplyLayers(uid, component);
  }

  private void OnHandleState(
    EntityUid uid,
    VehicleHardpointVisualsComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is VehicleHardpointVisualsComponentState current))
      return;
    component.Layers = new List<VehicleHardpointLayerState>((IEnumerable<VehicleHardpointLayerState>) current.Layers);
    this.ApplyLayers(uid, component);
  }

  private void ApplyLayers(EntityUid uid, VehicleHardpointVisualsComponent component)
  {
    SpriteComponent sprite;
    if (!this.TryComp<SpriteComponent>(uid, ref sprite))
      return;
    foreach (VehicleHardpointLayerState layer in component.Layers)
      this.UpdateLayer(sprite, layer.Layer, layer.State);
  }

  private void UpdateLayer(SpriteComponent sprite, string layerMap, string state)
  {
    int num;
    if (!sprite.LayerMapTryGet((object) layerMap, ref num, false))
      return;
    if (string.IsNullOrWhiteSpace(state))
    {
      sprite.LayerSetVisible(num, false);
    }
    else
    {
      sprite.LayerSetState(num, RSI.StateId.op_Implicit(state));
      sprite.LayerSetVisible(num, true);
    }
  }
}
