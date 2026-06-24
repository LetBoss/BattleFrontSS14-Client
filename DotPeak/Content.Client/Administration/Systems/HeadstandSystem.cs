// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.Systems.HeadstandSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Administration.Systems;

public sealed class HeadstandSystem : EntitySystem
{
  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HeadstandComponent, ComponentStartup>(new ComponentEventHandler<HeadstandComponent, ComponentStartup>((object) this, __methodptr(OnHeadstandAdded)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HeadstandComponent, ComponentShutdown>(new ComponentEventHandler<HeadstandComponent, ComponentShutdown>((object) this, __methodptr(OnHeadstandRemoved)), (Type[]) null, (Type[]) null);
  }

  private void OnHeadstandAdded(EntityUid uid, HeadstandComponent component, ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    foreach (ISpriteLayer allLayer in spriteComponent.AllLayers)
      allLayer.Rotation = Angle.op_Addition(allLayer.Rotation, Angle.FromDegrees(180.0));
  }

  private void OnHeadstandRemoved(
    EntityUid uid,
    HeadstandComponent component,
    ComponentShutdown args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    foreach (ISpriteLayer allLayer in spriteComponent.AllLayers)
      allLayer.Rotation = Angle.op_Subtraction(allLayer.Rotation, Angle.FromDegrees(180.0));
  }
}
