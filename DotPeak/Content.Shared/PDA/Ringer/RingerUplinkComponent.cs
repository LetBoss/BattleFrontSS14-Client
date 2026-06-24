// Decompiled with JetBrains decompiler
// Type: Content.Shared.PDA.Ringer.RingerUplinkComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.PDA.Ringer;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedRingerSystem)})]
public sealed class RingerUplinkComponent : 
  Component,
  ISerializationGenerated<RingerUplinkComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Note[]? Code;
  [DataField(null, false, 1, false, false, null)]
  public bool Unlocked;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RingerUplinkComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RingerUplinkComponent) target1;
    if (serialization.TryCustomCopy<RingerUplinkComponent>(this, ref target, hookCtx, false, context))
      return;
    Note[] target2 = (Note[]) null;
    if (!serialization.TryCustomCopy<Note[]>(this.Code, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Note[]>(this.Code, hookCtx, context);
    target.Code = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Unlocked, ref target3, hookCtx, false, context))
      target3 = this.Unlocked;
    target.Unlocked = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RingerUplinkComponent target,
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
    RingerUplinkComponent target1 = (RingerUplinkComponent) target;
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
    RingerUplinkComponent target1 = (RingerUplinkComponent) target;
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
    RingerUplinkComponent target1 = (RingerUplinkComponent) target;
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
  virtual RingerUplinkComponent Component.Instantiate() => new RingerUplinkComponent();
}
