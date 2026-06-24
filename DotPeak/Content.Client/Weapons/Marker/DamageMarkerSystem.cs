// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Marker.DamageMarkerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Weapons.Marker;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.Weapons.Marker;

public sealed class DamageMarkerSystem : SharedDamageMarkerSystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageMarkerComponent, ComponentStartup>(new ComponentEventHandler<DamageMarkerComponent, ComponentStartup>((object) this, __methodptr(OnMarkerStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageMarkerComponent, ComponentShutdown>(new ComponentEventHandler<DamageMarkerComponent, ComponentShutdown>((object) this, __methodptr(OnMarkerShutdown)), (Type[]) null, (Type[]) null);
  }

  private void OnMarkerStartup(
    EntityUid uid,
    DamageMarkerComponent component,
    ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    if (!this._timing.ApplyingState || component.Effect == null || !this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    int num = this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) DamageMarkerSystem.DamageMarkerKey.Key);
    this._sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num, component.Effect.RsiPath, new RSI.StateId?(RSI.StateId.op_Implicit(component.Effect.RsiState)));
  }

  private void OnMarkerShutdown(
    EntityUid uid,
    DamageMarkerComponent component,
    ComponentShutdown args)
  {
    SpriteComponent spriteComponent;
    int num;
    if (!this._timing.ApplyingState || !this.TryComp<SpriteComponent>(uid, ref spriteComponent) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) DamageMarkerSystem.DamageMarkerKey.Key, ref num, false))
      return;
    this._sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num, true);
  }

  private enum DamageMarkerKey : byte
  {
    Key,
  }
}
