// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.EntitySystems.LightBehaviorSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Light.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Light.EntitySystems;

public sealed class LightBehaviorSystem : EntitySystem
{
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private AnimationPlayerSystem _player;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<LightBehaviourComponent, ComponentStartup>(new EntityEventRefHandler<LightBehaviourComponent, ComponentStartup>((object) this, __methodptr(OnLightStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<LightBehaviourComponent, AnimationCompletedEvent>(new ComponentEventHandler<LightBehaviourComponent, AnimationCompletedEvent>((object) this, __methodptr(OnBehaviorAnimationCompleted)), (Type[]) null, (Type[]) null);
  }

  private void OnBehaviorAnimationCompleted(
    EntityUid uid,
    LightBehaviourComponent component,
    AnimationCompletedEvent args)
  {
    if (!args.Finished)
      return;
    LightBehaviourComponent.AnimationContainer animationContainer = component.Animations.FirstOrDefault<LightBehaviourComponent.AnimationContainer>((Func<LightBehaviourComponent.AnimationContainer, bool>) (x => x.FullKey == args.Key));
    if (animationContainer == null || !animationContainer.LightBehaviour.IsLooped)
      return;
    animationContainer.LightBehaviour.UpdatePlaybackValues(animationContainer.Animation);
    this._player.Play(uid, animationContainer.Animation, animationContainer.FullKey);
  }

  private void OnLightStartup(Entity<LightBehaviourComponent> entity, ref ComponentStartup args)
  {
    this.EnsureComp<AnimationPlayerComponent>(Entity<LightBehaviourComponent>.op_Implicit(entity));
    foreach (LightBehaviourComponent.AnimationContainer animation in entity.Comp.Animations)
      animation.LightBehaviour.Initialize(Entity<LightBehaviourComponent>.op_Implicit(entity), this._random, (IEntityManager) this.EntityManager);
    foreach (LightBehaviourComponent.AnimationContainer animation in entity.Comp.Animations)
    {
      if (animation.LightBehaviour.Enabled)
        this.StartLightBehaviour(entity, animation.LightBehaviour.ID);
    }
  }

  private void CopyLightSettings(Entity<LightBehaviourComponent> entity, string property)
  {
    PointLightComponent pointLightComponent;
    if (this.TryComp<PointLightComponent>(Entity<LightBehaviourComponent>.op_Implicit(entity), ref pointLightComponent))
    {
      object animatableProperty = AnimationHelper.GetAnimatableProperty((object) pointLightComponent, property);
      if (animatableProperty == null)
        return;
      entity.Comp.OriginalPropertyValues[property] = animatableProperty;
    }
    else
      this.Log.Warning($"{this.Comp<MetaDataComponent>(Entity<LightBehaviourComponent>.op_Implicit(entity)).EntityName} has a {"LightBehaviourComponent"} but it has no {"PointLightComponent"}! Check the prototype!");
  }

  public void StartLightBehaviour(Entity<LightBehaviourComponent> entity, string id = "")
  {
    AnimationPlayerComponent animationPlayerComponent1;
    if (!this.TryComp<AnimationPlayerComponent>(Entity<LightBehaviourComponent>.op_Implicit(entity), ref animationPlayerComponent1))
      return;
    foreach (LightBehaviourComponent.AnimationContainer animation1 in entity.Comp.Animations)
    {
      if (animation1.LightBehaviour.ID == id || id == string.Empty)
      {
        AnimationPlayerSystem player1 = this._player;
        EntityUid entityUid1 = Entity<LightBehaviourComponent>.op_Implicit(entity);
        AnimationPlayerComponent animationPlayerComponent2 = animationPlayerComponent1;
        int key = animation1.Key;
        string str1 = "LightBehaviourComponent" + key.ToString();
        if (!player1.HasRunningAnimation(entityUid1, animationPlayerComponent2, str1))
        {
          this.CopyLightSettings(entity, animation1.LightBehaviour.Property);
          animation1.LightBehaviour.UpdatePlaybackValues(animation1.Animation);
          AnimationPlayerSystem player2 = this._player;
          EntityUid entityUid2 = Entity<LightBehaviourComponent>.op_Implicit(entity);
          Animation animation2 = animation1.Animation;
          key = animation1.Key;
          string str2 = "LightBehaviourComponent" + key.ToString();
          player2.Play(entityUid2, animation2, str2);
        }
      }
    }
  }

  public void StopLightBehaviour(
    Entity<LightBehaviourComponent> entity,
    string id = "",
    bool removeBehaviour = false,
    bool resetToOriginalSettings = false)
  {
    AnimationPlayerComponent animationPlayerComponent1;
    if (!this.TryComp<AnimationPlayerComponent>(Entity<LightBehaviourComponent>.op_Implicit(entity), ref animationPlayerComponent1))
      return;
    LightBehaviourComponent comp = entity.Comp;
    List<LightBehaviourComponent.AnimationContainer> animationContainerList = new List<LightBehaviourComponent.AnimationContainer>();
    foreach (LightBehaviourComponent.AnimationContainer animation in comp.Animations)
    {
      if (animation.LightBehaviour.ID == id || id == string.Empty)
      {
        AnimationPlayerSystem player1 = this._player;
        EntityUid entityUid1 = Entity<LightBehaviourComponent>.op_Implicit(entity);
        AnimationPlayerComponent animationPlayerComponent2 = animationPlayerComponent1;
        int key = animation.Key;
        string str1 = "LightBehaviourComponent" + key.ToString();
        if (player1.HasRunningAnimation(entityUid1, animationPlayerComponent2, str1))
        {
          AnimationPlayerSystem player2 = this._player;
          EntityUid entityUid2 = Entity<LightBehaviourComponent>.op_Implicit(entity);
          AnimationPlayerComponent animationPlayerComponent3 = animationPlayerComponent1;
          key = animation.Key;
          string str2 = "LightBehaviourComponent" + key.ToString();
          player2.Stop(entityUid2, animationPlayerComponent3, str2);
        }
        if (removeBehaviour)
          animationContainerList.Add(animation);
      }
    }
    foreach (LightBehaviourComponent.AnimationContainer animationContainer in animationContainerList)
      comp.Animations.Remove(animationContainer);
    PointLightComponent pointLightComponent;
    if (resetToOriginalSettings && this.TryComp<PointLightComponent>(Entity<LightBehaviourComponent>.op_Implicit(entity), ref pointLightComponent))
    {
      foreach ((string key, object obj) in comp.OriginalPropertyValues)
        AnimationHelper.SetAnimatableProperty((object) pointLightComponent, key, obj);
    }
    comp.OriginalPropertyValues.Clear();
  }

  public bool HasRunningBehaviours(Entity<LightBehaviourComponent> entity)
  {
    AnimationPlayerComponent animation;
    return this.TryComp<AnimationPlayerComponent>(Entity<LightBehaviourComponent>.op_Implicit(entity), ref animation) && entity.Comp.Animations.Any<LightBehaviourComponent.AnimationContainer>((Func<LightBehaviourComponent.AnimationContainer, bool>) (container => this._player.HasRunningAnimation(Entity<LightBehaviourComponent>.op_Implicit(entity), animation, "LightBehaviourComponent" + container.Key.ToString())));
  }

  public void AddNewLightBehaviour(
    Entity<LightBehaviourComponent> entity,
    LightBehaviourAnimationTrack behaviour,
    bool playImmediately = true)
  {
    int key = 0;
    LightBehaviourComponent comp = entity.Comp;
    while (comp.Animations.Any<LightBehaviourComponent.AnimationContainer>((Func<LightBehaviourComponent.AnimationContainer, bool>) (x => x.Key == key)))
      key++;
    Animation animation = new Animation()
    {
      AnimationTracks = {
        (AnimationTrack) behaviour
      }
    };
    behaviour.Initialize(entity.Owner, this._random, (IEntityManager) this.EntityManager);
    LightBehaviourComponent.AnimationContainer animationContainer = new LightBehaviourComponent.AnimationContainer(key, animation, behaviour);
    comp.Animations.Add(animationContainer);
    if (!playImmediately)
      return;
    this.StartLightBehaviour(entity, behaviour.ID);
  }
}
