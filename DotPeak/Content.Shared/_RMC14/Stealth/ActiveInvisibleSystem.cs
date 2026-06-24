// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Stealth.ActiveInvisibleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Evasion;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Stealth;

public sealed class ActiveInvisibleSystem : EntitySystem
{
  [Dependency]
  private EvasionSystem _evasionSystem;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<EntityActiveInvisibleComponent, ComponentAdd>(new EntityEventRefHandler<EntityActiveInvisibleComponent, ComponentAdd>(this.OnInvisibleComponentAdd));
    this.SubscribeLocalEvent<EntityActiveInvisibleComponent, ComponentRemove>(new EntityEventRefHandler<EntityActiveInvisibleComponent, ComponentRemove>(this.OnInvisibleComponentRemove));
    this.SubscribeLocalEvent<EntityActiveInvisibleComponent, EvasionRefreshModifiersEvent>(new EntityEventRefHandler<EntityActiveInvisibleComponent, EvasionRefreshModifiersEvent>(this.OnInvisibleRefreshModifiers));
    this.SubscribeLocalEvent<EntityActiveInvisibleComponent, AttemptMobCollideEvent>(new EntityEventRefHandler<EntityActiveInvisibleComponent, AttemptMobCollideEvent>(this.OnAttemptMobCollide));
    this.SubscribeLocalEvent<EntityActiveInvisibleComponent, AttemptMobTargetCollideEvent>(new EntityEventRefHandler<EntityActiveInvisibleComponent, AttemptMobTargetCollideEvent>(this.OnAttemptMobTargetCollide));
  }

  private void OnInvisibleComponentAdd(
    Entity<EntityActiveInvisibleComponent> entity,
    ref ComponentAdd args)
  {
    this._evasionSystem.RefreshEvasionModifiers(entity.Owner);
  }

  private void OnInvisibleComponentRemove(
    Entity<EntityActiveInvisibleComponent> entity,
    ref ComponentRemove args)
  {
    this._evasionSystem.RefreshEvasionModifiers(entity.Owner);
  }

  private void OnInvisibleRefreshModifiers(
    Entity<EntityActiveInvisibleComponent> entity,
    ref EvasionRefreshModifiersEvent args)
  {
    if (entity.Owner != args.Entity.Owner)
      return;
    args.Evasion += entity.Comp.EvasionModifier;
    args.EvasionFriendly += entity.Comp.EvasionFriendlyModifier;
  }

  private void OnAttemptMobCollide(
    Entity<EntityActiveInvisibleComponent> ent,
    ref AttemptMobCollideEvent args)
  {
    if (!ent.Comp.DisableMobCollision)
      return;
    args.Cancelled = true;
  }

  private void OnAttemptMobTargetCollide(
    Entity<EntityActiveInvisibleComponent> ent,
    ref AttemptMobTargetCollideEvent args)
  {
    if (!ent.Comp.DisableMobCollision)
      return;
    args.Cancelled = true;
  }
}
