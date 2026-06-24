// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.Components.FadeBehaviour
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Animations;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Light.Components;

public sealed class FadeBehaviour : 
  LightBehaviourAnimationTrack,
  ISerializationGenerated<FadeBehaviour>,
  ISerializationGenerated
{
  [DataField("reverseWhenFinished", false, 1, false, false, null)]
  public bool ReverseWhenFinished { get; set; }

  public virtual (int KeyFrameIndex, float FramePlayingTime) AdvancePlayback(
    object context,
    int prevKeyFrameIndex,
    float prevPlayingTime,
    float frameTime)
  {
    float num = prevPlayingTime + frameTime;
    float interpolateValue = num / this.MaxTime;
    if (this.Property == "AnimatedEnable")
    {
      this.ApplyProperty((object) ((double) interpolateValue < (double) this.EndValue));
      return (-1, num);
    }
    if (this.ReverseWhenFinished)
    {
      if ((double) interpolateValue < 0.5)
        this.ApplyInterpolation(this.StartValue, this.EndValue, interpolateValue * 2f);
      else
        this.ApplyInterpolation(this.EndValue, this.StartValue, (float) (((double) interpolateValue - 0.5) * 2.0));
    }
    else
      this.ApplyInterpolation(this.StartValue, this.EndValue, interpolateValue);
    return (-1, num);
  }

  private void ApplyInterpolation(float start, float end, float interpolateValue)
  {
    switch ((int) this.InterpolateMode)
    {
      case 0:
        this.ApplyProperty(AnimationTrackProperty.InterpolateLinear((object) start, (object) end, interpolateValue));
        break;
      case 1:
        this.ApplyProperty(AnimationTrackProperty.InterpolateCubic((object) end, (object) start, (object) end, (object) start, interpolateValue));
        break;
      default:
        this.ApplyProperty((object) (float) ((double) interpolateValue < 0.5 ? (double) start : (double) end));
        break;
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FadeBehaviour target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LightBehaviourAnimationTrack target1 = (LightBehaviourAnimationTrack) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FadeBehaviour) target1;
    if (serialization.TryCustomCopy<FadeBehaviour>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.ReverseWhenFinished, ref flag, hookCtx, false, context))
      flag = this.ReverseWhenFinished;
    target.ReverseWhenFinished = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FadeBehaviour target,
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
    FadeBehaviour target1 = (FadeBehaviour) target;
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
    FadeBehaviour target1 = (FadeBehaviour) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual FadeBehaviour LightBehaviourAnimationTrack.Instantiate() => new FadeBehaviour();
}
