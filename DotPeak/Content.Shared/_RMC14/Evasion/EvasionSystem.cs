// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Evasion.EvasionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.FixedPoint;
using Content.Shared.Standing;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared._RMC14.Evasion;

public sealed class EvasionSystem : EntitySystem
{
  public override void Initialize()
  {
    this.SubscribeLocalEvent<EvasionComponent, MapInitEvent>(new EntityEventRefHandler<EvasionComponent, MapInitEvent>(this.CallRefresh<MapInitEvent>));
    this.SubscribeLocalEvent<EvasionComponent, XenoRestEvent>(new EntityEventRefHandler<EvasionComponent, XenoRestEvent>(this.CallRefresh<XenoRestEvent>));
    this.SubscribeLocalEvent<EvasionComponent, DownedEvent>(new EntityEventRefHandler<EvasionComponent, DownedEvent>(this.CallRefresh<DownedEvent>));
    this.SubscribeLocalEvent<EvasionComponent, StoodEvent>(new EntityEventRefHandler<EvasionComponent, StoodEvent>(this.CallRefresh<StoodEvent>));
    this.SubscribeLocalEvent<RMCSizeComponent, EvasionRefreshModifiersEvent>(new EntityEventRefHandler<RMCSizeComponent, EvasionRefreshModifiersEvent>(this.OnSizeRefreshEvasion));
  }

  public void RefreshEvasionModifiers(EntityUid entity)
  {
    EvasionComponent comp;
    if (!this.TryComp<EvasionComponent>(entity, out comp))
      return;
    this.RefreshEvasionModifiers((Entity<EvasionComponent>) (entity, comp));
  }

  public void RefreshEvasionModifiers(Entity<EvasionComponent> entity)
  {
    EvasionRefreshModifiersEvent args = new EvasionRefreshModifiersEvent(entity, entity.Comp.Evasion, entity.Comp.EvasionFriendly);
    this.RaiseLocalEvent<EvasionRefreshModifiersEvent>(entity.Owner, ref args);
    entity.Comp.ModifiedEvasion = args.Evasion;
    entity.Comp.ModifiedEvasionFriendly = args.EvasionFriendly;
    this.Dirty<EvasionComponent>(entity);
  }

  private void CallRefresh<T>(Entity<EvasionComponent> entity, ref T args) where T : notnull
  {
    this.RefreshEvasionModifiers(entity);
  }

  private void OnSizeRefreshEvasion(
    Entity<RMCSizeComponent> size,
    ref EvasionRefreshModifiersEvent args)
  {
    if (size.Owner != args.Entity.Owner)
      return;
    if (size.Comp.Size <= RMCSizes.Small)
      args.Evasion += (FixedPoint2) 10;
    if (size.Comp.Size < RMCSizes.Big)
      return;
    args.Evasion += (FixedPoint2) -10;
  }
}
