// Decompiled with JetBrains decompiler
// Type: Content.Shared.DoAfter.DoAfter
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;

#nullable enable
namespace Content.Shared.DoAfter;

[NetSerializable]
[DataDefinition]
[Access(new Type[] {typeof (SharedDoAfterSystem)})]
[Serializable]
public sealed class DoAfter : ISerializationGenerated<Content.Shared.DoAfter.DoAfter>, ISerializationGenerated
{
  [DataField("index", false, 1, true, false, null)]
  public ushort Index;
  [IncludeDataField(false, 1, false, null)]
  public DoAfterArgs Args;
  [DataField("startTime", false, 1, true, false, typeof (TimeOffsetSerializer))]
  public TimeSpan StartTime;
  [DataField("cancelledTime", false, 1, true, false, typeof (TimeOffsetSerializer))]
  public TimeSpan? CancelledTime;
  [DataField("lastEffectSpawnTime", false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan? LastEffectSpawnTime;
  [DataField("completed", false, 1, false, false, null)]
  public bool Completed;
  [DataField("userPosition", false, 1, false, false, null)]
  [NonSerialized]
  public EntityCoordinates UserPosition;
  public NetCoordinates NetUserPosition;
  [DataField("targetDistance", false, 1, false, false, null)]
  public float TargetDistance;
  [DataField("activeHand", false, 1, false, false, null)]
  public string? InitialHand;
  [DataField("activeItem", false, 1, false, false, null)]
  [NonSerialized]
  public EntityUid? InitialItem;
  public NetEntity? NetInitialItem;
  [NonSerialized]
  public object? AttemptEvent;

  public DoAfterId Id => new DoAfterId(this.Args.User, this.Index);

  public bool Cancelled => this.CancelledTime.HasValue;

  private DoAfter()
  {
  }

  public DoAfter(ushort index, DoAfterArgs args, TimeSpan startTime)
  {
    this.Index = index;
    this.Args = args;
    this.StartTime = startTime;
  }

  public DoAfter(IEntityManager entManager, Content.Shared.DoAfter.DoAfter other)
  {
    this.Index = other.Index;
    this.Args = new DoAfterArgs(other.Args);
    this.StartTime = other.StartTime;
    this.CancelledTime = other.CancelledTime;
    this.Completed = other.Completed;
    this.UserPosition = other.UserPosition;
    this.TargetDistance = other.TargetDistance;
    this.InitialHand = other.InitialHand;
    this.InitialItem = other.InitialItem;
    this.NetUserPosition = other.NetUserPosition;
    this.NetInitialItem = other.NetInitialItem;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Content.Shared.DoAfter.DoAfter target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<Content.Shared.DoAfter.DoAfter>(this, ref target, hookCtx, false, context))
      return;
    ushort num1 = 0;
    if (!serialization.TryCustomCopy<ushort>(this.Index, ref num1, hookCtx, false, context))
      num1 = this.Index;
    target.Index = num1;
    DoAfterArgs doAfterArgs = (DoAfterArgs) null;
    if (this.Args == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DoAfterArgs>(this.Args, ref doAfterArgs, hookCtx, false, context))
    {
      if (this.Args == null)
        doAfterArgs = (DoAfterArgs) null;
      else
        serialization.CopyTo<DoAfterArgs>(this.Args, ref doAfterArgs, hookCtx, context, true);
    }
    target.Args = doAfterArgs;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StartTime, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.StartTime, hookCtx, context, false);
    target.StartTime = timeSpan;
    TimeSpan? nullable1 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.CancelledTime, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<TimeSpan?>(this.CancelledTime, hookCtx, context, false);
    target.CancelledTime = nullable1;
    TimeSpan? nullable2 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LastEffectSpawnTime, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<TimeSpan?>(this.LastEffectSpawnTime, hookCtx, context, false);
    target.LastEffectSpawnTime = nullable2;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Completed, ref flag, hookCtx, false, context))
      flag = this.Completed;
    target.Completed = flag;
    EntityCoordinates entityCoordinates = new EntityCoordinates();
    if (!serialization.TryCustomCopy<EntityCoordinates>(this.UserPosition, ref entityCoordinates, hookCtx, false, context))
      entityCoordinates = serialization.CreateCopy<EntityCoordinates>(this.UserPosition, hookCtx, context, false);
    target.UserPosition = entityCoordinates;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TargetDistance, ref num2, hookCtx, false, context))
      num2 = this.TargetDistance;
    target.TargetDistance = num2;
    string str = (string) null;
    if (!serialization.TryCustomCopy<string>(this.InitialHand, ref str, hookCtx, false, context))
      str = this.InitialHand;
    target.InitialHand = str;
    EntityUid? nullable3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.InitialItem, ref nullable3, hookCtx, false, context))
      nullable3 = serialization.CreateCopy<EntityUid?>(this.InitialItem, hookCtx, context, false);
    target.InitialItem = nullable3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Content.Shared.DoAfter.DoAfter target,
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
    Content.Shared.DoAfter.DoAfter target1 = (Content.Shared.DoAfter.DoAfter) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public Content.Shared.DoAfter.DoAfter Instantiate() => new Content.Shared.DoAfter.DoAfter();
}
