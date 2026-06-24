// Decompiled with JetBrains decompiler
// Type: Content.Shared.Singularity.Components.ContainmentFieldComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Singularity.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ContainmentFieldComponent : 
  Component,
  ISerializationGenerated<ContainmentFieldComponent>,
  ISerializationGenerated
{
  [DataField("throwForce", false, 1, false, false, null)]
  public float ThrowForce = 100f;
  [DataField("maxMass", false, 1, false, false, null)]
  public float MaxMass = 10000f;
  [DataField(null, false, 1, false, false, null)]
  public bool DestroyGarbage = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ContainmentFieldComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ContainmentFieldComponent) target1;
    if (serialization.TryCustomCopy<ContainmentFieldComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ThrowForce, ref target2, hookCtx, false, context))
      target2 = this.ThrowForce;
    target.ThrowForce = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxMass, ref target3, hookCtx, false, context))
      target3 = this.MaxMass;
    target.MaxMass = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.DestroyGarbage, ref target4, hookCtx, false, context))
      target4 = this.DestroyGarbage;
    target.DestroyGarbage = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ContainmentFieldComponent target,
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
    ContainmentFieldComponent target1 = (ContainmentFieldComponent) target;
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
    ContainmentFieldComponent target1 = (ContainmentFieldComponent) target;
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
    ContainmentFieldComponent target1 = (ContainmentFieldComponent) target;
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
  virtual ContainmentFieldComponent Component.Instantiate() => new ContainmentFieldComponent();
}
