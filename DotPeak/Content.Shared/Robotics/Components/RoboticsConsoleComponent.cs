// Decompiled with JetBrains decompiler
// Type: Content.Shared.Robotics.Components.RoboticsConsoleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Radio;
using Content.Shared.Robotics.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Robotics.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedRoboticsConsoleSystem)})]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
public sealed class RoboticsConsoleComponent : 
  Component,
  ISerializationGenerated<RoboticsConsoleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, CyborgControlData> Cyborgs = new Dictionary<string, CyborgControlData>();
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Timeout = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<RadioChannelPrototype> RadioChannel = (ProtoId<RadioChannelPrototype>) "Science";
  [DataField(null, false, 1, false, false, null)]
  public LocId DestroyMessage = (LocId) "robotics-console-cyborg-destroying";
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DestroyCooldown = TimeSpan.FromSeconds(30L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextDestroy = TimeSpan.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RoboticsConsoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RoboticsConsoleComponent) target1;
    if (serialization.TryCustomCopy<RoboticsConsoleComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, CyborgControlData> target2 = (Dictionary<string, CyborgControlData>) null;
    if (this.Cyborgs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, CyborgControlData>>(this.Cyborgs, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<string, CyborgControlData>>(this.Cyborgs, hookCtx, context);
    target.Cyborgs = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Timeout, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Timeout, hookCtx, context);
    target.Timeout = target3;
    ProtoId<RadioChannelPrototype> target4 = new ProtoId<RadioChannelPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<RadioChannelPrototype>>(this.RadioChannel, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<RadioChannelPrototype>>(this.RadioChannel, hookCtx, context);
    target.RadioChannel = target4;
    LocId target5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.DestroyMessage, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId>(this.DestroyMessage, hookCtx, context);
    target.DestroyMessage = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DestroyCooldown, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.DestroyCooldown, hookCtx, context);
    target.DestroyCooldown = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextDestroy, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.NextDestroy, hookCtx, context);
    target.NextDestroy = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RoboticsConsoleComponent target,
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
    RoboticsConsoleComponent target1 = (RoboticsConsoleComponent) target;
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
    RoboticsConsoleComponent target1 = (RoboticsConsoleComponent) target;
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
    RoboticsConsoleComponent target1 = (RoboticsConsoleComponent) target;
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
  virtual RoboticsConsoleComponent Component.Instantiate() => new RoboticsConsoleComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RoboticsConsoleComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RoboticsConsoleComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<RoboticsConsoleComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      RoboticsConsoleComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextDestroy += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RoboticsConsoleComponent_AutoState : IComponentState
  {
    public TimeSpan NextDestroy;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RoboticsConsoleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RoboticsConsoleComponent, ComponentGetState>(new ComponentEventRefHandler<RoboticsConsoleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RoboticsConsoleComponent, ComponentHandleState>(new ComponentEventRefHandler<RoboticsConsoleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      RoboticsConsoleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RoboticsConsoleComponent.RoboticsConsoleComponent_AutoState()
      {
        NextDestroy = component.NextDestroy
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RoboticsConsoleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RoboticsConsoleComponent.RoboticsConsoleComponent_AutoState current))
        return;
      component.NextDestroy = current.NextDestroy;
    }
  }
}
