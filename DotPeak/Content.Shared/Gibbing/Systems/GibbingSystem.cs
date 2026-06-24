// Decompiled with JetBrains decompiler
// Type: Content.Shared.Gibbing.Systems.GibbingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Gibbing.Components;
using Content.Shared.Gibbing.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Random;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared.Gibbing.Systems;

public sealed class GibbingSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _containerSystem;
  [Dependency]
  private SharedTransformSystem _transformSystem;
  [Dependency]
  private SharedAudioSystem _audioSystem;
  [Dependency]
  private SharedPhysicsSystem _physicsSystem;
  [Dependency]
  private IRobustRandom _random;

  public bool TryGibEntity(
    Entity<TransformComponent?> outerEntity,
    Entity<GibbableComponent?> gibbable,
    GibType gibType,
    GibContentsOption gibContentsOption,
    out HashSet<EntityUid> droppedEntities,
    bool launchGibs = true,
    Vector2 launchDirection = default (Vector2),
    float launchImpulse = 0.0f,
    float launchImpulseVariance = 0.0f,
    Angle launchCone = default (Angle),
    float randomSpreadMod = 1f,
    bool playAudio = true,
    List<string>? allowedContainers = null,
    List<string>? excludedContainers = null,
    bool logMissingGibable = false)
  {
    droppedEntities = new HashSet<EntityUid>();
    return this.TryGibEntityWithRef(outerEntity, gibbable, gibType, gibContentsOption, ref droppedEntities, launchGibs, new Vector2?(launchDirection), launchImpulse, launchImpulseVariance, launchCone, randomSpreadMod, playAudio, allowedContainers, excludedContainers, logMissingGibable);
  }

  public bool TryGibEntityWithRef(
    Entity<TransformComponent?> outerEntity,
    Entity<GibbableComponent?> gibbable,
    GibType gibType,
    GibContentsOption gibContentsOption,
    ref HashSet<EntityUid> droppedEntities,
    bool launchGibs = true,
    Vector2? launchDirection = null,
    float launchImpulse = 0.0f,
    float launchImpulseVariance = 0.0f,
    Angle launchCone = default (Angle),
    float randomSpreadMod = 1f,
    bool playAudio = true,
    List<string>? allowedContainers = null,
    List<string>? excludedContainers = null,
    bool logMissingGibable = false)
  {
    if (!this.Resolve<GibbableComponent>((EntityUid) gibbable, ref gibbable.Comp, false))
    {
      this.DropEntity(gibbable, (Entity<TransformComponent>) ((EntityUid) outerEntity, this.Transform((EntityUid) outerEntity)), randomSpreadMod, ref droppedEntities, launchGibs, launchDirection, launchImpulse, launchImpulseVariance, launchCone);
      if (logMissingGibable)
        this.Log.Warning($"{this.ToPrettyString(new EntityUid?((EntityUid) gibbable))} does not have a GibbableComponent! This is not required but may cause issues contained items to not be dropped.");
      return false;
    }
    if (gibType == GibType.Skip && gibContentsOption == GibContentsOption.Skip)
      return true;
    if (launchGibs)
      randomSpreadMod = 0.0f;
    HashSet<BaseContainer> baseContainerSet = new HashSet<BaseContainer>();
    AttemptEntityContentsGibEvent args = new AttemptEntityContentsGibEvent((EntityUid) gibbable, gibContentsOption, allowedContainers, excludedContainers);
    this.RaiseLocalEvent<AttemptEntityContentsGibEvent>((EntityUid) gibbable, ref args);
    foreach (BaseContainer allContainer in this._containerSystem.GetAllContainers((EntityUid) gibbable))
    {
      bool flag = true;
      if (allowedContainers != null)
        flag = allowedContainers.Contains(allContainer.ID);
      if (excludedContainers != null)
        flag = flag && !excludedContainers.Contains(allContainer.ID);
      if (flag)
        baseContainerSet.Add(allContainer);
    }
    switch (gibContentsOption)
    {
      case GibContentsOption.Drop:
        using (HashSet<BaseContainer>.Enumerator enumerator = baseContainerSet.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) enumerator.Current.ContainedEntities)
              this.DropEntity(new Entity<GibbableComponent>(containedEntity, (GibbableComponent) null), outerEntity, randomSpreadMod, ref droppedEntities, launchGibs, launchDirection, launchImpulse, launchImpulseVariance, launchCone);
          }
          break;
        }
      case GibContentsOption.Gib:
        using (HashSet<BaseContainer>.Enumerator enumerator = baseContainerSet.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) enumerator.Current.ContainedEntities)
              this.GibEntity(new Entity<GibbableComponent>(containedEntity, (GibbableComponent) null), outerEntity, randomSpreadMod, ref droppedEntities, launchGibs, launchDirection, launchImpulse, launchImpulseVariance, launchCone);
          }
          break;
        }
    }
    switch (gibType)
    {
      case GibType.Drop:
        this.DropEntity(gibbable, outerEntity, randomSpreadMod, ref droppedEntities, launchGibs, launchDirection, launchImpulse, launchImpulseVariance, launchCone);
        break;
      case GibType.Gib:
        this.GibEntity(gibbable, outerEntity, randomSpreadMod, ref droppedEntities, launchGibs, launchDirection, launchImpulse, launchImpulseVariance, launchCone);
        break;
    }
    if (playAudio)
      this._audioSystem.PlayPredicted(gibbable.Comp.GibSound, (EntityUid) outerEntity, new EntityUid?());
    if (gibType == GibType.Gib)
      this.PredictedQueueDel(gibbable.Owner);
    return true;
  }

  private void DropEntity(
    Entity<GibbableComponent?> gibbable,
    Entity<TransformComponent?> parent,
    float randomSpreadMod,
    ref HashSet<EntityUid> droppedEntities,
    bool flingEntity,
    Vector2? scatterDirection,
    float scatterImpulse,
    float scatterImpulseVariance,
    Angle scatterCone)
  {
    int GibletCount = 0;
    if (this.Resolve<GibbableComponent>((EntityUid) gibbable, ref gibbable.Comp, false))
      GibletCount = gibbable.Comp.GibCount;
    if (!this.Resolve((EntityUid) parent, ref parent.Comp, false))
      return;
    AttemptEntityGibEvent args1 = new AttemptEntityGibEvent((EntityUid) gibbable, GibletCount, GibType.Drop);
    this.RaiseLocalEvent<AttemptEntityGibEvent>((EntityUid) gibbable, ref args1);
    switch (args1.GibType)
    {
      case GibType.Skip:
        break;
      case GibType.Gib:
        this.GibEntity(gibbable, parent, randomSpreadMod, ref droppedEntities, flingEntity, scatterDirection, scatterImpulse, scatterImpulseVariance, scatterCone, false);
        break;
      default:
        this._transformSystem.DropNextTo((Entity<TransformComponent>) gibbable.Owner, parent);
        this._transformSystem.SetWorldRotation((EntityUid) gibbable, this._random.NextAngle());
        droppedEntities.Add((EntityUid) gibbable);
        if (flingEntity)
          this.FlingDroppedEntity((EntityUid) gibbable, scatterDirection, scatterImpulse, scatterImpulseVariance, scatterCone);
        EntityGibbedEvent args2 = new EntityGibbedEvent((EntityUid) gibbable, new List<EntityUid>()
        {
          (EntityUid) gibbable
        });
        this.RaiseLocalEvent<EntityGibbedEvent>((EntityUid) gibbable, ref args2);
        break;
    }
  }

  private List<EntityUid> GibEntity(
    Entity<GibbableComponent?> gibbable,
    Entity<TransformComponent?> parent,
    float randomSpreadMod,
    ref HashSet<EntityUid> droppedEntities,
    bool flingEntity,
    Vector2? scatterDirection,
    float scatterImpulse,
    float scatterImpulseVariance,
    Angle scatterCone,
    bool deleteTarget = true)
  {
    List<EntityUid> DroppedEntities = new List<EntityUid>();
    int GibletCount = 0;
    int num = 0;
    if (this.Resolve<GibbableComponent>((EntityUid) gibbable, ref gibbable.Comp, false))
    {
      GibletCount = gibbable.Comp.GibCount;
      num = gibbable.Comp.GibPrototypes.Count;
    }
    if (!this.Resolve((EntityUid) parent, ref parent.Comp, false))
      return new List<EntityUid>();
    AttemptEntityGibEvent args1 = new AttemptEntityGibEvent((EntityUid) gibbable, GibletCount, GibType.Drop);
    this.RaiseLocalEvent<AttemptEntityGibEvent>((EntityUid) gibbable, ref args1);
    switch (args1.GibType)
    {
      case GibType.Skip:
        return DroppedEntities;
      case GibType.Drop:
        this.DropEntity(gibbable, parent, randomSpreadMod, ref droppedEntities, flingEntity, scatterDirection, scatterImpulse, scatterImpulseVariance, scatterCone);
        DroppedEntities.Add((EntityUid) gibbable);
        return DroppedEntities;
      default:
        if (gibbable.Comp != null && num > 0)
        {
          if (flingEntity)
          {
            for (int index = 0; index < args1.GibletCount; ++index)
            {
              EntityUid? gibletEntity;
              if (this.TryCreateRandomGiblet(gibbable.Comp, parent.Comp.Coordinates, false, out gibletEntity, new float?(randomSpreadMod)))
              {
                this.FlingDroppedEntity(gibletEntity.Value, scatterDirection, scatterImpulse, scatterImpulseVariance, scatterCone);
                droppedEntities.Add(gibletEntity.Value);
              }
            }
          }
          else
          {
            for (int index = 0; index < args1.GibletCount; ++index)
            {
              EntityUid? gibletEntity;
              if (this.TryCreateRandomGiblet(gibbable.Comp, parent.Comp.Coordinates, false, out gibletEntity, new float?(randomSpreadMod)))
                droppedEntities.Add(gibletEntity.Value);
            }
          }
        }
        this._transformSystem.AttachToGridOrMap((EntityUid) gibbable, this.Transform((EntityUid) gibbable));
        if (flingEntity)
          this.FlingDroppedEntity((EntityUid) gibbable, scatterDirection, scatterImpulse, scatterImpulseVariance, scatterCone);
        EntityGibbedEvent args2 = new EntityGibbedEvent((EntityUid) gibbable, DroppedEntities);
        this.RaiseLocalEvent<EntityGibbedEvent>((EntityUid) gibbable, ref args2);
        if (deleteTarget)
          this.PredictedQueueDel(gibbable.Owner);
        return DroppedEntities;
    }
  }

  public bool TryCreateRandomGiblet(
    Entity<GibbableComponent?> gibbable,
    [NotNullWhen(true)] out EntityUid? gibletEntity,
    float randomSpreadModifier = 1f,
    bool playSound = true)
  {
    gibletEntity = new EntityUid?();
    return this.Resolve<GibbableComponent>((EntityUid) gibbable, ref gibbable.Comp) && this.TryCreateRandomGiblet(gibbable.Comp, this.Transform((EntityUid) gibbable).Coordinates, playSound, out gibletEntity, new float?(randomSpreadModifier));
  }

  public bool TryCreateAndFlingRandomGiblet(
    Entity<GibbableComponent?> gibbable,
    [NotNullWhen(true)] out EntityUid? gibletEntity,
    Vector2 scatterDirection,
    float force,
    float scatterImpulseVariance,
    Angle scatterCone = default (Angle),
    bool playSound = true)
  {
    gibletEntity = new EntityUid?();
    if (!this.Resolve<GibbableComponent>((EntityUid) gibbable, ref gibbable.Comp) || !this.TryCreateRandomGiblet(gibbable.Comp, this.Transform((EntityUid) gibbable).Coordinates, playSound, out gibletEntity))
      return false;
    this.FlingDroppedEntity(gibletEntity.Value, new Vector2?(scatterDirection), force, scatterImpulseVariance, scatterCone);
    return true;
  }

  private void FlingDroppedEntity(
    EntityUid target,
    Vector2? direction,
    float impulse,
    float impulseVariance,
    Angle scatterConeAngle)
  {
    Angle angle1 = direction.HasValue ? DirectionExtensions.ToAngle(direction.GetValueOrDefault()) : this._random.NextAngle();
    Angle angle2 = this._random.NextAngle(Angle.op_Subtraction(angle1, Angle.op_Implicit(Angle.op_Implicit(scatterConeAngle) / 2.0)), Angle.op_Addition(angle1, Angle.op_Implicit(Angle.op_Implicit(scatterConeAngle) / 2.0)));
    Vector2 impulse1 = ((Angle) ref angle2).ToVec() * (impulse + this._random.NextFloat(impulseVariance));
    this._physicsSystem.ApplyLinearImpulse(target, impulse1);
  }

  private bool TryCreateRandomGiblet(
    GibbableComponent gibbable,
    EntityCoordinates coords,
    bool playSound,
    [NotNullWhen(true)] out EntityUid? gibletEntity,
    float? randomSpreadModifier = null)
  {
    gibletEntity = new EntityUid?();
    if (gibbable.GibPrototypes.Count == 0)
      return false;
    gibletEntity = new EntityUid?(this.Spawn((string) gibbable.GibPrototypes[this._random.Next(0, gibbable.GibPrototypes.Count)], !randomSpreadModifier.HasValue ? coords : coords.Offset(this._random.NextVector2(gibbable.GibScatterRange * randomSpreadModifier.Value))));
    if (playSound)
      this._audioSystem.PlayPredicted(gibbable.GibSound, coords, new EntityUid?());
    this._transformSystem.SetWorldRotation(gibletEntity.Value, this._random.NextAngle());
    return true;
  }
}
