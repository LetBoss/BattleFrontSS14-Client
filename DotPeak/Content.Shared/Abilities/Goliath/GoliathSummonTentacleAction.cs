// Decompiled with JetBrains decompiler
// Type: Content.Shared.Abilities.Goliath.GoliathSummonTentacleAction
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Abilities.Goliath;

public sealed class GoliathSummonTentacleAction : 
  WorldTargetActionEvent,
  ISerializationGenerated<GoliathSummonTentacleAction>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId EntityId = EntProtoId.op_Implicit("EffectGoliathTentacleSpawn");
  [DataField(null, false, 1, false, false, null)]
  public List<Direction> OffsetDirections = new List<Direction>()
  {
    (Direction) 4,
    (Direction) 0,
    (Direction) 2,
    (Direction) 6
  };
  [DataField(null, false, 1, false, false, null)]
  public int ExtraSpawns = 3;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GoliathSummonTentacleAction target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    WorldTargetActionEvent target1 = (WorldTargetActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GoliathSummonTentacleAction) target1;
    if (serialization.TryCustomCopy<GoliathSummonTentacleAction>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId entProtoId = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.EntityId, ref entProtoId, hookCtx, false, context))
      entProtoId = serialization.CreateCopy<EntProtoId>(this.EntityId, hookCtx, context, false);
    target.EntityId = entProtoId;
    List<Direction> directionList = (List<Direction>) null;
    if (this.OffsetDirections == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<Direction>>(this.OffsetDirections, ref directionList, hookCtx, true, context))
      directionList = serialization.CreateCopy<List<Direction>>(this.OffsetDirections, hookCtx, context, false);
    target.OffsetDirections = directionList;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.ExtraSpawns, ref num, hookCtx, false, context))
      num = this.ExtraSpawns;
    target.ExtraSpawns = num;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GoliathSummonTentacleAction target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref WorldTargetActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GoliathSummonTentacleAction target1 = (GoliathSummonTentacleAction) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (WorldTargetActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GoliathSummonTentacleAction target1 = (GoliathSummonTentacleAction) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual GoliathSummonTentacleAction WorldTargetActionEvent.Instantiate()
  {
    return new GoliathSummonTentacleAction();
  }
}
