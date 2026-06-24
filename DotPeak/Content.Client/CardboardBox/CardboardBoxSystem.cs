// Decompiled with JetBrains decompiler
// Type: Content.Client.CardboardBox.CardboardBoxSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Body.Components;
using Content.Shared.CardboardBox;
using Content.Shared.CardboardBox.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Movement.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.CardboardBox;

public sealed class CardboardBoxSystem : SharedCardboardBoxSystem
{
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private TransformSystem _transform;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private SpriteSystem _sprite;
  private EntityQuery<BodyComponent> _bodyQuery;

  public virtual void Initialize()
  {
    base.Initialize();
    this._bodyQuery = this.GetEntityQuery<BodyComponent>();
    this.SubscribeNetworkEvent<PlayBoxEffectMessage>(new EntityEventHandler<PlayBoxEffectMessage>(this.OnBoxEffect), (Type[]) null, (Type[]) null);
  }

  private void OnBoxEffect(PlayBoxEffectMessage msg)
  {
    EntityUid entity1 = this.GetEntity(msg.Source);
    CardboardBoxComponent cardboardBoxComponent;
    if (!this.TryComp<CardboardBoxComponent>(entity1, ref cardboardBoxComponent))
      return;
    EntityQuery<TransformComponent> entityQuery = this.GetEntityQuery<TransformComponent>();
    TransformComponent transformComponent1;
    if (!entityQuery.TryGetComponent(entity1, ref transformComponent1))
      return;
    MapCoordinates mapCoordinates1 = ((SharedTransformSystem) this._transform).GetMapCoordinates(entity1, transformComponent1);
    List<EntityUid> entityUidList = new List<EntityUid>();
    EntityUid entity2 = this.GetEntity(msg.Mover);
    HashSet<Entity<MobMoverComponent>> entitySet = new HashSet<Entity<MobMoverComponent>>();
    this._entityLookup.GetEntitiesInRange<MobMoverComponent>(transformComponent1.Coordinates, cardboardBoxComponent.Distance, entitySet, (LookupFlags) 110);
    foreach (Entity<MobMoverComponent> entity3 in entitySet)
    {
      EntityUid owner = entity3.Owner;
      if (!EntityUid.op_Equality(owner, entity2))
        entityUidList.Add(owner);
    }
    foreach (EntityUid entityUid1 in entityUidList)
    {
      MapCoordinates mapCoordinates2 = ((SharedTransformSystem) this._transform).GetMapCoordinates(entityUid1, (TransformComponent) null);
      if (this._examine.InRangeUnOccluded(mapCoordinates1, mapCoordinates2, cardboardBoxComponent.Distance, (SharedInteractionSystem.Ignored) null) && this._bodyQuery.HasComp(entityUid1))
      {
        EntityUid entityUid2 = this.Spawn(cardboardBoxComponent.Effect, mapCoordinates2, (ComponentRegistry) null, new Angle());
        TransformComponent transformComponent2;
        SpriteComponent spriteComponent;
        if (entityQuery.TryGetComponent(entityUid2, ref transformComponent2) && this.TryComp<SpriteComponent>(entityUid2, ref spriteComponent))
        {
          this._sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((entityUid2, spriteComponent)), new Vector2(0.0f, 1f));
          ((SharedTransformSystem) this._transform).SetParent(entityUid2, transformComponent2, entityUid1, (TransformComponent) null);
        }
      }
    }
  }
}
