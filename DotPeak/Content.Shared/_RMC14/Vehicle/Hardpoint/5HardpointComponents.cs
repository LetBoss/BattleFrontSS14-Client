// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.HardpointInsertDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[NetSerializable]
[Serializable]
public sealed class HardpointInsertDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<HardpointInsertDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string SlotId = string.Empty;

  public HardpointInsertDoAfterEvent()
  {
  }

  public HardpointInsertDoAfterEvent(string slotId) => this.SlotId = slotId;

  public override DoAfterEvent Clone()
  {
    return (DoAfterEvent) new HardpointInsertDoAfterEvent(this.SlotId);
  }

  public override bool IsDuplicate(DoAfterEvent other)
  {
    if (other is HardpointInsertDoAfterEvent insertDoAfterEvent && insertDoAfterEvent.SlotId == this.SlotId && other.User == this.User)
    {
      EntityUid? target = other.Target;
      EntityUid? nullable = this.Target;
      if ((target.HasValue == nullable.HasValue ? (target.HasValue ? (target.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      {
        nullable = other.Used;
        EntityUid? used = this.Used;
        if (nullable.HasValue != used.HasValue)
          return false;
        return !nullable.HasValue || nullable.GetValueOrDefault() == used.GetValueOrDefault();
      }
    }
    return false;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HardpointInsertDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HardpointInsertDoAfterEvent) target1;
    if (serialization.TryCustomCopy<HardpointInsertDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.SlotId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SlotId, ref target2, hookCtx, false, context))
      target2 = this.SlotId;
    target.SlotId = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HardpointInsertDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref DoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HardpointInsertDoAfterEvent target1 = (HardpointInsertDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (DoAfterEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HardpointInsertDoAfterEvent target1 = (HardpointInsertDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual HardpointInsertDoAfterEvent DoAfterEvent.Instantiate()
  {
    return new HardpointInsertDoAfterEvent();
  }
}
