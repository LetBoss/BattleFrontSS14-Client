// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Entrenching.EntrenchingToolDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Entrenching;

[NetSerializable]
[Serializable]
public sealed class EntrenchingToolDoAfterEvent : 
  SimpleDoAfterEvent,
  ISerializationGenerated<EntrenchingToolDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public NetCoordinates Coordinates;

  public EntrenchingToolDoAfterEvent(NetCoordinates coordinates) => this.Coordinates = coordinates;

  public EntrenchingToolDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EntrenchingToolDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EntrenchingToolDoAfterEvent) target1;
    if (serialization.TryCustomCopy<EntrenchingToolDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    NetCoordinates target2 = new NetCoordinates();
    if (!serialization.TryCustomCopy<NetCoordinates>(this.Coordinates, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<NetCoordinates>(this.Coordinates, hookCtx, context);
    target.Coordinates = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EntrenchingToolDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref SimpleDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntrenchingToolDoAfterEvent target1 = (EntrenchingToolDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (SimpleDoAfterEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntrenchingToolDoAfterEvent target1 = (EntrenchingToolDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual EntrenchingToolDoAfterEvent SimpleDoAfterEvent.Instantiate()
  {
    return new EntrenchingToolDoAfterEvent();
  }
}
