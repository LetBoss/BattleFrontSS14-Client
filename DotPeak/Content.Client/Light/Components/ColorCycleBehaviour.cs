// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.Components.ColorCycleBehaviour
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Animations;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Light.Components;

[DataDefinition]
public sealed class ColorCycleBehaviour : 
  LightBehaviourAnimationTrack,
  ISerializationHooks,
  ISerializationGenerated<ColorCycleBehaviour>,
  ISerializationGenerated
{
  private int _colorIndex;

  [DataField("property", false, 1, false, false, null)]
  public override string Property { get; protected set; } = "Color";

  [DataField("colors", false, 1, false, false, null)]
  public List<Color> ColorsToCycle { get; set; } = new List<Color>();

  public override void OnStart()
  {
    ++this._colorIndex;
    if (this._colorIndex <= this.ColorsToCycle.Count - 1)
      return;
    this._colorIndex = 0;
  }

  public virtual (int KeyFrameIndex, float FramePlayingTime) AdvancePlayback(
    object context,
    int prevKeyFrameIndex,
    float prevPlayingTime,
    float frameTime)
  {
    float num1 = prevPlayingTime + frameTime;
    float num2 = num1 / this.MaxTime;
    switch ((int) this.InterpolateMode)
    {
      case 0:
        this.ApplyProperty(AnimationTrackProperty.InterpolateLinear((object) this.ColorsToCycle[(this._colorIndex - 1) % this.ColorsToCycle.Count], (object) this.ColorsToCycle[this._colorIndex], num2));
        break;
      case 1:
        this.ApplyProperty(AnimationTrackProperty.InterpolateCubic((object) this.ColorsToCycle[this._colorIndex], (object) this.ColorsToCycle[(this._colorIndex + 1) % this.ColorsToCycle.Count], (object) this.ColorsToCycle[(this._colorIndex + 2) % this.ColorsToCycle.Count], (object) this.ColorsToCycle[(this._colorIndex + 3) % this.ColorsToCycle.Count], num2));
        break;
      default:
        this.ApplyProperty((object) this.ColorsToCycle[this._colorIndex]);
        break;
    }
    return (-1, num1);
  }

  void ISerializationHooks.AfterDeserialization()
  {
    if (this.ColorsToCycle.Count < 2)
      throw new InvalidOperationException("ColorCycleBehaviour has less than 2 colors to cycle");
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ColorCycleBehaviour target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LightBehaviourAnimationTrack target1 = (LightBehaviourAnimationTrack) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ColorCycleBehaviour) target1;
    if (serialization.TryCustomCopy<ColorCycleBehaviour>(this, ref target, hookCtx, true, context))
      return;
    string str = (string) null;
    if (this.Property == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Property, ref str, hookCtx, false, context))
      str = this.Property;
    target.Property = str;
    List<Color> colorList = (List<Color>) null;
    if (this.ColorsToCycle == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<Color>>(this.ColorsToCycle, ref colorList, hookCtx, true, context))
      colorList = serialization.CreateCopy<List<Color>>(this.ColorsToCycle, hookCtx, context, false);
    target.ColorsToCycle = colorList;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ColorCycleBehaviour target,
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
    ColorCycleBehaviour target1 = (ColorCycleBehaviour) target;
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
    ColorCycleBehaviour target1 = (ColorCycleBehaviour) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ColorCycleBehaviour LightBehaviourAnimationTrack.Instantiate()
  {
    return new ColorCycleBehaviour();
  }
}
