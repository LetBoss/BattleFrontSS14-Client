// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Evacuation.EvacuationProgressComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
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
namespace Content.Shared._RMC14.Evacuation;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedEvacuationSystem)})]
public sealed class EvacuationProgressComponent : 
  Component,
  ISerializationGenerated<EvacuationProgressComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DropShipCrashed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool StartAnnounced;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double Progress;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double Required = 100.0;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UpdateEvery = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextUpdate;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int AnnounceEvery = 25;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int NextAnnounce;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<EntityUid, bool> LastPower = new Dictionary<EntityUid, bool>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EvacuationProgressComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EvacuationProgressComponent) target1;
    if (serialization.TryCustomCopy<EvacuationProgressComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.DropShipCrashed, ref target3, hookCtx, false, context))
      target3 = this.DropShipCrashed;
    target.DropShipCrashed = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.StartAnnounced, ref target4, hookCtx, false, context))
      target4 = this.StartAnnounced;
    target.StartAnnounced = target4;
    double target5 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.Progress, ref target5, hookCtx, false, context))
      target5 = this.Progress;
    target.Progress = target5;
    double target6 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.Required, ref target6, hookCtx, false, context))
      target6 = this.Required;
    target.Required = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateEvery, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.UpdateEvery, hookCtx, context);
    target.UpdateEvery = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextUpdate, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.NextUpdate, hookCtx, context);
    target.NextUpdate = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.AnnounceEvery, ref target9, hookCtx, false, context))
      target9 = this.AnnounceEvery;
    target.AnnounceEvery = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.NextAnnounce, ref target10, hookCtx, false, context))
      target10 = this.NextAnnounce;
    target.NextAnnounce = target10;
    Dictionary<EntityUid, bool> target11 = (Dictionary<EntityUid, bool>) null;
    if (this.LastPower == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntityUid, bool>>(this.LastPower, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<Dictionary<EntityUid, bool>>(this.LastPower, hookCtx, context);
    target.LastPower = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EvacuationProgressComponent target,
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
    EvacuationProgressComponent target1 = (EvacuationProgressComponent) target;
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
    EvacuationProgressComponent target1 = (EvacuationProgressComponent) target;
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
    EvacuationProgressComponent target1 = (EvacuationProgressComponent) target;
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
  virtual EvacuationProgressComponent Component.Instantiate() => new EvacuationProgressComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EvacuationProgressComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EvacuationProgressComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<EvacuationProgressComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      EvacuationProgressComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextUpdate += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EvacuationProgressComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public bool DropShipCrashed;
    public bool StartAnnounced;
    public double Progress;
    public double Required;
    public TimeSpan UpdateEvery;
    public TimeSpan NextUpdate;
    public int AnnounceEvery;
    public int NextAnnounce;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EvacuationProgressComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EvacuationProgressComponent, ComponentGetState>(new ComponentEventRefHandler<EvacuationProgressComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EvacuationProgressComponent, ComponentHandleState>(new ComponentEventRefHandler<EvacuationProgressComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      EvacuationProgressComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new EvacuationProgressComponent.EvacuationProgressComponent_AutoState()
      {
        Enabled = component.Enabled,
        DropShipCrashed = component.DropShipCrashed,
        StartAnnounced = component.StartAnnounced,
        Progress = component.Progress,
        Required = component.Required,
        UpdateEvery = component.UpdateEvery,
        NextUpdate = component.NextUpdate,
        AnnounceEvery = component.AnnounceEvery,
        NextAnnounce = component.NextAnnounce
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EvacuationProgressComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is EvacuationProgressComponent.EvacuationProgressComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.DropShipCrashed = current.DropShipCrashed;
      component.StartAnnounced = current.StartAnnounced;
      component.Progress = current.Progress;
      component.Required = current.Required;
      component.UpdateEvery = current.UpdateEvery;
      component.NextUpdate = current.NextUpdate;
      component.AnnounceEvery = current.AnnounceEvery;
      component.NextAnnounce = current.NextAnnounce;
    }
  }
}
