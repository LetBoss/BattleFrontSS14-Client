// Decompiled with JetBrains decompiler
// Type: Content.Shared.RCD.Systems.RCDDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.RCD.Systems;

[NetSerializable]
[Serializable]
public sealed class RCDDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<RCDDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public NetCoordinates Location { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public Direction Direction { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<RCDPrototype> StartingProtoId { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public int Cost { get; private set; } = 1;

  [DataField("fx", false, 1, false, false, null)]
  public NetEntity? Effect { get; private set; }

  private RCDDoAfterEvent()
  {
  }

  public RCDDoAfterEvent(
    NetCoordinates location,
    Direction direction,
    ProtoId<RCDPrototype> startingProtoId,
    int cost,
    NetEntity? effect = null)
  {
    this.Location = location;
    this.Direction = direction;
    this.StartingProtoId = startingProtoId;
    this.Cost = cost;
    this.Effect = effect;
  }

  public override DoAfterEvent Clone() => (DoAfterEvent) this;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RCDDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RCDDoAfterEvent) target1;
    if (serialization.TryCustomCopy<RCDDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    NetCoordinates target2 = new NetCoordinates();
    if (!serialization.TryCustomCopy<NetCoordinates>(this.Location, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<NetCoordinates>(this.Location, hookCtx, context);
    target.Location = target2;
    Direction target3 = (Direction) 0;
    if (!serialization.TryCustomCopy<Direction>(this.Direction, ref target3, hookCtx, false, context))
      target3 = this.Direction;
    target.Direction = target3;
    ProtoId<RCDPrototype> target4 = new ProtoId<RCDPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<RCDPrototype>>(this.StartingProtoId, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<RCDPrototype>>(this.StartingProtoId, hookCtx, context);
    target.StartingProtoId = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.Cost, ref target5, hookCtx, false, context))
      target5 = this.Cost;
    target.Cost = target5;
    NetEntity? target6 = new NetEntity?();
    if (!serialization.TryCustomCopy<NetEntity?>(this.Effect, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<NetEntity?>(this.Effect, hookCtx, context);
    target.Effect = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RCDDoAfterEvent target,
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
    RCDDoAfterEvent target1 = (RCDDoAfterEvent) target;
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
    RCDDoAfterEvent target1 = (RCDDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RCDDoAfterEvent DoAfterEvent.Instantiate() => new RCDDoAfterEvent();
}
