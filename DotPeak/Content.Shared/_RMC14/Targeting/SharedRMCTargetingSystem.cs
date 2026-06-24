// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Targeting.SharedRMCTargetingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Rangefinder;
using Content.Shared._RMC14.Rangefinder.Spotting;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Targeting;

public abstract class SharedRMCTargetingSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<TargetingComponent, ComponentRemove>(new EntityEventRefHandler<TargetingComponent, ComponentRemove>(this.OnTargetingRemoved<ComponentRemove>));
    this.SubscribeLocalEvent<TargetingComponent, DroppedEvent>(new EntityEventRefHandler<TargetingComponent, DroppedEvent>(this.OnTargetingDropped<DroppedEvent>));
    this.SubscribeLocalEvent<TargetingComponent, RMCDroppedEvent>(new EntityEventRefHandler<TargetingComponent, RMCDroppedEvent>(this.OnTargetingDropped<RMCDroppedEvent>));
    this.SubscribeLocalEvent<TargetingComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<TargetingComponent, GotUnequippedHandEvent>(this.OnTargetingDropped<GotUnequippedHandEvent>));
    this.SubscribeLocalEvent<TargetingComponent, HandDeselectedEvent>(new EntityEventRefHandler<TargetingComponent, HandDeselectedEvent>(this.OnTargetingDropped<HandDeselectedEvent>));
    this.SubscribeLocalEvent<RMCTargetedComponent, ComponentRemove>(new EntityEventRefHandler<RMCTargetedComponent, ComponentRemove>(this.OnTargetedRemove<ComponentRemove>));
    this.SubscribeLocalEvent<RMCTargetedComponent, EntityTerminatingEvent>(new EntityEventRefHandler<RMCTargetedComponent, EntityTerminatingEvent>(this.OnTargetedRemove<EntityTerminatingEvent>));
  }

  private void OnTargetingRemoved<T>(Entity<TargetingComponent> targeting, ref T args)
  {
    if (this._net.IsClient)
      return;
    RangefinderComponent comp;
    if (this.TryComp<RangefinderComponent>((EntityUid) targeting, out comp))
      this._appearance.SetData((EntityUid) targeting, (Enum) RangefinderLayers.Layer, (object) comp.Mode);
    while (targeting.Comp.Targets.Count > 0)
      this.StopTargeting((Entity<TargetingComponent>) ((EntityUid) targeting, (TargetingComponent) targeting), targeting.Comp.Targets[0]);
  }

  private void OnTargetingDropped<T>(Entity<TargetingComponent> targeting, ref T args)
  {
    TargetingCancelledEvent args1 = new TargetingCancelledEvent();
    this.RaiseLocalEvent<TargetingCancelledEvent>((EntityUid) targeting, ref args1);
    while (targeting.Comp.Targets.Count > 0)
      this.StopTargeting((Entity<TargetingComponent>) ((EntityUid) targeting, (TargetingComponent) targeting), targeting.Comp.Targets[0]);
  }

  private void OnTargetedRemove<T>(Entity<RMCTargetedComponent> ent, ref T args)
  {
    foreach (EntityUid uid in ent.Comp.TargetedBy)
    {
      TargetingComponent comp;
      if (this.TryComp<TargetingComponent>(uid, out comp))
      {
        comp.Targets.Remove((EntityUid) ent);
        comp.LaserDurations.Remove((EntityUid) ent);
        comp.OriginalLaserDurations.Remove((EntityUid) ent);
        this.Dirty(uid, (IComponent) comp);
      }
    }
  }

  public void StopTargeting(Entity<TargetingComponent?> targeting, EntityUid target)
  {
    if (!this.Resolve<TargetingComponent>((EntityUid) targeting, ref targeting.Comp, false))
      return;
    targeting.Comp.Targets.Remove(target);
    this.Dirty<TargetingComponent>(targeting);
    RMCTargetedComponent comp1;
    if (!this.TryComp<RMCTargetedComponent>(target, out comp1))
      return;
    comp1.TargetedBy.Remove((EntityUid) targeting);
    this.Dirty(target, (IComponent) comp1);
    TargetedEffects newMarker = TargetedEffects.None;
    DirectionTargetedEffects directionEffect = DirectionTargetedEffects.None;
    bool flag = false;
    foreach (EntityUid uid in comp1.TargetedBy)
    {
      TargetingComponent comp2;
      if (this.TryComp<TargetingComponent>(uid, out comp2))
      {
        if (comp2.LaserType > newMarker)
          newMarker = comp2.LaserType;
        if (comp2.DirectionEffect > directionEffect)
          directionEffect = comp2.DirectionEffect;
        if (comp2.LaserType == TargetedEffects.Spotted)
          flag = true;
      }
    }
    if (!flag)
      this.RemComp<SpottedComponent>(target);
    this.UpdateTargetMarker(target, newMarker, directionEffect, true);
    if (comp1.TargetedBy.Count != 0)
      return;
    this.RemComp<RMCTargetedComponent>(target);
    if (targeting.Comp.Targets.Count != 0)
      return;
    this.RemComp<TargetingComponent>((EntityUid) targeting);
  }

  public void Target(
    EntityUid equipment,
    EntityUid user,
    EntityUid target,
    float targetingDuration,
    TargetedEffects targetedEffect = TargetedEffects.None,
    DirectionTargetedEffects directionEffect = DirectionTargetedEffects.None)
  {
    TargetingStartedEvent args1 = new TargetingStartedEvent(directionEffect, targetedEffect, target);
    this.RaiseLocalEvent<TargetingStartedEvent>(equipment, ref args1);
    targetedEffect = args1.TargetedEffect;
    directionEffect = args1.DirectionEffect;
    RMCTargetedComponent targetedComponent = this.EnsureComp<RMCTargetedComponent>(target);
    targetedComponent.TargetedBy.Add(equipment);
    this.Dirty(target, (IComponent) targetedComponent);
    TargetingComponent targetingComponent = this.EnsureComp<TargetingComponent>(equipment);
    if (!targetingComponent.LaserDurations.TryAdd(target, new List<float>()
    {
      targetingDuration
    }))
      targetingComponent.LaserDurations[target].Add(targetingDuration);
    if (!targetingComponent.OriginalLaserDurations.TryAdd(target, new List<float>()
    {
      targetingDuration
    }))
      targetingComponent.OriginalLaserDurations[target].Add(targetingDuration);
    targetingComponent.Source = equipment;
    targetingComponent.Targets.Add(target);
    targetingComponent.Origin = this.Transform(user).Coordinates;
    targetingComponent.User = user;
    targetingComponent.LaserType = targetedEffect;
    targetingComponent.DirectionEffect = directionEffect;
    this.Dirty(equipment, (IComponent) targetingComponent);
    GotTargetedEvent args2 = new GotTargetedEvent();
    this.RaiseLocalEvent<GotTargetedEvent>(target, ref args2);
    this.UpdateTargetMarker(target, targetedEffect, directionEffect);
  }

  private void UpdateTargetMarker(
    EntityUid target,
    TargetedEffects newMarker,
    DirectionTargetedEffects directionEffect,
    bool force = false)
  {
    TargetedEffects targetedEffects;
    this._appearance.TryGetData<TargetedEffects>(target, (Enum) TargetedVisuals.Targeted, out targetedEffects);
    if (force || newMarker > targetedEffects)
      this._appearance.SetData(target, (Enum) TargetedVisuals.Targeted, (object) newMarker);
    bool flag1 = directionEffect > DirectionTargetedEffects.None;
    bool flag2 = directionEffect > DirectionTargetedEffects.DirectionTargeted;
    this._appearance.SetData(target, (Enum) TargetedVisuals.TargetedDirection, (object) (bool) (!flag1 ? 0 : (!flag2 ? 1 : 0)));
    this._appearance.SetData(target, (Enum) TargetedVisuals.TargetedDirectionIntense, (object) (flag1 & flag2));
  }

  private void RemoveLaser(Entity<TargetingComponent> ent, EntityUid target, int laserNumber)
  {
    ent.Comp.LaserDurations[target].RemoveAt(laserNumber);
    ent.Comp.OriginalLaserDurations[target].RemoveAt(laserNumber);
    if (ent.Comp.LaserDurations[target].Count <= 0)
    {
      ent.Comp.LaserDurations.Remove(target);
      ent.Comp.OriginalLaserDurations.Remove(target);
    }
    this.Dirty<TargetingComponent>(ent);
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<TargetingComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<TargetingComponent, TransformComponent>();
    EntityUid uid;
    TargetingComponent comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      int index1 = 0;
      List<EntityUid> entityUidList = new List<EntityUid>();
      while (index1 < comp1.Targets.Count)
      {
        EntityUid target = comp1.Targets[index1];
        if (!comp1.LaserDurations.Keys.Contains(target) || entityUidList.Contains(target))
        {
          ++index1;
        }
        else
        {
          entityUidList.Add(target);
          for (int index2 = 0; index2 < comp1.LaserDurations[target].Count; ++index2)
          {
            comp1.LaserDurations[target][index2] -= frameTime;
            this.Dirty(uid, (IComponent) comp1);
            RMCTargetedComponent comp3;
            if (this.TryComp<RMCTargetedComponent>(target, out comp3))
            {
              float num = (float) (1.0 - (double) comp1.LaserDurations[target][index2] / (double) comp1.OriginalLaserDurations[target][index2]);
              comp3.AlphaMultipliers.TryAdd(uid, num);
              if ((double) num > (double) comp3.AlphaMultipliers[uid])
                comp3.AlphaMultipliers[uid] = num;
            }
            if ((double) comp1.LaserDurations[target][index2] <= 0.0)
            {
              this.RemoveLaser((Entity<TargetingComponent>) (uid, comp1), target, index2);
              TransformComponent comp4;
              if (this.TryComp(target, out comp4))
              {
                TargetingFinishedEvent args = new TargetingFinishedEvent(comp1.User, comp4.Coordinates, target);
                this.RaiseLocalEvent<TargetingFinishedEvent>(uid, ref args);
                break;
              }
              break;
            }
          }
          ++index1;
        }
      }
      TransformComponent comp;
      if (this.TryComp(comp2.ParentUid, out comp) && !this._transform.InRange(comp.Coordinates, comp1.Origin, 0.1f))
      {
        TargetingCancelledEvent args = new TargetingCancelledEvent();
        this.RaiseLocalEvent<TargetingCancelledEvent>(uid, ref args);
        comp1.LaserDurations.Clear();
        comp1.OriginalLaserDurations.Clear();
        this.Dirty(uid, (IComponent) comp1);
        while (comp1.Targets.Count > 0)
          this.StopTargeting((Entity<TargetingComponent>) (uid, comp1), comp1.Targets[0]);
      }
    }
  }
}
