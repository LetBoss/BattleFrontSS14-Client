// Decompiled with JetBrains decompiler
// Type: Content.Shared.Throwing.ThrownItemComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Throwing;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
public sealed class ThrownItemComponent : 
  Component,
  ISerializationGenerated<ThrownItemComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Animate = true;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public EntityUid? Thrower;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public TimeSpan? ThrownTime;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? LandTime;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public bool Landed;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public bool PlayLandSound;
  [DataField(null, false, 1, false, false, null)]
  public Vector2? OriginalScale;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ThrownItemComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ThrownItemComponent) target1;
    if (serialization.TryCustomCopy<ThrownItemComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Animate, ref target2, hookCtx, false, context))
      target2 = this.Animate;
    target.Animate = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Thrower, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Thrower, hookCtx, context);
    target.Thrower = target3;
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.ThrownTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.ThrownTime, hookCtx, context);
    target.ThrownTime = target4;
    TimeSpan? target5 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LandTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan?>(this.LandTime, hookCtx, context);
    target.LandTime = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Landed, ref target6, hookCtx, false, context))
      target6 = this.Landed;
    target.Landed = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.PlayLandSound, ref target7, hookCtx, false, context))
      target7 = this.PlayLandSound;
    target.PlayLandSound = target7;
    Vector2? target8 = new Vector2?();
    if (!serialization.TryCustomCopy<Vector2?>(this.OriginalScale, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<Vector2?>(this.OriginalScale, hookCtx, context);
    target.OriginalScale = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ThrownItemComponent target,
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
    ThrownItemComponent target1 = (ThrownItemComponent) target;
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
    ThrownItemComponent target1 = (ThrownItemComponent) target;
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
    ThrownItemComponent target1 = (ThrownItemComponent) target;
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
  virtual ThrownItemComponent Component.Instantiate() => new ThrownItemComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ThrownItemComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ThrownItemComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<ThrownItemComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      ThrownItemComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.LandTime.HasValue)
        component.LandTime = new TimeSpan?(component.LandTime.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ThrownItemComponent_AutoState : IComponentState
  {
    public bool Animate;
    public NetEntity? Thrower;
    public TimeSpan? ThrownTime;
    public TimeSpan? LandTime;
    public bool Landed;
    public bool PlayLandSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ThrownItemComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ThrownItemComponent, ComponentGetState>(new ComponentEventRefHandler<ThrownItemComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ThrownItemComponent, ComponentHandleState>(new ComponentEventRefHandler<ThrownItemComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      ThrownItemComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ThrownItemComponent.ThrownItemComponent_AutoState()
      {
        Animate = component.Animate,
        Thrower = this.GetNetEntity(component.Thrower),
        ThrownTime = component.ThrownTime,
        LandTime = component.LandTime,
        Landed = component.Landed,
        PlayLandSound = component.PlayLandSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ThrownItemComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ThrownItemComponent.ThrownItemComponent_AutoState current))
        return;
      component.Animate = current.Animate;
      component.Thrower = this.EnsureEntity<ThrownItemComponent>(current.Thrower, uid);
      component.ThrownTime = current.ThrownTime;
      component.LandTime = current.LandTime;
      component.Landed = current.Landed;
      component.PlayLandSound = current.PlayLandSound;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, ThrownItemComponent>(uid, component, ref args1);
    }
  }
}
