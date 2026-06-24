// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.Components.LightBehaviourComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Light.Components;
using Robust.Client.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Light.Components;

[RegisterComponent]
public sealed class LightBehaviourComponent : 
  SharedLightBehaviourComponent,
  ISerializationHooks,
  ISerializationGenerated<LightBehaviourComponent>,
  ISerializationGenerated
{
  public const string KeyPrefix = "LightBehaviourComponent";
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("behaviours", false, 1, false, false, null)]
  public List<LightBehaviourAnimationTrack> Behaviours = new List<LightBehaviourAnimationTrack>();
  [Robust.Shared.ViewVariables.ViewVariables]
  public readonly List<LightBehaviourComponent.AnimationContainer> Animations = new List<LightBehaviourComponent.AnimationContainer>();
  [Robust.Shared.ViewVariables.ViewVariables]
  public Dictionary<string, object> OriginalPropertyValues = new Dictionary<string, object>();

  void ISerializationHooks.AfterDeserialization()
  {
    int key = 0;
    foreach (LightBehaviourAnimationTrack behaviour in this.Behaviours)
    {
      Animation animation = new Animation()
      {
        AnimationTracks = {
          (AnimationTrack) behaviour
        }
      };
      this.Animations.Add(new LightBehaviourComponent.AnimationContainer(key, animation, behaviour));
      ++key;
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LightBehaviourComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SharedLightBehaviourComponent target1 = (SharedLightBehaviourComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (LightBehaviourComponent) target1;
    if (serialization.TryCustomCopy<LightBehaviourComponent>(this, ref target, hookCtx, true, context))
      return;
    List<LightBehaviourAnimationTrack> behaviourAnimationTrackList = (List<LightBehaviourAnimationTrack>) null;
    if (this.Behaviours == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<LightBehaviourAnimationTrack>>(this.Behaviours, ref behaviourAnimationTrackList, hookCtx, true, context))
      behaviourAnimationTrackList = serialization.CreateCopy<List<LightBehaviourAnimationTrack>>(this.Behaviours, hookCtx, context, false);
    target.Behaviours = behaviourAnimationTrackList;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LightBehaviourComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref SharedLightBehaviourComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LightBehaviourComponent target1 = (LightBehaviourComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (SharedLightBehaviourComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LightBehaviourComponent target1 = (LightBehaviourComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LightBehaviourComponent target1 = (LightBehaviourComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual LightBehaviourComponent SharedLightBehaviourComponent.Instantiate()
  {
    return new LightBehaviourComponent();
  }

  public sealed class AnimationContainer
  {
    public AnimationContainer(int key, Animation animation, LightBehaviourAnimationTrack track)
    {
      this.Key = key;
      this.Animation = animation;
      this.LightBehaviour = track;
    }

    public string FullKey => nameof (LightBehaviourComponent) + this.Key.ToString();

    public int Key { get; set; }

    public Animation Animation { get; set; }

    public LightBehaviourAnimationTrack LightBehaviour { get; set; }
  }
}
