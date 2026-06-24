// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Interaction.RMCInteractionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Light.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Shared._RMC14.Interaction;

public sealed class RMCInteractionSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private EntityWhitelistSystem _whitelist;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<InteractedBlacklistComponent, GettingInteractedWithAttemptEvent>(new EntityEventRefHandler<InteractedBlacklistComponent, GettingInteractedWithAttemptEvent>(this.OnBlacklistInteractionAttempt));
    this.SubscribeLocalEvent<NoHandsInteractionBlockedComponent, GettingInteractedWithAttemptEvent>(new EntityEventRefHandler<NoHandsInteractionBlockedComponent, GettingInteractedWithAttemptEvent>(this.OnNoHandsInteractionAttempt));
    this.SubscribeLocalEvent<InsertBlacklistComponent, ContainerIsInsertingAttemptEvent>(new EntityEventRefHandler<InsertBlacklistComponent, ContainerIsInsertingAttemptEvent>(this.OnInsertBlacklistContainerIsInsertingAttempt));
    this.SubscribeLocalEvent<IgnoreInteractionRangeComponent, InRangeOverrideEvent>(new EntityEventRefHandler<IgnoreInteractionRangeComponent, InRangeOverrideEvent>(this.OnInRangeOverride));
  }

  private void OnNoHandsInteractionAttempt(
    Entity<NoHandsInteractionBlockedComponent> ent,
    ref GettingInteractedWithAttemptEvent args)
  {
    if (args.Cancelled || this.HasComp<HandsComponent>(args.Uid))
      return;
    args.Cancelled = true;
  }

  private void OnBlacklistInteractionAttempt(
    Entity<InteractedBlacklistComponent> ent,
    ref GettingInteractedWithAttemptEvent args)
  {
    TransformComponent comp1;
    HandheldLightComponent comp2;
    if (args.Cancelled || ent.Comp.Blacklist == null || !this.TryComp((EntityUid) ent, out comp1) || ent.Comp.AnchoredOnly && !comp1.Anchored || this.TryComp<HandheldLightComponent>((EntityUid) ent, out comp2) && comp2.Activated || !this._whitelist.IsValid(ent.Comp.Blacklist, args.Uid))
      return;
    args.Cancelled = true;
  }

  private void OnInsertBlacklistContainerIsInsertingAttempt(
    Entity<InsertBlacklistComponent> ent,
    ref ContainerIsInsertingAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    InsertBlacklistComponent comp1 = ent.Comp;
    MobStateComponent comp2;
    if (comp1.Blacklist != null && this._whitelist.IsValid(comp1.Blacklist, args.EntityUid) || comp1.BlacklistedMobStates != null && this.TryComp<MobStateComponent>(args.EntityUid, out comp2) && comp1.BlacklistedMobStates.Contains(comp2.CurrentState))
    {
      args.Cancel();
    }
    else
    {
      MobStateComponent comp3;
      if ((comp1.Whitelist == null || this._whitelist.IsValid(comp1.Whitelist, args.EntityUid)) && (comp1.WhitelistedMobStates == null || !this.TryComp<MobStateComponent>(args.EntityUid, out comp3) || comp1.WhitelistedMobStates.Contains(comp3.CurrentState)))
        return;
      args.Cancel();
    }
  }

  private void OnInRangeOverride(
    Entity<IgnoreInteractionRangeComponent> ent,
    ref InRangeOverrideEvent args)
  {
    if (!this._whitelist.IsWhitelistPassOrNull(ent.Comp.Whitelist, args.Target) || !this._transform.InRange((Entity<TransformComponent>) args.User, (Entity<TransformComponent>) args.Target, ent.Comp.Range))
      return;
    args.InRange = true;
    args.Handled = true;
  }

  public void TryCapWorldRotation(
    Entity<MaxRotationComponent?, TransformComponent?> max,
    ref Angle angle)
  {
    if (!this.Resolve<MaxRotationComponent, TransformComponent>((EntityUid) max, ref max.Comp1, ref max.Comp2, false))
      return;
    Angle set = max.Comp1.Set;
    Angle deviation = max.Comp1.Deviation;
    if (Angle.op_Implicit(Angle.ShortestDistance(ref angle, ref set)) > Angle.op_Implicit(deviation))
      angle = Angle.op_Addition(set, deviation);
    if (Angle.op_Implicit(Angle.ShortestDistance(ref angle, ref set)) >= Angle.op_Implicit(Angle.op_UnaryNegation(deviation)))
      return;
    angle = Angle.op_Subtraction(set, deviation);
  }

  public bool CanFaceMaxRotation(
    Entity<MaxRotationComponent?, TransformComponent?> max,
    Angle angle)
  {
    if (!this.Resolve<MaxRotationComponent, TransformComponent>((EntityUid) max, ref max.Comp1, ref max.Comp2, false))
      return true;
    Angle set = max.Comp1.Set;
    Angle deviation = max.Comp1.Deviation;
    return Angle.op_Implicit(Angle.ShortestDistance(ref angle, ref set)) <= Angle.op_Implicit(deviation) && Angle.op_Implicit(Angle.ShortestDistance(ref angle, ref set)) >= Angle.op_Implicit(Angle.op_UnaryNegation(deviation));
  }

  public void SetMaxRotation(Entity<MaxRotationComponent?> ent, Angle set, Angle deviation)
  {
    ref MaxRotationComponent local = ref ent.Comp;
    if (local == null)
      local = this.EnsureComp<MaxRotationComponent>((EntityUid) ent);
    ent.Comp.Set = set;
    ent.Comp.Deviation = deviation;
    this.Dirty<MaxRotationComponent>(ent);
  }
}
