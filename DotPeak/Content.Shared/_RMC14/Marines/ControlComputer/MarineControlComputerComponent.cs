// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.ControlComputer.MarineControlComputerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Marines.ControlComputer;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedMarineControlComputerSystem)})]
public sealed class MarineControlComputerComponent : 
  Component,
  ISerializationGenerated<MarineControlComputerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Evacuating;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanEvacuate;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? EvacuationCancelledSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Announcements/ARES/evacuate_cancelled.ogg", new AudioParams?(AudioParams.Default.WithVolume(-5f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ToggleCooldown = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LastToggle;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, GibbedMarineInfo> GibbedMarines = new Dictionary<string, GibbedMarineInfo>();
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? LastShipAnnouncement;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ShipAnnouncementCooldown = TimeSpan.FromSeconds(30L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MarineControlComputerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MarineControlComputerComponent) target1;
    if (serialization.TryCustomCopy<MarineControlComputerComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Evacuating, ref target2, hookCtx, false, context))
      target2 = this.Evacuating;
    target.Evacuating = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanEvacuate, ref target3, hookCtx, false, context))
      target3 = this.CanEvacuate;
    target.CanEvacuate = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EvacuationCancelledSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.EvacuationCancelledSound, hookCtx, context);
    target.EvacuationCancelledSound = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ToggleCooldown, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.ToggleCooldown, hookCtx, context);
    target.ToggleCooldown = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastToggle, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.LastToggle, hookCtx, context);
    target.LastToggle = target6;
    Dictionary<string, GibbedMarineInfo> target7 = (Dictionary<string, GibbedMarineInfo>) null;
    if (this.GibbedMarines == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, GibbedMarineInfo>>(this.GibbedMarines, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<Dictionary<string, GibbedMarineInfo>>(this.GibbedMarines, hookCtx, context);
    target.GibbedMarines = target7;
    TimeSpan? target8 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LastShipAnnouncement, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan?>(this.LastShipAnnouncement, hookCtx, context);
    target.LastShipAnnouncement = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ShipAnnouncementCooldown, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.ShipAnnouncementCooldown, hookCtx, context);
    target.ShipAnnouncementCooldown = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MarineControlComputerComponent target,
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
    MarineControlComputerComponent target1 = (MarineControlComputerComponent) target;
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
    MarineControlComputerComponent target1 = (MarineControlComputerComponent) target;
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
    MarineControlComputerComponent target1 = (MarineControlComputerComponent) target;
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
  virtual MarineControlComputerComponent Component.Instantiate()
  {
    return new MarineControlComputerComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MarineControlComputerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MarineControlComputerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<MarineControlComputerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      MarineControlComputerComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.LastShipAnnouncement.HasValue)
        component.LastShipAnnouncement = new TimeSpan?(component.LastShipAnnouncement.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MarineControlComputerComponent_AutoState : IComponentState
  {
    public bool Evacuating;
    public bool CanEvacuate;
    public 
    #nullable enable
    SoundSpecifier? EvacuationCancelledSound;
    public TimeSpan ToggleCooldown;
    public TimeSpan LastToggle;
    public Dictionary<string, GibbedMarineInfo> GibbedMarines;
    public TimeSpan? LastShipAnnouncement;
    public TimeSpan ShipAnnouncementCooldown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MarineControlComputerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MarineControlComputerComponent, ComponentGetState>(new ComponentEventRefHandler<MarineControlComputerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MarineControlComputerComponent, ComponentHandleState>(new ComponentEventRefHandler<MarineControlComputerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MarineControlComputerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MarineControlComputerComponent.MarineControlComputerComponent_AutoState()
      {
        Evacuating = component.Evacuating,
        CanEvacuate = component.CanEvacuate,
        EvacuationCancelledSound = component.EvacuationCancelledSound,
        ToggleCooldown = component.ToggleCooldown,
        LastToggle = component.LastToggle,
        GibbedMarines = component.GibbedMarines,
        LastShipAnnouncement = component.LastShipAnnouncement,
        ShipAnnouncementCooldown = component.ShipAnnouncementCooldown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MarineControlComputerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MarineControlComputerComponent.MarineControlComputerComponent_AutoState current))
        return;
      component.Evacuating = current.Evacuating;
      component.CanEvacuate = current.CanEvacuate;
      component.EvacuationCancelledSound = current.EvacuationCancelledSound;
      component.ToggleCooldown = current.ToggleCooldown;
      component.LastToggle = current.LastToggle;
      component.GibbedMarines = current.GibbedMarines == null ? (Dictionary<string, GibbedMarineInfo>) null : new Dictionary<string, GibbedMarineInfo>((IDictionary<string, GibbedMarineInfo>) current.GibbedMarines);
      component.LastShipAnnouncement = current.LastShipAnnouncement;
      component.ShipAnnouncementCooldown = current.ShipAnnouncementCooldown;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, MarineControlComputerComponent>(uid, component, ref args1);
    }
  }
}
