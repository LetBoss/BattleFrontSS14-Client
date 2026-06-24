// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Buckle.RMCBuckleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CrashLand;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Interaction;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Shuttles.Components;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Buckle;

public sealed class RMCBuckleSystem : EntitySystem
{
  [Dependency]
  private SharedBuckleSystem _buckle;
  [Dependency]
  private SharedCrashLandSystem _crashLand;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private SharedPopupSystem _popup;
  private readonly HashSet<EntityUid> _intersecting = new HashSet<EntityUid>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<BuckleClimbableComponent, StrappedEvent>(new EntityEventRefHandler<BuckleClimbableComponent, StrappedEvent>(this.OnBuckleClimbableStrapped));
    this.SubscribeLocalEvent<ActiveBuckleClimbingComponent, PreventCollideEvent>(new EntityEventRefHandler<ActiveBuckleClimbingComponent, PreventCollideEvent>(this.OnBuckleClimbablePreventCollide));
    this.SubscribeLocalEvent<BuckleWhitelistComponent, BuckleAttemptEvent>(new EntityEventRefHandler<BuckleWhitelistComponent, BuckleAttemptEvent>(this.OnBuckleWhitelistAttempt));
    this.SubscribeLocalEvent<BuckleComponent, AttemptMobTargetCollideEvent>(new EntityEventRefHandler<BuckleComponent, AttemptMobTargetCollideEvent>(this.OnBuckleAttemptMobTargetCollide));
    this.SubscribeLocalEvent<StrapComponent, EntParentChangedMessage>(new EntityEventRefHandler<StrapComponent, EntParentChangedMessage>(this.OnBuckleParentChanged));
    this.SubscribeLocalEvent<StrapComponent, CombatModeShouldHandInteractEvent>(new EntityEventRefHandler<StrapComponent, CombatModeShouldHandInteractEvent>(this.OnStrapCombatModeShouldHandInteract));
  }

  private void OnBuckleClimbableStrapped(
    Entity<BuckleClimbableComponent> ent,
    ref StrappedEvent args)
  {
    ActiveBuckleClimbingComponent climbingComponent = this.EnsureComp<ActiveBuckleClimbingComponent>((EntityUid) args.Buckle);
    climbingComponent.Strap = new EntityUid?((EntityUid) ent);
    this.Dirty((EntityUid) args.Buckle, (IComponent) climbingComponent);
  }

  private void OnBuckleClimbablePreventCollide(
    Entity<ActiveBuckleClimbingComponent> ent,
    ref PreventCollideEvent args)
  {
    if (args.Cancelled)
      return;
    EntityUid? strap = ent.Comp.Strap;
    EntityUid otherEntity = args.OtherEntity;
    if ((strap.HasValue ? (strap.GetValueOrDefault() == otherEntity ? 1 : 0) : 0) == 0)
      return;
    args.Cancelled = true;
  }

  private void OnBuckleWhitelistAttempt(
    Entity<BuckleWhitelistComponent> ent,
    ref BuckleAttemptEvent args)
  {
    if (this._entityWhitelist.IsWhitelistPassOrNull(ent.Comp.Whitelist, (EntityUid) args.Strap))
      return;
    args.Cancelled = true;
  }

  private void OnBuckleAttemptMobTargetCollide(
    Entity<BuckleComponent> ent,
    ref AttemptMobTargetCollideEvent args)
  {
    if (args.Cancelled || !ent.Comp.Buckled)
      return;
    args.Cancelled = true;
  }

  private void OnBuckleParentChanged(Entity<StrapComponent> ent, ref EntParentChangedMessage args)
  {
    if (!this.HasComp<FTLMapComponent>(args.Transform.ParentUid) || !args.OldParent.HasValue)
      return;
    foreach (EntityUid buckledEntity in ent.Comp.BuckledEntities)
    {
      this._buckle.TryUnbuckle((Entity<BuckleComponent>) buckledEntity, new EntityUid?(buckledEntity), false);
      AttemptCrashLandEvent args1 = new AttemptCrashLandEvent(buckledEntity);
      this.RaiseLocalEvent<AttemptCrashLandEvent>(args.OldParent.Value, ref args1);
      if (!args1.Cancelled)
        this._crashLand.TryCrashLand((Entity<CrashLandableComponent>) buckledEntity, true);
    }
  }

  private void OnStrapCombatModeShouldHandInteract(
    Entity<StrapComponent> ent,
    ref CombatModeShouldHandInteractEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.User))
      return;
    args.Cancelled = true;
  }

  public Vector2 GetOffset(Entity<RMCBuckleOffsetComponent?> offset)
  {
    return !this.Resolve<RMCBuckleOffsetComponent>((EntityUid) offset, ref offset.Comp, false) ? Vector2.Zero : offset.Comp.Offset;
  }

  public bool CanBuckle(EntityUid? user, EntityUid buckle, bool popup = true)
  {
    if (!this.HasComp<XenoComponent>(user))
      return true;
    if (popup)
      this._popup.PopupPredicted("You don't have the dexterity to do that, try a nest.", buckle, new EntityUid?(user.Value), PopupType.SmallCaution);
    return false;
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveBuckleClimbingComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveBuckleClimbingComponent>();
    EntityUid uid;
    ActiveBuckleClimbingComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntityUid? strap = comp1.Strap;
      if (strap.HasValue)
      {
        EntityUid valueOrDefault = strap.GetValueOrDefault();
        this._intersecting.Clear();
        this._entityLookup.GetEntitiesIntersecting(uid, this._intersecting);
        if (!this._intersecting.Contains(valueOrDefault))
          this.RemCompDeferred<ActiveBuckleClimbingComponent>(uid);
      }
      else
        this.RemCompDeferred<ActiveBuckleClimbingComponent>(uid);
    }
  }
}
