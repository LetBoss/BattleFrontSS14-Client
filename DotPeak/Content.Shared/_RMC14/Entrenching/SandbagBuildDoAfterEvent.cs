// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Entrenching.SandbagBuildDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Entrenching;

[NetSerializable]
[Serializable]
public sealed class SandbagBuildDoAfterEvent : 
  SimpleDoAfterEvent,
  ISerializationGenerated<SandbagBuildDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public NetCoordinates Coordinates;
  [DataField(null, false, 1, true, false, null)]
  public Direction Direction;

  public SandbagBuildDoAfterEvent(NetCoordinates coordinates, Direction direction)
  {
    this.Coordinates = coordinates;
    this.Direction = direction;
  }

  public SandbagBuildDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SandbagBuildDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SandbagBuildDoAfterEvent) target1;
    if (serialization.TryCustomCopy<SandbagBuildDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    NetCoordinates target2 = new NetCoordinates();
    if (!serialization.TryCustomCopy<NetCoordinates>(this.Coordinates, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<NetCoordinates>(this.Coordinates, hookCtx, context);
    target.Coordinates = target2;
    Direction target3 = (Direction) 0;
    if (!serialization.TryCustomCopy<Direction>(this.Direction, ref target3, hookCtx, false, context))
      target3 = this.Direction;
    target.Direction = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SandbagBuildDoAfterEvent target,
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
    SandbagBuildDoAfterEvent target1 = (SandbagBuildDoAfterEvent) target;
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
    SandbagBuildDoAfterEvent target1 = (SandbagBuildDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SandbagBuildDoAfterEvent SimpleDoAfterEvent.Instantiate()
  {
    return new SandbagBuildDoAfterEvent();
  }
}
