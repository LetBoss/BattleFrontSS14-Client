// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.SharedPointLightComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Animations;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

[NetworkedComponent]
[Access(new Type[] {typeof (SharedPointLightSystem)})]
public abstract class SharedPointLightComponent : 
  Component,
  ISerializationGenerated<SharedPointLightComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("offset", false, 1, false, false, null)]
  [Access(new Type[] {}, Other = AccessPermissions.ReadWriteExecute)]
  public Vector2 Offset = Vector2.Zero;
  [DataField("castShadows", false, 1, false, false, null)]
  public bool CastShadows = true;
  [Access(new Type[] {typeof (SharedPointLightSystem)})]
  [DataField("enabled", false, 1, false, false, null)]
  public bool Enabled = true;
  [DataField("radius", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPointLightSystem)})]
  public float Radius = 5f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool ContainerOccluded;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("autoRot", false, 1, false, false, null)]
  public bool MaskAutoRotate;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("mask", false, 1, false, false, null)]
  public string? MaskPath;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("color", false, 1, false, false, null)]
  [Animatable]
  public Color Color { get; set; } = Color.White;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("energy", false, 1, false, false, null)]
  [Animatable]
  public float Energy { get; set; } = 1f;

  [DataField("softness", false, 1, false, false, null)]
  [Animatable]
  public float Softness { get; set; } = 1f;

  [DataField(null, false, 1, false, false, null)]
  [Animatable]
  public float Falloff { get; set; } = 6.8f;

  [DataField(null, false, 1, false, false, null)]
  [Animatable]
  public float CurveFactor { get; set; }

  [Animatable]
  public bool AnimatedEnable
  {
    [Obsolete] get => this.Enabled;
    [Obsolete] set
    {
      IoCManager.Resolve<IEntityManager>().System<SharedPointLightSystem>().SetEnabled(this.Owner, value, this);
    }
  }

  [Animatable]
  public float AnimatedRadius
  {
    [Obsolete] get => this.Radius;
    [Obsolete] set
    {
      IoCManager.Resolve<IEntityManager>().System<SharedPointLightSystem>().SetRadius(this.Owner, value, this);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [Animatable]
  public Angle Rotation { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref SharedPointLightComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SharedPointLightComponent) target1;
    if (serialization.TryCustomCopy<SharedPointLightComponent>(this, ref target, hookCtx, false, context))
      return;
    Color target2 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Color>(this.Color, hookCtx, context);
    target.Color = target2;
    Vector2 target3 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Offset, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Vector2>(this.Offset, hookCtx, context);
    target.Offset = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Energy, ref target4, hookCtx, false, context))
      target4 = this.Energy;
    target.Energy = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Softness, ref target5, hookCtx, false, context))
      target5 = this.Softness;
    target.Softness = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Falloff, ref target6, hookCtx, false, context))
      target6 = this.Falloff;
    target.Falloff = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CurveFactor, ref target7, hookCtx, false, context))
      target7 = this.CurveFactor;
    target.CurveFactor = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.CastShadows, ref target8, hookCtx, false, context))
      target8 = this.CastShadows;
    target.CastShadows = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target9, hookCtx, false, context))
      target9 = this.Enabled;
    target.Enabled = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Radius, ref target10, hookCtx, false, context))
      target10 = this.Radius;
    target.Radius = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.MaskAutoRotate, ref target11, hookCtx, false, context))
      target11 = this.MaskAutoRotate;
    target.MaskAutoRotate = target11;
    string target12 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.MaskPath, ref target12, hookCtx, false, context))
      target12 = this.MaskPath;
    target.MaskPath = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref SharedPointLightComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SharedPointLightComponent target1 = (SharedPointLightComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SharedPointLightComponent target1 = (SharedPointLightComponent) target;
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
    SharedPointLightComponent target1 = (SharedPointLightComponent) target;
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
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SharedPointLightComponent Component.Instantiate() => throw new NotImplementedException();
}
