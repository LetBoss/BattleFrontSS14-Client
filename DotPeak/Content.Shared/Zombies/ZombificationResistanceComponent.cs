// Decompiled with JetBrains decompiler
// Type: Content.Shared.Zombies.ZombificationResistanceComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Zombies;

[NetworkedComponent]
[RegisterComponent]
public sealed class ZombificationResistanceComponent : 
  Component,
  ISerializationGenerated<ZombificationResistanceComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float ZombificationResistanceCoefficient = 1f;
  [DataField(null, false, 1, false, false, null)]
  public LocId Examine = (LocId) "zombification-resistance-coefficient-value";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ZombificationResistanceComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ZombificationResistanceComponent) target1;
    if (serialization.TryCustomCopy<ZombificationResistanceComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ZombificationResistanceCoefficient, ref target2, hookCtx, false, context))
      target2 = this.ZombificationResistanceCoefficient;
    target.ZombificationResistanceCoefficient = target2;
    LocId target3 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Examine, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId>(this.Examine, hookCtx, context);
    target.Examine = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ZombificationResistanceComponent target,
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
    ZombificationResistanceComponent target1 = (ZombificationResistanceComponent) target;
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
    ZombificationResistanceComponent target1 = (ZombificationResistanceComponent) target;
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
    ZombificationResistanceComponent target1 = (ZombificationResistanceComponent) target;
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
  virtual ZombificationResistanceComponent Component.Instantiate()
  {
    return new ZombificationResistanceComponent();
  }
}
