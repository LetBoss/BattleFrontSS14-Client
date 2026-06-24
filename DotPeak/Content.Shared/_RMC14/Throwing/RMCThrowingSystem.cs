// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Throwing.RMCThrowingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Components;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Throwing;

public sealed class RMCThrowingSystem : EntitySystem
{
  [Dependency]
  private ThrownItemSystem _thrown;
  private Robust.Shared.GameObjects.EntityQuery<ThrownItemComponent> _thrownItemQuery;

  public override void Initialize()
  {
    this._thrownItemQuery = this.GetEntityQuery<ThrownItemComponent>();
    this.SubscribeLocalEvent<DamageOtherOnHitComponent, ThrownEvent>(new EntityEventRefHandler<DamageOtherOnHitComponent, ThrownEvent>(this.OnDamageOtherOnHitThrown));
    this.SubscribeLocalEvent<ThrownLimitHitsComponent, ThrowDoHitEvent>(new EntityEventRefHandler<ThrownLimitHitsComponent, ThrowDoHitEvent>(this.OnThrownLimitHitsDoHit));
    this.SubscribeLocalEvent<ThrownLimitHitsComponent, LandEvent>(new EntityEventRefHandler<ThrownLimitHitsComponent, LandEvent>(this.OnThrownLimitHitsLand));
    this.SubscribeLocalEvent<ThrownLimitHitsComponent, StopThrowEvent>(new EntityEventRefHandler<ThrownLimitHitsComponent, StopThrowEvent>(this.OnThrownLimitHitsStopThrow));
  }

  private void OnDamageOtherOnHitThrown(Entity<DamageOtherOnHitComponent> ent, ref ThrownEvent args)
  {
    ThrownLimitHitsComponent limitHitsComponent = this.EnsureComp<ThrownLimitHitsComponent>((EntityUid) ent);
    limitHitsComponent.Hit = false;
    this.Dirty((EntityUid) ent, (IComponent) limitHitsComponent);
  }

  private void OnThrownLimitHitsLand(Entity<ThrownLimitHitsComponent> ent, ref LandEvent args)
  {
    ent.Comp.Hit = false;
    this.Dirty<ThrownLimitHitsComponent>(ent);
  }

  private void OnThrownLimitHitsDoHit(
    Entity<ThrownLimitHitsComponent> ent,
    ref ThrowDoHitEvent args)
  {
    ent.Comp.Hit = true;
    this.Dirty<ThrownLimitHitsComponent>(ent);
    ThrownItemComponent component;
    if (!this._thrownItemQuery.TryComp((EntityUid) ent, out component))
      return;
    this._thrown.StopThrow((EntityUid) ent, component);
  }

  private void OnThrownLimitHitsStopThrow(
    Entity<ThrownLimitHitsComponent> ent,
    ref StopThrowEvent args)
  {
    this.RemCompDeferred<ThrownLimitHitsComponent>((EntityUid) ent);
  }
}
