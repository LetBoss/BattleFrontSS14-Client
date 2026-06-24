// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sound.RMCEmitSoundOnSpawnComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Sound.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Sound;

[RegisterComponent]
public sealed class RMCEmitSoundOnSpawnComponent : 
  BaseEmitSoundComponent,
  ISerializationGenerated<RMCEmitSoundOnSpawnComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Entity;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCEmitSoundOnSpawnComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BaseEmitSoundComponent target1 = (BaseEmitSoundComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCEmitSoundOnSpawnComponent) target1;
    if (serialization.TryCustomCopy<RMCEmitSoundOnSpawnComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Entity, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Entity, hookCtx, context);
    target.Entity = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCEmitSoundOnSpawnComponent target,
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
    RMCEmitSoundOnSpawnComponent target1 = (RMCEmitSoundOnSpawnComponent) target;
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
    RMCEmitSoundOnSpawnComponent target1 = (RMCEmitSoundOnSpawnComponent) target;
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
    RMCEmitSoundOnSpawnComponent target1 = (RMCEmitSoundOnSpawnComponent) target;
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
  virtual RMCEmitSoundOnSpawnComponent BaseEmitSoundComponent.Instantiate()
  {
    return new RMCEmitSoundOnSpawnComponent();
  }
}
