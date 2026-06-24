// Decompiled with JetBrains decompiler
// Type: Content.Shared.ParticleAccelerator.Components.ParticleAcceleratorEmitterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.ParticleAccelerator.Components;

[RegisterComponent]
public sealed class ParticleAcceleratorEmitterComponent : 
  Component,
  ISerializationGenerated<ParticleAcceleratorEmitterComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId EmittedPrototype = (EntProtoId) "ParticlesProjectile";
  [DataField("emitterType", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public ParticleAcceleratorEmitterType Type = ParticleAcceleratorEmitterType.Fore;

  public override string ToString() => base.ToString() + $" EmitterType:{this.Type}";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ParticleAcceleratorEmitterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ParticleAcceleratorEmitterComponent) target1;
    if (serialization.TryCustomCopy<ParticleAcceleratorEmitterComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.EmittedPrototype, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.EmittedPrototype, hookCtx, context);
    target.EmittedPrototype = target2;
    ParticleAcceleratorEmitterType target3 = ParticleAcceleratorEmitterType.Port;
    if (!serialization.TryCustomCopy<ParticleAcceleratorEmitterType>(this.Type, ref target3, hookCtx, false, context))
      target3 = this.Type;
    target.Type = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ParticleAcceleratorEmitterComponent target,
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
    ParticleAcceleratorEmitterComponent target1 = (ParticleAcceleratorEmitterComponent) target;
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
    ParticleAcceleratorEmitterComponent target1 = (ParticleAcceleratorEmitterComponent) target;
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
    ParticleAcceleratorEmitterComponent target1 = (ParticleAcceleratorEmitterComponent) target;
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
  virtual ParticleAcceleratorEmitterComponent Component.Instantiate()
  {
    return new ParticleAcceleratorEmitterComponent();
  }
}
