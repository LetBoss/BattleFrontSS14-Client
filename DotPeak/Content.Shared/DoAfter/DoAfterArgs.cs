// Decompiled with JetBrains decompiler
// Type: Content.Shared.DoAfter.DoAfterArgs
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared.DoAfter;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class DoAfterArgs : ISerializationGenerated<DoAfterArgs>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool RootEntity;
  [DataField("user", false, 1, true, false, null)]
  [NonSerialized]
  public EntityUid User;
  public NetEntity NetUser;
  [DataField(null, false, 1, true, false, null)]
  public TimeSpan Delay;
  [DataField(null, false, 1, false, false, null)]
  [NonSerialized]
  public EntityUid? Target;
  public NetEntity? NetTarget;
  [DataField("using", false, 1, false, false, null)]
  [NonSerialized]
  public EntityUid? Used;
  public NetEntity? NetUsed;
  [DataField(null, false, 1, false, false, null)]
  public bool Hidden;
  [DataField(null, false, 1, false, false, null)]
  public bool ForceVisible;
  [DataField(null, false, 1, true, false, null)]
  public DoAfterEvent Event;
  [DataField("attemptEventFrequency", false, 1, false, false, null)]
  public AttemptFrequency AttemptFrequency;
  [DataField(null, false, 1, false, false, null)]
  [NonSerialized]
  public EntityUid? EventTarget;
  public NetEntity? NetEventTarget;
  [DataField(null, false, 1, false, false, null)]
  public bool Broadcast;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? TargetEffect;
  [DataField(null, false, 1, false, false, null)]
  public bool NeedHand;
  [DataField(null, false, 1, false, false, null)]
  public bool BreakOnHandChange = true;
  [DataField(null, false, 1, false, false, null)]
  public bool BreakOnDropItem = true;
  [DataField(null, false, 1, false, false, null)]
  public bool BreakOnMove;
  [DataField(null, false, 1, false, false, null)]
  public bool BreakOnWeightlessMove = true;
  [DataField(null, false, 1, false, false, null)]
  public float MovementThreshold = 0.3f;
  [DataField(null, false, 1, false, false, null)]
  public float? DistanceThreshold;
  [DataField(null, false, 1, false, false, null)]
  public bool BreakOnDamage;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 DamageThreshold = (FixedPoint2) 1;
  [DataField(null, false, 1, false, false, null)]
  public bool RequireCanInteract = true;
  [DataField(null, false, 1, false, false, null)]
  public bool BreakOnRest = true;
  [DataField(null, false, 1, false, false, null)]
  public bool LagCompensated;
  [DataField(null, false, 1, false, false, null)]
  public bool BlockDuplicate = true;
  [DataField(null, false, 1, false, false, null)]
  public bool CancelDuplicate = true;
  [DataField(null, false, 1, false, false, null)]
  public DuplicateConditions DuplicateCondition = DuplicateConditions.All;
  [Obsolete("Use checkEvent instead")]
  [NonSerialized]
  public Func<bool>? ExtraCheck;

  public DoAfterArgs(
    IEntityManager entManager,
    EntityUid user,
    TimeSpan delay,
    DoAfterEvent @event,
    EntityUid? eventTarget,
    EntityUid? target = null,
    EntityUid? used = null)
  {
    this.User = user;
    this.Delay = delay;
    this.Target = target;
    this.Used = used;
    this.EventTarget = eventTarget;
    this.Event = @event;
    this.NetUser = entManager.GetNetEntity(this.User, (MetaDataComponent) null);
    this.NetTarget = entManager.GetNetEntity(this.Target, (MetaDataComponent) null);
    this.NetUsed = entManager.GetNetEntity(this.Used, (MetaDataComponent) null);
  }

  private DoAfterArgs()
  {
  }

  public DoAfterArgs(
    IEntityManager entManager,
    EntityUid user,
    float seconds,
    DoAfterEvent @event,
    EntityUid? eventTarget,
    EntityUid? target = null,
    EntityUid? used = null)
    : this(entManager, user, TimeSpan.FromSeconds((double) seconds), @event, eventTarget, target, used)
  {
  }

  public DoAfterArgs(DoAfterArgs other)
  {
    this.User = other.User;
    this.Delay = other.Delay;
    this.Target = other.Target;
    this.Used = other.Used;
    this.Hidden = other.Hidden;
    this.EventTarget = other.EventTarget;
    this.Broadcast = other.Broadcast;
    this.NeedHand = other.NeedHand;
    this.BreakOnHandChange = other.BreakOnHandChange;
    this.BreakOnDropItem = other.BreakOnDropItem;
    this.BreakOnMove = other.BreakOnMove;
    this.BreakOnWeightlessMove = other.BreakOnWeightlessMove;
    this.MovementThreshold = other.MovementThreshold;
    this.DistanceThreshold = other.DistanceThreshold;
    this.BreakOnDamage = other.BreakOnDamage;
    this.DamageThreshold = other.DamageThreshold;
    this.RequireCanInteract = other.RequireCanInteract;
    this.AttemptFrequency = other.AttemptFrequency;
    this.BlockDuplicate = other.BlockDuplicate;
    this.CancelDuplicate = other.CancelDuplicate;
    this.DuplicateCondition = other.DuplicateCondition;
    this.ForceVisible = other.ForceVisible;
    this.BreakOnRest = other.BreakOnRest;
    this.LagCompensated = other.LagCompensated;
    this.RootEntity = other.RootEntity;
    this.NetUser = other.NetUser;
    this.NetTarget = other.NetTarget;
    this.NetUsed = other.NetUsed;
    this.NetEventTarget = other.NetEventTarget;
    this.Event = other.Event.Clone();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DoAfterArgs target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<DoAfterArgs>(this, ref target, hookCtx, false, context))
      return;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.RootEntity, ref flag1, hookCtx, false, context))
      flag1 = this.RootEntity;
    target.RootEntity = flag1;
    EntityUid entityUid = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.User, ref entityUid, hookCtx, false, context))
      entityUid = serialization.CreateCopy<EntityUid>(this.User, hookCtx, context, false);
    target.User = entityUid;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context, false);
    target.Delay = timeSpan;
    EntityUid? nullable1 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Target, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<EntityUid?>(this.Target, hookCtx, context, false);
    target.Target = nullable1;
    EntityUid? nullable2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Used, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<EntityUid?>(this.Used, hookCtx, context, false);
    target.Used = nullable2;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Hidden, ref flag2, hookCtx, false, context))
      flag2 = this.Hidden;
    target.Hidden = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.ForceVisible, ref flag3, hookCtx, false, context))
      flag3 = this.ForceVisible;
    target.ForceVisible = flag3;
    DoAfterEvent doAfterEvent = (DoAfterEvent) null;
    if (this.Event == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DoAfterEvent>(this.Event, ref doAfterEvent, hookCtx, true, context))
      doAfterEvent = serialization.CreateCopy<DoAfterEvent>(this.Event, hookCtx, context, false);
    target.Event = doAfterEvent;
    AttemptFrequency attemptFrequency = AttemptFrequency.Never;
    if (!serialization.TryCustomCopy<AttemptFrequency>(this.AttemptFrequency, ref attemptFrequency, hookCtx, false, context))
      attemptFrequency = this.AttemptFrequency;
    target.AttemptFrequency = attemptFrequency;
    EntityUid? nullable3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.EventTarget, ref nullable3, hookCtx, false, context))
      nullable3 = serialization.CreateCopy<EntityUid?>(this.EventTarget, hookCtx, context, false);
    target.EventTarget = nullable3;
    bool flag4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Broadcast, ref flag4, hookCtx, false, context))
      flag4 = this.Broadcast;
    target.Broadcast = flag4;
    EntProtoId? nullable4 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.TargetEffect, ref nullable4, hookCtx, false, context))
      nullable4 = serialization.CreateCopy<EntProtoId?>(this.TargetEffect, hookCtx, context, false);
    target.TargetEffect = nullable4;
    bool flag5 = false;
    if (!serialization.TryCustomCopy<bool>(this.NeedHand, ref flag5, hookCtx, false, context))
      flag5 = this.NeedHand;
    target.NeedHand = flag5;
    bool flag6 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnHandChange, ref flag6, hookCtx, false, context))
      flag6 = this.BreakOnHandChange;
    target.BreakOnHandChange = flag6;
    bool flag7 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnDropItem, ref flag7, hookCtx, false, context))
      flag7 = this.BreakOnDropItem;
    target.BreakOnDropItem = flag7;
    bool flag8 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnMove, ref flag8, hookCtx, false, context))
      flag8 = this.BreakOnMove;
    target.BreakOnMove = flag8;
    bool flag9 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnWeightlessMove, ref flag9, hookCtx, false, context))
      flag9 = this.BreakOnWeightlessMove;
    target.BreakOnWeightlessMove = flag9;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MovementThreshold, ref num, hookCtx, false, context))
      num = this.MovementThreshold;
    target.MovementThreshold = num;
    float? nullable5 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.DistanceThreshold, ref nullable5, hookCtx, false, context))
      nullable5 = this.DistanceThreshold;
    target.DistanceThreshold = nullable5;
    bool flag10 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnDamage, ref flag10, hookCtx, false, context))
      flag10 = this.BreakOnDamage;
    target.BreakOnDamage = flag10;
    FixedPoint2 fixedPoint2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.DamageThreshold, ref fixedPoint2, hookCtx, false, context))
      fixedPoint2 = serialization.CreateCopy<FixedPoint2>(this.DamageThreshold, hookCtx, context, false);
    target.DamageThreshold = fixedPoint2;
    bool flag11 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireCanInteract, ref flag11, hookCtx, false, context))
      flag11 = this.RequireCanInteract;
    target.RequireCanInteract = flag11;
    bool flag12 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnRest, ref flag12, hookCtx, false, context))
      flag12 = this.BreakOnRest;
    target.BreakOnRest = flag12;
    bool flag13 = false;
    if (!serialization.TryCustomCopy<bool>(this.LagCompensated, ref flag13, hookCtx, false, context))
      flag13 = this.LagCompensated;
    target.LagCompensated = flag13;
    bool flag14 = false;
    if (!serialization.TryCustomCopy<bool>(this.BlockDuplicate, ref flag14, hookCtx, false, context))
      flag14 = this.BlockDuplicate;
    target.BlockDuplicate = flag14;
    bool flag15 = false;
    if (!serialization.TryCustomCopy<bool>(this.CancelDuplicate, ref flag15, hookCtx, false, context))
      flag15 = this.CancelDuplicate;
    target.CancelDuplicate = flag15;
    DuplicateConditions duplicateConditions = DuplicateConditions.None;
    if (!serialization.TryCustomCopy<DuplicateConditions>(this.DuplicateCondition, ref duplicateConditions, hookCtx, false, context))
      duplicateConditions = this.DuplicateCondition;
    target.DuplicateCondition = duplicateConditions;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DoAfterArgs target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterArgs target1 = (DoAfterArgs) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public DoAfterArgs Instantiate() => new DoAfterArgs();
}
