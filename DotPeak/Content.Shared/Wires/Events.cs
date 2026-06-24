// Decompiled with JetBrains decompiler
// Type: Content.Shared.Wires.WireDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Wires;

[NetSerializable]
[Serializable]
public sealed class WireDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<WireDoAfterEvent>,
  ISerializationGenerated
{
  [DataField("action", false, 1, true, false, null)]
  public WiresAction Action;
  [DataField("id", false, 1, true, false, null)]
  public int Id;

  private WireDoAfterEvent()
  {
  }

  public WireDoAfterEvent(WiresAction action, int id)
  {
    this.Action = action;
    this.Id = id;
  }

  public override DoAfterEvent Clone() => (DoAfterEvent) this;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WireDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WireDoAfterEvent) target1;
    if (serialization.TryCustomCopy<WireDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    WiresAction target2 = WiresAction.Mend;
    if (!serialization.TryCustomCopy<WiresAction>(this.Action, ref target2, hookCtx, false, context))
      target2 = this.Action;
    target.Action = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Id, ref target3, hookCtx, false, context))
      target3 = this.Id;
    target.Id = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WireDoAfterEvent target,
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
    WireDoAfterEvent target1 = (WireDoAfterEvent) target;
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
    WireDoAfterEvent target1 = (WireDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual WireDoAfterEvent DoAfterEvent.Instantiate() => new WireDoAfterEvent();
}
