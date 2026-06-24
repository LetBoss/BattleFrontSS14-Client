// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.Components.RandomizeBehaviour
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Animations;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Light.Components;

public sealed class RandomizeBehaviour : 
  LightBehaviourAnimationTrack,
  ISerializationGenerated<RandomizeBehaviour>,
  ISerializationGenerated
{
  private float _randomValue1;
  private float _randomValue2;
  private float _randomValue3;
  private float _randomValue4;

  public override void OnInitialize()
  {
    this._randomValue1 = (float) AnimationTrackProperty.InterpolateLinear((object) this.StartValue, (object) this.EndValue, (float) this._random.NextDouble());
    this._randomValue2 = (float) AnimationTrackProperty.InterpolateLinear((object) this.StartValue, (object) this.EndValue, (float) this._random.NextDouble());
    this._randomValue3 = (float) AnimationTrackProperty.InterpolateLinear((object) this.StartValue, (object) this.EndValue, (float) this._random.NextDouble());
  }

  public override void OnStart()
  {
    if (this.Property == "AnimatedEnable")
    {
      this.ApplyProperty((object) (this._random.NextDouble() < 0.5));
    }
    else
    {
      if (this.InterpolateMode == 1)
      {
        this._randomValue1 = this._randomValue2;
        this._randomValue2 = this._randomValue3;
      }
      this._randomValue3 = this._randomValue4;
      this._randomValue4 = (float) AnimationTrackProperty.InterpolateLinear((object) this.StartValue, (object) this.EndValue, (float) this._random.NextDouble());
    }
  }

  public virtual (int KeyFrameIndex, float FramePlayingTime) AdvancePlayback(
    object context,
    int prevKeyFrameIndex,
    float prevPlayingTime,
    float frameTime)
  {
    float num1 = prevPlayingTime + frameTime;
    float num2 = num1 / this.MaxTime;
    if (this.Property == "AnimatedEnable")
      return (-1, num1);
    switch ((int) this.InterpolateMode)
    {
      case 0:
        this.ApplyProperty(AnimationTrackProperty.InterpolateLinear((object) this._randomValue3, (object) this._randomValue4, num2));
        break;
      case 1:
        this.ApplyProperty(AnimationTrackProperty.InterpolateCubic((object) this._randomValue1, (object) this._randomValue2, (object) this._randomValue3, (object) this._randomValue4, num2));
        break;
      default:
        this.ApplyProperty((object) (float) ((double) num2 < 0.5 ? (double) this._randomValue3 : (double) this._randomValue4));
        break;
    }
    return (-1, num1);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RandomizeBehaviour target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LightBehaviourAnimationTrack target1 = (LightBehaviourAnimationTrack) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RandomizeBehaviour) target1;
    serialization.TryCustomCopy<RandomizeBehaviour>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RandomizeBehaviour target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref LightBehaviourAnimationTrack target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RandomizeBehaviour target1 = (RandomizeBehaviour) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (LightBehaviourAnimationTrack) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RandomizeBehaviour target1 = (RandomizeBehaviour) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RandomizeBehaviour LightBehaviourAnimationTrack.Instantiate() => new RandomizeBehaviour();
}
