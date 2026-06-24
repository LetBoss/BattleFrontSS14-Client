// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityTable.Conditions.PlayerCountCondition
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityTable.Conditions;

public sealed class PlayerCountCondition : 
  EntityTableCondition,
  ISerializationGenerated<PlayerCountCondition>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int Min = int.MinValue;
  [DataField(null, false, 1, false, false, null)]
  public int Max = int.MaxValue;
  private static ISharedPlayerManager? _playerManager;

  protected override bool EvaluateImplementation(
    EntityTableSelector root,
    IEntityManager entMan,
    IPrototypeManager proto,
    EntityTableContext ctx)
  {
    if (PlayerCountCondition._playerManager == null)
      PlayerCountCondition._playerManager = IoCManager.Resolve<ISharedPlayerManager>();
    int playerCount = PlayerCountCondition._playerManager.PlayerCount;
    return playerCount >= this.Min && playerCount <= this.Max;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PlayerCountCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityTableCondition target1 = (EntityTableCondition) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PlayerCountCondition) target1;
    if (serialization.TryCustomCopy<PlayerCountCondition>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Min, ref target2, hookCtx, false, context))
      target2 = this.Min;
    target.Min = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Max, ref target3, hookCtx, false, context))
      target3 = this.Max;
    target.Max = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PlayerCountCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityTableCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PlayerCountCondition target1 = (PlayerCountCondition) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityTableCondition) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PlayerCountCondition target1 = (PlayerCountCondition) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PlayerCountCondition EntityTableCondition.Instantiate() => new PlayerCountCondition();
}
