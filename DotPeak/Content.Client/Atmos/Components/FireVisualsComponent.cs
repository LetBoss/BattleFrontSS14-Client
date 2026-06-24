// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.Components.FireVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Atmos.Components;

[RegisterComponent]
public sealed class FireVisualsComponent : 
  Component,
  ISerializationGenerated<FireVisualsComponent>,
  ISerializationGenerated
{
  [DataField("fireStackAlternateState", false, 1, false, false, null)]
  public int FireStackAlternateState = 3;
  [DataField("normalState", false, 1, false, false, null)]
  public string? NormalState;
  [DataField("alternateState", false, 1, false, false, null)]
  public string? AlternateState;
  [DataField("sprite", false, 1, false, false, null)]
  public string? Sprite;
  [DataField("lightEnergyPerStack", false, 1, false, false, null)]
  public float LightEnergyPerStack = 0.5f;
  [DataField("lightRadiusPerStack", false, 1, false, false, null)]
  public float LightRadiusPerStack = 0.3f;
  [DataField("maxLightEnergy", false, 1, false, false, null)]
  public float MaxLightEnergy = 10f;
  [DataField("maxLightRadius", false, 1, false, false, null)]
  public float MaxLightRadius = 4f;
  [DataField("lightColor", false, 1, false, false, null)]
  public Color LightColor = Color.Orange;
  public EntityUid? LightEntity;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FireVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (FireVisualsComponent) component;
    if (serialization.TryCustomCopy<FireVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    int num1 = 0;
    if (!serialization.TryCustomCopy<int>(this.FireStackAlternateState, ref num1, hookCtx, false, context))
      num1 = this.FireStackAlternateState;
    target.FireStackAlternateState = num1;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.NormalState, ref str1, hookCtx, false, context))
      str1 = this.NormalState;
    target.NormalState = str1;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.AlternateState, ref str2, hookCtx, false, context))
      str2 = this.AlternateState;
    target.AlternateState = str2;
    string str3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Sprite, ref str3, hookCtx, false, context))
      str3 = this.Sprite;
    target.Sprite = str3;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LightEnergyPerStack, ref num2, hookCtx, false, context))
      num2 = this.LightEnergyPerStack;
    target.LightEnergyPerStack = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LightRadiusPerStack, ref num3, hookCtx, false, context))
      num3 = this.LightRadiusPerStack;
    target.LightRadiusPerStack = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxLightEnergy, ref num4, hookCtx, false, context))
      num4 = this.MaxLightEnergy;
    target.MaxLightEnergy = num4;
    float num5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxLightRadius, ref num5, hookCtx, false, context))
      num5 = this.MaxLightRadius;
    target.MaxLightRadius = num5;
    Color color = new Color();
    if (!serialization.TryCustomCopy<Color>(this.LightColor, ref color, hookCtx, false, context))
      color = serialization.CreateCopy<Color>(this.LightColor, hookCtx, context, false);
    target.LightColor = color;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FireVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FireVisualsComponent target1 = (FireVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FireVisualsComponent target1 = (FireVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FireVisualsComponent target1 = (FireVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual FireVisualsComponent Component.Instantiate() => new FireVisualsComponent();
}
