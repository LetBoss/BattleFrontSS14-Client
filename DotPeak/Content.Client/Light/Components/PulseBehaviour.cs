// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.Components.PulseBehaviour
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

public sealed class PulseBehaviour : 
  LightBehaviourAnimationTrack,
  ISerializationGenerated<PulseBehaviour>,
  ISerializationGenerated
{
  public virtual (int KeyFrameIndex, float FramePlayingTime) AdvancePlayback(
    object context,
    int prevKeyFrameIndex,
    float prevPlayingTime,
    float frameTime)
  {
    float num1 = prevPlayingTime + frameTime;
    float num2 = num1 / this.MaxTime;
    if (this.Property == "AnimatedEnable")
    {
      this.ApplyProperty((object) ((double) num2 < 0.5));
      return (-1, num1);
    }
    if ((double) num2 < 0.5)
    {
      switch ((int) this.InterpolateMode)
      {
        case 0:
          this.ApplyProperty(AnimationTrackProperty.InterpolateLinear((object) this.StartValue, (object) this.EndValue, num2 * 2f));
          break;
        case 1:
          this.ApplyProperty(AnimationTrackProperty.InterpolateCubic((object) this.EndValue, (object) this.StartValue, (object) this.EndValue, (object) this.StartValue, num2 * 2f));
          break;
        default:
          this.ApplyProperty((object) this.StartValue);
          break;
      }
    }
    else
    {
      switch ((int) this.InterpolateMode)
      {
        case 0:
          this.ApplyProperty(AnimationTrackProperty.InterpolateLinear((object) this.EndValue, (object) this.StartValue, (float) (((double) num2 - 0.5) * 2.0)));
          break;
        case 1:
          this.ApplyProperty(AnimationTrackProperty.InterpolateCubic((object) this.StartValue, (object) this.EndValue, (object) this.StartValue, (object) this.EndValue, (float) (((double) num2 - 0.5) * 2.0)));
          break;
        default:
          this.ApplyProperty((object) this.EndValue);
          break;
      }
    }
    return (-1, num1);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PulseBehaviour target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LightBehaviourAnimationTrack target1 = (LightBehaviourAnimationTrack) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PulseBehaviour) target1;
    serialization.TryCustomCopy<PulseBehaviour>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PulseBehaviour target,
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
    PulseBehaviour target1 = (PulseBehaviour) target;
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
    PulseBehaviour target1 = (PulseBehaviour) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PulseBehaviour LightBehaviourAnimationTrack.Instantiate() => new PulseBehaviour();
}
