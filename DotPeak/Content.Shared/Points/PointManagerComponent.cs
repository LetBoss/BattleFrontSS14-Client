// Decompiled with JetBrains decompiler
// Type: Content.Shared.Points.PointManagerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Points;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedPointSystem)})]
public sealed class PointManagerComponent : 
  Component,
  ISerializationGenerated<PointManagerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<NetUserId, FixedPoint2> Points = new Dictionary<NetUserId, FixedPoint2>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FormattedMessage Scoreboard = new FormattedMessage();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PointManagerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PointManagerComponent) target1;
    if (serialization.TryCustomCopy<PointManagerComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<NetUserId, FixedPoint2> target2 = (Dictionary<NetUserId, FixedPoint2>) null;
    if (this.Points == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<NetUserId, FixedPoint2>>(this.Points, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<NetUserId, FixedPoint2>>(this.Points, hookCtx, context);
    target.Points = target2;
    FormattedMessage target3 = (FormattedMessage) null;
    if (this.Scoreboard == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<FormattedMessage>(this.Scoreboard, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FormattedMessage>(this.Scoreboard, hookCtx, context);
    target.Scoreboard = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PointManagerComponent target,
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
    PointManagerComponent target1 = (PointManagerComponent) target;
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
    PointManagerComponent target1 = (PointManagerComponent) target;
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
    PointManagerComponent target1 = (PointManagerComponent) target;
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
  virtual PointManagerComponent Component.Instantiate() => new PointManagerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PointManagerComponent_AutoState : IComponentState
  {
    public Dictionary<NetUserId, FixedPoint2> Points;
    public FormattedMessage Scoreboard;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PointManagerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PointManagerComponent, ComponentGetState>(new ComponentEventRefHandler<PointManagerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PointManagerComponent, ComponentHandleState>(new ComponentEventRefHandler<PointManagerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PointManagerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PointManagerComponent.PointManagerComponent_AutoState()
      {
        Points = component.Points,
        Scoreboard = component.Scoreboard
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PointManagerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PointManagerComponent.PointManagerComponent_AutoState current))
        return;
      component.Points = current.Points == null ? (Dictionary<NetUserId, FixedPoint2>) null : new Dictionary<NetUserId, FixedPoint2>((IDictionary<NetUserId, FixedPoint2>) current.Points);
      component.Scoreboard = current.Scoreboard;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, PointManagerComponent>(uid, component, ref args1);
    }
  }
}
