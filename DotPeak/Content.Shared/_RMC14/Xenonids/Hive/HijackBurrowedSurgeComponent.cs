// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Hive.HijackBurrowedSurgeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Hive;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class HijackBurrowedSurgeComponent : 
  Component,
  ISerializationGenerated<HijackBurrowedSurgeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextSurgeAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int PooledLarva;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SurgeEvery = TimeSpan.FromSeconds(90L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ReduceSurgeBy = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MinSurgeTime = TimeSpan.FromSeconds(30L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ResetSurgeTime = TimeSpan.FromSeconds(90L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HijackBurrowedSurgeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HijackBurrowedSurgeComponent) target1;
    if (serialization.TryCustomCopy<HijackBurrowedSurgeComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextSurgeAt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.NextSurgeAt, hookCtx, context);
    target.NextSurgeAt = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.PooledLarva, ref target3, hookCtx, false, context))
      target3 = this.PooledLarva;
    target.PooledLarva = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SurgeEvery, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.SurgeEvery, hookCtx, context);
    target.SurgeEvery = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ReduceSurgeBy, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.ReduceSurgeBy, hookCtx, context);
    target.ReduceSurgeBy = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MinSurgeTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.MinSurgeTime, hookCtx, context);
    target.MinSurgeTime = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ResetSurgeTime, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.ResetSurgeTime, hookCtx, context);
    target.ResetSurgeTime = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HijackBurrowedSurgeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HijackBurrowedSurgeComponent target1 = (HijackBurrowedSurgeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HijackBurrowedSurgeComponent target1 = (HijackBurrowedSurgeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HijackBurrowedSurgeComponent target1 = (HijackBurrowedSurgeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual HijackBurrowedSurgeComponent Component.Instantiate()
  {
    return new HijackBurrowedSurgeComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HijackBurrowedSurgeComponent_AutoState : IComponentState
  {
    public TimeSpan NextSurgeAt;
    public int PooledLarva;
    public TimeSpan SurgeEvery;
    public TimeSpan ReduceSurgeBy;
    public TimeSpan MinSurgeTime;
    public TimeSpan ResetSurgeTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HijackBurrowedSurgeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HijackBurrowedSurgeComponent, ComponentGetState>(new ComponentEventRefHandler<HijackBurrowedSurgeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HijackBurrowedSurgeComponent, ComponentHandleState>(new ComponentEventRefHandler<HijackBurrowedSurgeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      HijackBurrowedSurgeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new HijackBurrowedSurgeComponent.HijackBurrowedSurgeComponent_AutoState()
      {
        NextSurgeAt = component.NextSurgeAt,
        PooledLarva = component.PooledLarva,
        SurgeEvery = component.SurgeEvery,
        ReduceSurgeBy = component.ReduceSurgeBy,
        MinSurgeTime = component.MinSurgeTime,
        ResetSurgeTime = component.ResetSurgeTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HijackBurrowedSurgeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HijackBurrowedSurgeComponent.HijackBurrowedSurgeComponent_AutoState current))
        return;
      component.NextSurgeAt = current.NextSurgeAt;
      component.PooledLarva = current.PooledLarva;
      component.SurgeEvery = current.SurgeEvery;
      component.ReduceSurgeBy = current.ReduceSurgeBy;
      component.MinSurgeTime = current.MinSurgeTime;
      component.ResetSurgeTime = current.ResetSurgeTime;
    }
  }
}
