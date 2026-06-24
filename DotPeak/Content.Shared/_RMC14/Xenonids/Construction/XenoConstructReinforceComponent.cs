// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.XenoConstructReinforceComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedXenoConstructReinforceSystem)})]
public sealed class XenoConstructReinforceComponent : 
  Component,
  ISerializationGenerated<XenoConstructReinforceComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromSeconds(15L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? EndAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ReinforceAmount = (FixedPoint2) 0;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoConstructReinforceComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoConstructReinforceComponent) target1;
    if (serialization.TryCustomCopy<XenoConstructReinforceComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target2;
    TimeSpan? target3 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.EndAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan?>(this.EndAt, hookCtx, context);
    target.EndAt = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ReinforceAmount, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.ReinforceAmount, hookCtx, context);
    target.ReinforceAmount = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoConstructReinforceComponent target,
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
    XenoConstructReinforceComponent target1 = (XenoConstructReinforceComponent) target;
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
    XenoConstructReinforceComponent target1 = (XenoConstructReinforceComponent) target;
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
    XenoConstructReinforceComponent target1 = (XenoConstructReinforceComponent) target;
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
  virtual XenoConstructReinforceComponent Component.Instantiate()
  {
    return new XenoConstructReinforceComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoConstructReinforceComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoConstructReinforceComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoConstructReinforceComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoConstructReinforceComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.EndAt.HasValue)
        component.EndAt = new TimeSpan?(component.EndAt.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoConstructReinforceComponent_AutoState : IComponentState
  {
    public TimeSpan Duration;
    public TimeSpan? EndAt;
    public FixedPoint2 ReinforceAmount;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoConstructReinforceComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoConstructReinforceComponent, ComponentGetState>(new ComponentEventRefHandler<XenoConstructReinforceComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoConstructReinforceComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoConstructReinforceComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      XenoConstructReinforceComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoConstructReinforceComponent.XenoConstructReinforceComponent_AutoState()
      {
        Duration = component.Duration,
        EndAt = component.EndAt,
        ReinforceAmount = component.ReinforceAmount
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoConstructReinforceComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoConstructReinforceComponent.XenoConstructReinforceComponent_AutoState current))
        return;
      component.Duration = current.Duration;
      component.EndAt = current.EndAt;
      component.ReinforceAmount = current.ReinforceAmount;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, XenoConstructReinforceComponent>(uid, component, ref args1);
    }
  }
}
