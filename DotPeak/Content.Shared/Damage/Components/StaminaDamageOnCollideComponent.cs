// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Components.StaminaDamageOnCollideComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Damage.Components;

[RegisterComponent]
public sealed class StaminaDamageOnCollideComponent : 
  Component,
  ISerializationGenerated<StaminaDamageOnCollideComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("damage", false, 1, false, false, null)]
  public float Damage = 55f;
  [DataField("sound", false, 1, false, false, null)]
  public SoundSpecifier? Sound;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StaminaDamageOnCollideComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (StaminaDamageOnCollideComponent) component;
    if (serialization.TryCustomCopy<StaminaDamageOnCollideComponent>(this, ref target, hookCtx, false, context))
      return;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Damage, ref num, hookCtx, false, context))
      num = this.Damage;
    target.Damage = num;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context, false);
    target.Sound = soundSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StaminaDamageOnCollideComponent target,
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
    StaminaDamageOnCollideComponent target1 = (StaminaDamageOnCollideComponent) target;
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
    StaminaDamageOnCollideComponent target1 = (StaminaDamageOnCollideComponent) target;
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
    StaminaDamageOnCollideComponent target1 = (StaminaDamageOnCollideComponent) target;
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
  virtual StaminaDamageOnCollideComponent Component.Instantiate()
  {
    return new StaminaDamageOnCollideComponent();
  }
}
