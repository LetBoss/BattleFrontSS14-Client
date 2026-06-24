// Decompiled with JetBrains decompiler
// Type: Content.Client.Animations.EntityPickupAnimationSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Spawners;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Animations;

public sealed class EntityPickupAnimationSystem : EntitySystem
{
  [Dependency]
  private AnimationPlayerSystem _animations;
  [Dependency]
  private MetaDataSystem _metaData;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private TransformSystem _transform;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityPickupAnimationComponent, AnimationCompletedEvent>(new ComponentEventHandler<EntityPickupAnimationComponent, AnimationCompletedEvent>((object) this, __methodptr(OnEntityPickupAnimationCompleted)), (Type[]) null, (Type[]) null);
  }

  private void OnEntityPickupAnimationCompleted(
    EntityUid uid,
    EntityPickupAnimationComponent component,
    AnimationCompletedEvent args)
  {
    this.Del(new EntityUid?(uid));
  }

  public void AnimateEntityPickup(
    EntityUid uid,
    EntityCoordinates initial,
    Vector2 final,
    Angle initialAngle)
  {
    if (this.Deleted(uid, (MetaDataComponent) null) || !((EntityCoordinates) ref initial).IsValid((IEntityManager) this.EntityManager))
      return;
    MetaDataComponent metaDataComponent = this.MetaData(uid);
    if (this.IsPaused(new EntityUid?(uid), metaDataComponent))
      return;
    EntityUid entityUid = this.Spawn("clientsideclone", initial);
    this.EnsureComp<EntityPickupAnimationComponent>(entityUid);
    string entityName = metaDataComponent.EntityName;
    this._metaData.SetEntityName(entityUid, entityName, (MetaDataComponent) null, true);
    SpriteComponent spriteComponent1;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent1))
    {
      this.Log.Error("Entity ({0}) couldn't be animated for pickup since it doesn't have a {1}!", new object[2]
      {
        (object) metaDataComponent.EntityName,
        (object) "SpriteComponent"
      });
    }
    else
    {
      SpriteComponent spriteComponent2 = this.Comp<SpriteComponent>(entityUid);
      this._sprite.CopySprite(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent1)), Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent2)));
      this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent2)), true);
      AnimationPlayerComponent animationPlayerComponent = this.Comp<AnimationPlayerComponent>(entityUid);
      this.EnsureComp<TimedDespawnComponent>(entityUid).Lifetime = 0.25f;
      ((SharedTransformSystem) this._transform).SetLocalRotationNoLerp(entityUid, initialAngle, (TransformComponent) null);
      AnimationPlayerSystem animations = this._animations;
      Entity<AnimationPlayerComponent> entity = new Entity<AnimationPlayerComponent>(entityUid, animationPlayerComponent);
      Animation animation = new Animation();
      animation.Length = TimeSpan.FromMilliseconds(125L);
      List<AnimationTrack> animationTracks = animation.AnimationTracks;
      AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
      componentProperty.ComponentType = typeof (TransformComponent);
      componentProperty.Property = "LocalPosition";
      ((AnimationTrackProperty) componentProperty).InterpolationMode = (AnimationInterpolationMode) 0;
      ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) initial.Position, 0.0f, (Func<float, float>) null));
      ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) final, 0.125f, (Func<float, float>) null));
      animationTracks.Add((AnimationTrack) componentProperty);
      animations.Play(entity, animation, "fancy_pickup_anim");
    }
  }
}
