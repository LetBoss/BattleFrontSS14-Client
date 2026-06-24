// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mining.Components.MiningScannerViewerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Mining.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (MiningScannerSystem)})]
public sealed class MiningScannerViewerComponent : 
  Component,
  ISerializationGenerated<MiningScannerViewerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [AutoNetworkedField]
  public float ViewRange;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AnimationDuration;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan PingDelay;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextPingTime;
  [DataField(null, false, 1, false, false, null)]
  public EntityCoordinates? LastPingLocation;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? PingSound;
  [DataField(null, false, 1, false, false, null)]
  public bool QueueRemoval;

  public MiningScannerViewerComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Machines/sonar-ping.ogg");
    soundPathSpecifier.Params = new AudioParams()
    {
      Volume = -3f
    };
    this.PingSound = (SoundSpecifier) soundPathSpecifier;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MiningScannerViewerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MiningScannerViewerComponent) target1;
    if (serialization.TryCustomCopy<MiningScannerViewerComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ViewRange, ref target2, hookCtx, false, context))
      target2 = this.ViewRange;
    target.ViewRange = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AnimationDuration, ref target3, hookCtx, false, context))
      target3 = this.AnimationDuration;
    target.AnimationDuration = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PingDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.PingDelay, hookCtx, context);
    target.PingDelay = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextPingTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.NextPingTime, hookCtx, context);
    target.NextPingTime = target5;
    EntityCoordinates? target6 = new EntityCoordinates?();
    if (!serialization.TryCustomCopy<EntityCoordinates?>(this.LastPingLocation, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityCoordinates?>(this.LastPingLocation, hookCtx, context);
    target.LastPingLocation = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PingSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.PingSound, hookCtx, context);
    target.PingSound = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.QueueRemoval, ref target8, hookCtx, false, context))
      target8 = this.QueueRemoval;
    target.QueueRemoval = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MiningScannerViewerComponent target,
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
    MiningScannerViewerComponent target1 = (MiningScannerViewerComponent) target;
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
    MiningScannerViewerComponent target1 = (MiningScannerViewerComponent) target;
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
    MiningScannerViewerComponent target1 = (MiningScannerViewerComponent) target;
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
  virtual MiningScannerViewerComponent Component.Instantiate()
  {
    return new MiningScannerViewerComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MiningScannerViewerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MiningScannerViewerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<MiningScannerViewerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      MiningScannerViewerComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextPingTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MiningScannerViewerComponent_AutoState : IComponentState
  {
    public float ViewRange;
    public float AnimationDuration;
    public TimeSpan PingDelay;
    public TimeSpan NextPingTime;
    public 
    #nullable enable
    SoundSpecifier? PingSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MiningScannerViewerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MiningScannerViewerComponent, ComponentGetState>(new ComponentEventRefHandler<MiningScannerViewerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MiningScannerViewerComponent, ComponentHandleState>(new ComponentEventRefHandler<MiningScannerViewerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MiningScannerViewerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MiningScannerViewerComponent.MiningScannerViewerComponent_AutoState()
      {
        ViewRange = component.ViewRange,
        AnimationDuration = component.AnimationDuration,
        PingDelay = component.PingDelay,
        NextPingTime = component.NextPingTime,
        PingSound = component.PingSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MiningScannerViewerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MiningScannerViewerComponent.MiningScannerViewerComponent_AutoState current))
        return;
      component.ViewRange = current.ViewRange;
      component.AnimationDuration = current.AnimationDuration;
      component.PingDelay = current.PingDelay;
      component.NextPingTime = current.NextPingTime;
      component.PingSound = current.PingSound;
    }
  }
}
