// Decompiled with JetBrains decompiler
// Type: Content.Shared.IgnitionSource.Components.MatchstickComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Smoking;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared.IgnitionSource.Components;

[NetworkedComponent]
[RegisterComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
public sealed class MatchstickComponent : 
  Component,
  ISerializationGenerated<MatchstickComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SmokableState State;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? TimeMatchWillBurnOut;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? IgniteSound;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MatchstickComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MatchstickComponent) target1;
    if (serialization.TryCustomCopy<MatchstickComponent>(this, ref target, hookCtx, false, context))
      return;
    SmokableState target2 = SmokableState.Unlit;
    if (!serialization.TryCustomCopy<SmokableState>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target.State = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target3;
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.TimeMatchWillBurnOut, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.TimeMatchWillBurnOut, hookCtx, context);
    target.TimeMatchWillBurnOut = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.IgniteSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.IgniteSound, hookCtx, context);
    target.IgniteSound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MatchstickComponent target,
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
    MatchstickComponent target1 = (MatchstickComponent) target;
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
    MatchstickComponent target1 = (MatchstickComponent) target;
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
    MatchstickComponent target1 = (MatchstickComponent) target;
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
  virtual MatchstickComponent Component.Instantiate() => new MatchstickComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MatchstickComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MatchstickComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<MatchstickComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      MatchstickComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.TimeMatchWillBurnOut.HasValue)
        component.TimeMatchWillBurnOut = new TimeSpan?(component.TimeMatchWillBurnOut.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MatchstickComponent_AutoState : IComponentState
  {
    public SmokableState State;
    public TimeSpan Duration;
    public TimeSpan? TimeMatchWillBurnOut;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MatchstickComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MatchstickComponent, ComponentGetState>(new ComponentEventRefHandler<MatchstickComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MatchstickComponent, ComponentHandleState>(new ComponentEventRefHandler<MatchstickComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      MatchstickComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MatchstickComponent.MatchstickComponent_AutoState()
      {
        State = component.State,
        Duration = component.Duration,
        TimeMatchWillBurnOut = component.TimeMatchWillBurnOut
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MatchstickComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MatchstickComponent.MatchstickComponent_AutoState current))
        return;
      component.State = current.State;
      component.Duration = current.Duration;
      component.TimeMatchWillBurnOut = current.TimeMatchWillBurnOut;
    }
  }
}
