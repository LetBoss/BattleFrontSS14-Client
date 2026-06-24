// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.AreaPickupDoAfterEvent
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
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Storage;

[NetSerializable]
[Serializable]
public sealed class AreaPickupDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<AreaPickupDoAfterEvent>,
  ISerializationGenerated
{
  [DataField("entities", false, 1, true, false, null)]
  public IReadOnlyList<NetEntity> Entities;

  private AreaPickupDoAfterEvent()
  {
  }

  public AreaPickupDoAfterEvent(List<NetEntity> entities)
  {
    this.Entities = (IReadOnlyList<NetEntity>) entities;
  }

  public override DoAfterEvent Clone() => (DoAfterEvent) this;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AreaPickupDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AreaPickupDoAfterEvent) target1;
    if (serialization.TryCustomCopy<AreaPickupDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    IReadOnlyList<NetEntity> target2 = (IReadOnlyList<NetEntity>) null;
    if (this.Entities == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IReadOnlyList<NetEntity>>(this.Entities, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<IReadOnlyList<NetEntity>>(this.Entities, hookCtx, context);
    target.Entities = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AreaPickupDoAfterEvent target,
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
    AreaPickupDoAfterEvent target1 = (AreaPickupDoAfterEvent) target;
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
    AreaPickupDoAfterEvent target1 = (AreaPickupDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual AreaPickupDoAfterEvent DoAfterEvent.Instantiate() => new AreaPickupDoAfterEvent();
}
