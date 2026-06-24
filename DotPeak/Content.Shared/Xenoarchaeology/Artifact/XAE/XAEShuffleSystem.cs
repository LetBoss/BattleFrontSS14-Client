// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAE.XAEShuffleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mobs.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAE.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAE;

public sealed class XAEShuffleSystem : BaseXAESystem<XAEShuffleComponent>
{
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedTransformSystem _xform;
  [Dependency]
  private IGameTiming _timing;
  private Robust.Shared.GameObjects.EntityQuery<MobStateComponent> _mobState;
  private readonly HashSet<EntityUid> _entities = new HashSet<EntityUid>();

  public override void Initialize()
  {
    base.Initialize();
    this._mobState = this.GetEntityQuery<MobStateComponent>();
  }

  protected override void OnActivated(
    Entity<XAEShuffleComponent> ent,
    ref XenoArtifactNodeActivatedEvent args)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    List<Entity<TransformComponent>> list = new List<Entity<TransformComponent>>();
    this._entities.Clear();
    this._lookup.GetEntitiesInRange(ent.Owner, ent.Comp.Radius, this._entities, LookupFlags.Dynamic | LookupFlags.Sundries);
    foreach (EntityUid entity in this._entities)
    {
      if (this._mobState.HasComponent(entity))
      {
        TransformComponent transformComponent = this.Transform(entity);
        list.Add((Entity<TransformComponent>) (entity, transformComponent));
      }
    }
    this._random.Shuffle<Entity<TransformComponent>>((IList<Entity<TransformComponent>>) list);
    while (list.Count > 1)
    {
      Entity<TransformComponent> entity1 = this._random.PickAndTake<Entity<TransformComponent>>((IList<Entity<TransformComponent>>) list);
      Entity<TransformComponent> entity2 = this._random.PickAndTake<Entity<TransformComponent>>((IList<Entity<TransformComponent>>) list);
      this._xform.SwapPositions((Entity<TransformComponent>) ((EntityUid) entity1, (TransformComponent) entity1), (Entity<TransformComponent>) ((EntityUid) entity2, (TransformComponent) entity2));
    }
  }
}
