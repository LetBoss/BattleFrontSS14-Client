// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Movement.TemporarySpeedModifiersSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Slow;
using Content.Shared.Clothing;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Movement;

public sealed class TemporarySpeedModifiersSystem : EntitySystem
{
  private const float MaxSpeedModifier = 1f;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeedSystem;
  [Dependency]
  private INetManager _netManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedContainerSystem _container;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<TemporarySpeedModifiersComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<TemporarySpeedModifiersComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovement));
    this.SubscribeLocalEvent<ClothingSpeedModifierComponent, RMCMovementSpeedRefreshedEvent>(new EntityEventRefHandler<ClothingSpeedModifierComponent, RMCMovementSpeedRefreshedEvent>(this.OnRMCRefreshMovement));
  }

  private void OnRefreshMovement(
    Entity<TemporarySpeedModifiersComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    foreach ((TimeSpan ExpiresAt, float Walk, float Sprint) modifier in ent.Comp.Modifiers)
    {
      if (!(modifier.ExpiresAt <= this._timing.CurTime))
        args.ModifySpeed(modifier.Walk, modifier.Sprint);
    }
  }

  private void OnRMCRefreshMovement(
    Entity<ClothingSpeedModifierComponent> ent,
    ref RMCMovementSpeedRefreshedEvent args)
  {
    BaseContainer container;
    MovementSpeedModifierComponent comp;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null, (MetaDataComponent) null), out container) || !this.TryComp<MovementSpeedModifierComponent>(container.Owner, out comp) || !this.HasComp<RMCSlowdownComponent>(container.Owner) && !this.HasComp<RMCSuperSlowdownComponent>(container.Owner))
      return;
    float num = (float) (1.0 - (1.0 - (double) args.SprintModifier) * ((double) comp.CurrentSprintSpeed / (double) comp.BaseSprintSpeed));
    args.WalkModifier = num;
    args.SprintModifier = num;
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<TemporarySpeedModifiersComponent> entityQueryEnumerator = this.EntityQueryEnumerator<TemporarySpeedModifiersComponent>();
    EntityUid uid;
    TemporarySpeedModifiersComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      List<(TimeSpan, float, float)> valueTupleList = new List<(TimeSpan, float, float)>();
      foreach ((TimeSpan ExpiresAt, float Walk, float Sprint) modifier in comp1.Modifiers)
      {
        if (!(modifier.ExpiresAt > curTime))
          valueTupleList.Add(modifier);
      }
      foreach ((TimeSpan, float, float) valueTuple in valueTupleList)
        comp1.Modifiers.Remove(valueTuple);
      if (valueTupleList.Count > 0)
        this.Dirty(uid, (IComponent) comp1);
      if (comp1.Modifiers.Count <= 0)
        this.RemCompDeferred<TemporarySpeedModifiersComponent>(uid);
      this._movementSpeedSystem.RefreshMovementSpeedModifiers(uid);
    }
  }

  public void ModifySpeed(EntityUid entUid, List<TemporarySpeedModifierSet> modifiers)
  {
    if (this._netManager.IsClient)
      return;
    TemporarySpeedModifiersComponent modifiersComponent = this.EnsureComp<TemporarySpeedModifiersComponent>(entUid);
    foreach (TemporarySpeedModifierSet modifier in modifiers)
      modifiersComponent.Modifiers.Add((this._timing.CurTime + modifier.Duration, modifier.Walk, modifier.Sprint));
  }

  public float? CalculateSpeedModifier(
    EntityUid uid,
    float modifier,
    MovementSpeedModifierComponent? movement = null)
  {
    if (!this.Resolve<MovementSpeedModifierComponent>(uid, ref movement) || (double) movement.CurrentSprintSpeed == 0.0)
      return new float?();
    float currentSprintSpeed = movement.CurrentSprintSpeed;
    float baseSprintSpeed = movement.BaseSprintSpeed;
    return new float?(Math.Min((float) (1.0 / ((double) Math.Max((float) (1.0 / (double) currentSprintSpeed * 10.0) + modifier, 1f) / 10.0)) / movement.CurrentSprintSpeed, (float) (1.0 / ((double) Math.Max((float) (1.0 / (double) baseSprintSpeed * 10.0) + modifier, 1f) / 10.0)) / movement.BaseSprintSpeed));
  }
}
