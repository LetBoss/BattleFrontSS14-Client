// Decompiled with JetBrains decompiler
// Type: Content.Shared.Doors.Components.TurnstileComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Doors.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
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
namespace Content.Shared.Doors.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedTurnstileSystem)})]
public sealed class TurnstileComponent : 
  Component,
  ISerializationGenerated<TurnstileComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? ProcessWhitelist;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextResistTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> CollideExceptions;
  [DataField(null, false, 1, false, false, null)]
  public string DefaultState;
  [DataField(null, false, 1, false, false, null)]
  public string SpinState;
  [DataField(null, false, 1, false, false, null)]
  public string DenyState;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? TurnSound;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? DenySound;

  public TurnstileComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Machines/airlock_deny.ogg");
    soundPathSpecifier.Params = new AudioParams()
    {
      Volume = -7f
    };
    this.DenySound = (SoundSpecifier) soundPathSpecifier;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TurnstileComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TurnstileComponent) target1;
    if (serialization.TryCustomCopy<TurnstileComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.ProcessWhitelist, ref target2, hookCtx, false, context))
    {
      if (this.ProcessWhitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.ProcessWhitelist, ref target2, hookCtx, context);
    }
    target.ProcessWhitelist = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextResistTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.NextResistTime, hookCtx, context);
    target.NextResistTime = target3;
    HashSet<EntityUid> target4 = (HashSet<EntityUid>) null;
    if (this.CollideExceptions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.CollideExceptions, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<HashSet<EntityUid>>(this.CollideExceptions, hookCtx, context);
    target.CollideExceptions = target4;
    string target5 = (string) null;
    if (this.DefaultState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DefaultState, ref target5, hookCtx, false, context))
      target5 = this.DefaultState;
    target.DefaultState = target5;
    string target6 = (string) null;
    if (this.SpinState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SpinState, ref target6, hookCtx, false, context))
      target6 = this.SpinState;
    target.SpinState = target6;
    string target7 = (string) null;
    if (this.DenyState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DenyState, ref target7, hookCtx, false, context))
      target7 = this.DenyState;
    target.DenyState = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TurnSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.TurnSound, hookCtx, context);
    target.TurnSound = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DenySound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.DenySound, hookCtx, context);
    target.DenySound = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TurnstileComponent target,
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
    TurnstileComponent target1 = (TurnstileComponent) target;
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
    TurnstileComponent target1 = (TurnstileComponent) target;
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
    TurnstileComponent target1 = (TurnstileComponent) target;
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
  virtual TurnstileComponent Component.Instantiate() => new TurnstileComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TurnstileComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TurnstileComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<TurnstileComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      TurnstileComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextResistTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TurnstileComponent_AutoState : IComponentState
  {
    public TimeSpan NextResistTime;
    public 
    #nullable enable
    HashSet<NetEntity> CollideExceptions;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TurnstileComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TurnstileComponent, ComponentGetState>(new ComponentEventRefHandler<TurnstileComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TurnstileComponent, ComponentHandleState>(new ComponentEventRefHandler<TurnstileComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      TurnstileComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new TurnstileComponent.TurnstileComponent_AutoState()
      {
        NextResistTime = component.NextResistTime,
        CollideExceptions = this.GetNetEntitySet(component.CollideExceptions)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TurnstileComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TurnstileComponent.TurnstileComponent_AutoState current))
        return;
      component.NextResistTime = current.NextResistTime;
      this.EnsureEntitySet<TurnstileComponent>(current.CollideExceptions, uid, component.CollideExceptions);
    }
  }
}
