// Decompiled with JetBrains decompiler
// Type: Content.Shared.Instruments.SharedInstrumentComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Instruments;

[NetworkedComponent]
[Access(new Type[] {typeof (SharedInstrumentSystem)})]
public abstract class SharedInstrumentComponent : 
  Component,
  ISerializationGenerated<SharedInstrumentComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Playing { get; set; }

  [DataField("program", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public byte InstrumentProgram { get; set; }

  [DataField("bank", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public byte InstrumentBank { get; set; }

  [DataField("allowPercussion", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool AllowPercussion { get; set; }

  [DataField("allowProgramChange", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool AllowProgramChange { get; set; }

  [DataField("respectMidiLimits", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool RespectMidiLimits { get; set; } = true;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public EntityUid? Master { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public BitArray FilteredChannels { get; set; } = new BitArray(16 /*0x10*/, true);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref SharedInstrumentComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SharedInstrumentComponent) target1;
    if (serialization.TryCustomCopy<SharedInstrumentComponent>(this, ref target, hookCtx, false, context))
      return;
    byte target2 = 0;
    if (!serialization.TryCustomCopy<byte>(this.InstrumentProgram, ref target2, hookCtx, false, context))
      target2 = this.InstrumentProgram;
    target.InstrumentProgram = target2;
    byte target3 = 0;
    if (!serialization.TryCustomCopy<byte>(this.InstrumentBank, ref target3, hookCtx, false, context))
      target3 = this.InstrumentBank;
    target.InstrumentBank = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowPercussion, ref target4, hookCtx, false, context))
      target4 = this.AllowPercussion;
    target.AllowPercussion = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowProgramChange, ref target5, hookCtx, false, context))
      target5 = this.AllowProgramChange;
    target.AllowProgramChange = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.RespectMidiLimits, ref target6, hookCtx, false, context))
      target6 = this.RespectMidiLimits;
    target.RespectMidiLimits = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref SharedInstrumentComponent target,
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
    SharedInstrumentComponent target1 = (SharedInstrumentComponent) target;
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
    SharedInstrumentComponent target1 = (SharedInstrumentComponent) target;
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
    SharedInstrumentComponent target1 = (SharedInstrumentComponent) target;
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
  virtual SharedInstrumentComponent Component.Instantiate() => throw new NotImplementedException();
}
