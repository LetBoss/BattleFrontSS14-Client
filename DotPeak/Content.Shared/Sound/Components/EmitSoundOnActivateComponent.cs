// Decompiled with JetBrains decompiler
// Type: Content.Shared.Sound.Components.EmitSoundOnActivateComponent
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
namespace Content.Shared.Sound.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class EmitSoundOnActivateComponent : 
  BaseEmitSoundComponent,
  ISerializationGenerated<EmitSoundOnActivateComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Handle = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EmitSoundOnActivateComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BaseEmitSoundComponent target1 = (BaseEmitSoundComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EmitSoundOnActivateComponent) target1;
    if (serialization.TryCustomCopy<EmitSoundOnActivateComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Handle, ref target2, hookCtx, false, context))
      target2 = this.Handle;
    target.Handle = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EmitSoundOnActivateComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref BaseEmitSoundComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EmitSoundOnActivateComponent target1 = (EmitSoundOnActivateComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (BaseEmitSoundComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EmitSoundOnActivateComponent target1 = (EmitSoundOnActivateComponent) target;
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
    EmitSoundOnActivateComponent target1 = (EmitSoundOnActivateComponent) target;
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
  virtual EmitSoundOnActivateComponent BaseEmitSoundComponent.Instantiate()
  {
    return new EmitSoundOnActivateComponent();
  }
}
