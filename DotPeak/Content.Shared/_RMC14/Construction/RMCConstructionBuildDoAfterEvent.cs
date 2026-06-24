// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Construction.RMCConstructionBuildDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Construction.Prototypes;
using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Construction;

[NetSerializable]
[Serializable]
public sealed class RMCConstructionBuildDoAfterEvent : 
  SimpleDoAfterEvent,
  ISerializationGenerated<RMCConstructionBuildDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public RMCConstructionPrototype Prototype;
  [DataField(null, false, 1, true, false, null)]
  public int Amount;
  [DataField(null, false, 1, true, false, null)]
  public NetCoordinates Coordinates;
  [DataField(null, false, 1, true, false, null)]
  public Direction Direction;

  public RMCConstructionBuildDoAfterEvent(
    RMCConstructionPrototype prototype,
    int amount,
    NetCoordinates coordinates,
    Direction direction)
  {
    this.Prototype = prototype;
    this.Amount = amount;
    this.Coordinates = coordinates;
    this.Direction = direction;
  }

  public RMCConstructionBuildDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCConstructionBuildDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCConstructionBuildDoAfterEvent) target1;
    if (serialization.TryCustomCopy<RMCConstructionBuildDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    RMCConstructionPrototype target2 = (RMCConstructionPrototype) null;
    if (this.Prototype == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<RMCConstructionPrototype>(this.Prototype, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<RMCConstructionPrototype>(this.Prototype, hookCtx, context);
    target.Prototype = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Amount, ref target3, hookCtx, false, context))
      target3 = this.Amount;
    target.Amount = target3;
    NetCoordinates target4 = new NetCoordinates();
    if (!serialization.TryCustomCopy<NetCoordinates>(this.Coordinates, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<NetCoordinates>(this.Coordinates, hookCtx, context);
    target.Coordinates = target4;
    Direction target5 = (Direction) 0;
    if (!serialization.TryCustomCopy<Direction>(this.Direction, ref target5, hookCtx, false, context))
      target5 = this.Direction;
    target.Direction = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCConstructionBuildDoAfterEvent target,
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
    RMCConstructionBuildDoAfterEvent target1 = (RMCConstructionBuildDoAfterEvent) target;
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
    RMCConstructionBuildDoAfterEvent target1 = (RMCConstructionBuildDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RMCConstructionBuildDoAfterEvent SimpleDoAfterEvent.Instantiate()
  {
    return new RMCConstructionBuildDoAfterEvent();
  }
}
