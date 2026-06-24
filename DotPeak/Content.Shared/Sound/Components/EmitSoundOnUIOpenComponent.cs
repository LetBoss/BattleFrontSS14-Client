// Decompiled with JetBrains decompiler
// Type: Content.Shared.Sound.Components.EmitSoundOnUIOpenComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Sound.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class EmitSoundOnUIOpenComponent : 
  BaseEmitSoundComponent,
  ISerializationGenerated<EmitSoundOnUIOpenComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist Blacklist = new EntityWhitelist();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EmitSoundOnUIOpenComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BaseEmitSoundComponent target1 = (BaseEmitSoundComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EmitSoundOnUIOpenComponent) target1;
    if (serialization.TryCustomCopy<EmitSoundOnUIOpenComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (this.Blacklist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target2, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target2, hookCtx, context, true);
    }
    target.Blacklist = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EmitSoundOnUIOpenComponent target,
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
    EmitSoundOnUIOpenComponent target1 = (EmitSoundOnUIOpenComponent) target;
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
    EmitSoundOnUIOpenComponent target1 = (EmitSoundOnUIOpenComponent) target;
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
    EmitSoundOnUIOpenComponent target1 = (EmitSoundOnUIOpenComponent) target;
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
  virtual EmitSoundOnUIOpenComponent BaseEmitSoundComponent.Instantiate()
  {
    return new EmitSoundOnUIOpenComponent();
  }
}
