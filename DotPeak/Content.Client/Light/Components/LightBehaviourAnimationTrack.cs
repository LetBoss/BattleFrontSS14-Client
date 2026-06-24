// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.Components.LightBehaviourAnimationTrack
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Client.Light.Components;

[ImplicitDataDefinitionForInheritors]
[Serializable]
public abstract class LightBehaviourAnimationTrack : 
  AnimationTrackProperty,
  ISerializationGenerated<LightBehaviourAnimationTrack>,
  ISerializationGenerated
{
  protected IEntityManager _entMan;
  protected IRobustRandom _random;
  private float _maxTime;
  private EntityUid _parent;

  [DataField("id", false, 1, false, false, null)]
  public string ID { get; set; } = string.Empty;

  [DataField("property", false, 1, false, false, null)]
  public virtual string Property { get; protected set; } = "AnimatedRadius";

  [DataField("isLooped", false, 1, false, false, null)]
  public bool IsLooped { get; set; }

  [DataField("enabled", false, 1, false, false, null)]
  public bool Enabled { get; set; }

  [DataField("startValue", false, 1, false, false, null)]
  public float StartValue { get; set; }

  [DataField("endValue", false, 1, false, false, null)]
  public float EndValue { get; set; } = 2f;

  [DataField("minDuration", false, 1, false, false, null)]
  public float MinDuration { get; set; } = -1f;

  [DataField("maxDuration", false, 1, false, false, null)]
  public float MaxDuration { get; set; } = 2f;

  [DataField("interpolate", false, 1, false, false, null)]
  public AnimationInterpolationMode InterpolateMode { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  protected float MaxTime { get; set; }

  public void Initialize(EntityUid parent, IRobustRandom random, IEntityManager entMan)
  {
    this._random = random;
    this._entMan = entMan;
    this._parent = parent;
    PointLightComponent pointLightComponent;
    if (this.Enabled && this._entMan.TryGetComponent<PointLightComponent>(this._parent, ref pointLightComponent))
      ((SharedPointLightSystem) this._entMan.System<PointLightSystem>()).SetEnabled(this._parent, true, (SharedPointLightComponent) pointLightComponent, (MetaDataComponent) null);
    this.OnInitialize();
  }

  public void UpdatePlaybackValues(Animation owner)
  {
    PointLightComponent pointLightComponent;
    if (this._entMan.TryGetComponent<PointLightComponent>(this._parent, ref pointLightComponent))
      ((SharedPointLightSystem) this._entMan.System<PointLightSystem>()).SetEnabled(this._parent, true, (SharedPointLightComponent) pointLightComponent, (MetaDataComponent) null);
    this.MaxTime = (double) this.MinDuration <= 0.0 ? this.MaxDuration : (float) this._random.NextDouble() * (this.MaxDuration - this.MinDuration) + this.MinDuration;
    owner.Length = TimeSpan.FromSeconds((double) this.MaxTime);
  }

  public virtual (int KeyFrameIndex, float FramePlayingTime) InitPlayback()
  {
    this.OnStart();
    return (-1, this._maxTime);
  }

  protected void ApplyProperty(object value)
  {
    if (this.Property == null)
      throw new InvalidOperationException("Property parameter is null! Check the prototype!");
    PointLightComponent pointLightComponent;
    if (!this._entMan.TryGetComponent<PointLightComponent>(this._parent, ref pointLightComponent))
      return;
    AnimationHelper.SetAnimatableProperty((object) pointLightComponent, this.Property, value);
  }

  protected virtual void ApplyProperty(object context, object value) => this.ApplyProperty(value);

  public virtual void OnInitialize()
  {
  }

  public virtual void OnStart()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref LightBehaviourAnimationTrack target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<LightBehaviourAnimationTrack>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (this.ID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ID, ref str1, hookCtx, false, context))
      str1 = this.ID;
    target.ID = str1;
    string str2 = (string) null;
    if (this.Property == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Property, ref str2, hookCtx, false, context))
      str2 = this.Property;
    target.Property = str2;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsLooped, ref flag1, hookCtx, false, context))
      flag1 = this.IsLooped;
    target.IsLooped = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref flag2, hookCtx, false, context))
      flag2 = this.Enabled;
    target.Enabled = flag2;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StartValue, ref num1, hookCtx, false, context))
      num1 = this.StartValue;
    target.StartValue = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EndValue, ref num2, hookCtx, false, context))
      num2 = this.EndValue;
    target.EndValue = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinDuration, ref num3, hookCtx, false, context))
      num3 = this.MinDuration;
    target.MinDuration = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxDuration, ref num4, hookCtx, false, context))
      num4 = this.MaxDuration;
    target.MaxDuration = num4;
    AnimationInterpolationMode interpolationMode = (AnimationInterpolationMode) 0;
    if (!serialization.TryCustomCopy<AnimationInterpolationMode>(this.InterpolateMode, ref interpolationMode, hookCtx, false, context))
      interpolationMode = this.InterpolateMode;
    target.InterpolateMode = interpolationMode;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref LightBehaviourAnimationTrack target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LightBehaviourAnimationTrack target1 = (LightBehaviourAnimationTrack) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual LightBehaviourAnimationTrack Instantiate() => throw new NotImplementedException();
}
