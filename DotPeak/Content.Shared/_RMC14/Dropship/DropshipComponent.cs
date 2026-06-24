// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.DropshipComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Doors.Components;
using Content.Shared.Shuttles.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
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
namespace Content.Shared._RMC14.Dropship;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedDropshipSystem)})]
public sealed class DropshipComponent : 
  Component,
  ISerializationGenerated<DropshipComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FTLState State;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Destination;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? DepartureLocation;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Crashed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier LocalHijackSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/Shuttle/queen_alarm.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier MarineHijackSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Announcements/ARES/hijack.ogg", new AudioParams?(AudioParams.Default.WithVolume(-5f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier GeneralQuartersSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Announcements/ARES/GQfullcall.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier UnidentifledlifesignsSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Announcements/ARES/unidentified_lifesigns.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LockCooldown = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<DoorLocation, TimeSpan> LastLocked = new Dictionary<DoorLocation, TimeSpan>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> AttachmentPoints = new HashSet<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? RechargeTime;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? HijackLandAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId FireId = (EntProtoId) "RMCTileFire";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int FireRange = 11;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier CrashWarningSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Announcements/ARES/dropship_emergency.ogg", new AudioParams?(AudioParams.Default.WithVolume(-5f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier CrashSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Dropship/dropship_crash.ogg", new AudioParams?(AudioParams.Default.WithVolume(-1f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier IncomingSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Dropship/dropship_incoming.ogg", new AudioParams?(AudioParams.Default.WithVolume(-1f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AnnouncedCrash;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DidIncomingSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DidExplosion;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AnnounceCrashTime = TimeSpan.FromSeconds(23L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan PlayIncomingSoundTime = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ExplodeTime = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan CancelFlightTime = TimeSpan.FromSeconds(10L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DropshipComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DropshipComponent) target1;
    if (serialization.TryCustomCopy<DropshipComponent>(this, ref target, hookCtx, false, context))
      return;
    FTLState target2 = FTLState.Invalid;
    if (!serialization.TryCustomCopy<FTLState>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target.State = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Destination, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Destination, hookCtx, context);
    target.Destination = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.DepartureLocation, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.DepartureLocation, hookCtx, context);
    target.DepartureLocation = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Crashed, ref target5, hookCtx, false, context))
      target5 = this.Crashed;
    target.Crashed = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.LocalHijackSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LocalHijackSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.LocalHijackSound, hookCtx, context);
    target.LocalHijackSound = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (this.MarineHijackSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.MarineHijackSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.MarineHijackSound, hookCtx, context);
    target.MarineHijackSound = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (this.GeneralQuartersSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.GeneralQuartersSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.GeneralQuartersSound, hookCtx, context);
    target.GeneralQuartersSound = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (this.UnidentifledlifesignsSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UnidentifledlifesignsSound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.UnidentifledlifesignsSound, hookCtx, context);
    target.UnidentifledlifesignsSound = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LockCooldown, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.LockCooldown, hookCtx, context);
    target.LockCooldown = target10;
    Dictionary<DoorLocation, TimeSpan> target11 = (Dictionary<DoorLocation, TimeSpan>) null;
    if (this.LastLocked == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<DoorLocation, TimeSpan>>(this.LastLocked, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<Dictionary<DoorLocation, TimeSpan>>(this.LastLocked, hookCtx, context);
    target.LastLocked = target11;
    HashSet<EntityUid> target12 = (HashSet<EntityUid>) null;
    if (this.AttachmentPoints == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.AttachmentPoints, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<HashSet<EntityUid>>(this.AttachmentPoints, hookCtx, context);
    target.AttachmentPoints = target12;
    TimeSpan? target13 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.RechargeTime, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan?>(this.RechargeTime, hookCtx, context);
    target.RechargeTime = target13;
    TimeSpan? target14 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.HijackLandAt, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan?>(this.HijackLandAt, hookCtx, context);
    target.HijackLandAt = target14;
    EntProtoId target15 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.FireId, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<EntProtoId>(this.FireId, hookCtx, context);
    target.FireId = target15;
    int target16 = 0;
    if (!serialization.TryCustomCopy<int>(this.FireRange, ref target16, hookCtx, false, context))
      target16 = this.FireRange;
    target.FireRange = target16;
    SoundSpecifier target17 = (SoundSpecifier) null;
    if (this.CrashWarningSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CrashWarningSound, ref target17, hookCtx, true, context))
      target17 = serialization.CreateCopy<SoundSpecifier>(this.CrashWarningSound, hookCtx, context);
    target.CrashWarningSound = target17;
    SoundSpecifier target18 = (SoundSpecifier) null;
    if (this.CrashSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CrashSound, ref target18, hookCtx, true, context))
      target18 = serialization.CreateCopy<SoundSpecifier>(this.CrashSound, hookCtx, context);
    target.CrashSound = target18;
    SoundSpecifier target19 = (SoundSpecifier) null;
    if (this.IncomingSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.IncomingSound, ref target19, hookCtx, true, context))
      target19 = serialization.CreateCopy<SoundSpecifier>(this.IncomingSound, hookCtx, context);
    target.IncomingSound = target19;
    bool target20 = false;
    if (!serialization.TryCustomCopy<bool>(this.AnnouncedCrash, ref target20, hookCtx, false, context))
      target20 = this.AnnouncedCrash;
    target.AnnouncedCrash = target20;
    bool target21 = false;
    if (!serialization.TryCustomCopy<bool>(this.DidIncomingSound, ref target21, hookCtx, false, context))
      target21 = this.DidIncomingSound;
    target.DidIncomingSound = target21;
    bool target22 = false;
    if (!serialization.TryCustomCopy<bool>(this.DidExplosion, ref target22, hookCtx, false, context))
      target22 = this.DidExplosion;
    target.DidExplosion = target22;
    TimeSpan target23 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AnnounceCrashTime, ref target23, hookCtx, false, context))
      target23 = serialization.CreateCopy<TimeSpan>(this.AnnounceCrashTime, hookCtx, context);
    target.AnnounceCrashTime = target23;
    TimeSpan target24 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PlayIncomingSoundTime, ref target24, hookCtx, false, context))
      target24 = serialization.CreateCopy<TimeSpan>(this.PlayIncomingSoundTime, hookCtx, context);
    target.PlayIncomingSoundTime = target24;
    TimeSpan target25 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExplodeTime, ref target25, hookCtx, false, context))
      target25 = serialization.CreateCopy<TimeSpan>(this.ExplodeTime, hookCtx, context);
    target.ExplodeTime = target25;
    TimeSpan target26 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CancelFlightTime, ref target26, hookCtx, false, context))
      target26 = serialization.CreateCopy<TimeSpan>(this.CancelFlightTime, hookCtx, context);
    target.CancelFlightTime = target26;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DropshipComponent target,
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
    DropshipComponent target1 = (DropshipComponent) target;
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
    DropshipComponent target1 = (DropshipComponent) target;
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
    DropshipComponent target1 = (DropshipComponent) target;
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
  virtual DropshipComponent Component.Instantiate() => new DropshipComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<DropshipComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      DropshipComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.HijackLandAt.HasValue)
        component.HijackLandAt = new TimeSpan?(component.HijackLandAt.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DropshipComponent_AutoState : IComponentState
  {
    public FTLState State;
    public NetEntity? Destination;
    public NetEntity? DepartureLocation;
    public bool Crashed;
    public 
    #nullable enable
    SoundSpecifier LocalHijackSound;
    public SoundSpecifier MarineHijackSound;
    public SoundSpecifier GeneralQuartersSound;
    public SoundSpecifier UnidentifledlifesignsSound;
    public TimeSpan LockCooldown;
    public Dictionary<DoorLocation, TimeSpan> LastLocked;
    public HashSet<NetEntity> AttachmentPoints;
    public TimeSpan? RechargeTime;
    public TimeSpan? HijackLandAt;
    public EntProtoId FireId;
    public int FireRange;
    public SoundSpecifier CrashWarningSound;
    public SoundSpecifier CrashSound;
    public SoundSpecifier IncomingSound;
    public bool AnnouncedCrash;
    public bool DidIncomingSound;
    public bool DidExplosion;
    public TimeSpan AnnounceCrashTime;
    public TimeSpan PlayIncomingSoundTime;
    public TimeSpan ExplodeTime;
    public TimeSpan CancelFlightTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipComponent, ComponentGetState>(new ComponentEventRefHandler<DropshipComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DropshipComponent, ComponentHandleState>(new ComponentEventRefHandler<DropshipComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, DropshipComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new DropshipComponent.DropshipComponent_AutoState()
      {
        State = component.State,
        Destination = this.GetNetEntity(component.Destination),
        DepartureLocation = this.GetNetEntity(component.DepartureLocation),
        Crashed = component.Crashed,
        LocalHijackSound = component.LocalHijackSound,
        MarineHijackSound = component.MarineHijackSound,
        GeneralQuartersSound = component.GeneralQuartersSound,
        UnidentifledlifesignsSound = component.UnidentifledlifesignsSound,
        LockCooldown = component.LockCooldown,
        LastLocked = component.LastLocked,
        AttachmentPoints = this.GetNetEntitySet(component.AttachmentPoints),
        RechargeTime = component.RechargeTime,
        HijackLandAt = component.HijackLandAt,
        FireId = component.FireId,
        FireRange = component.FireRange,
        CrashWarningSound = component.CrashWarningSound,
        CrashSound = component.CrashSound,
        IncomingSound = component.IncomingSound,
        AnnouncedCrash = component.AnnouncedCrash,
        DidIncomingSound = component.DidIncomingSound,
        DidExplosion = component.DidExplosion,
        AnnounceCrashTime = component.AnnounceCrashTime,
        PlayIncomingSoundTime = component.PlayIncomingSoundTime,
        ExplodeTime = component.ExplodeTime,
        CancelFlightTime = component.CancelFlightTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DropshipComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DropshipComponent.DropshipComponent_AutoState current))
        return;
      component.State = current.State;
      component.Destination = this.EnsureEntity<DropshipComponent>(current.Destination, uid);
      component.DepartureLocation = this.EnsureEntity<DropshipComponent>(current.DepartureLocation, uid);
      component.Crashed = current.Crashed;
      component.LocalHijackSound = current.LocalHijackSound;
      component.MarineHijackSound = current.MarineHijackSound;
      component.GeneralQuartersSound = current.GeneralQuartersSound;
      component.UnidentifledlifesignsSound = current.UnidentifledlifesignsSound;
      component.LockCooldown = current.LockCooldown;
      component.LastLocked = current.LastLocked == null ? (Dictionary<DoorLocation, TimeSpan>) null : new Dictionary<DoorLocation, TimeSpan>((IDictionary<DoorLocation, TimeSpan>) current.LastLocked);
      this.EnsureEntitySet<DropshipComponent>(current.AttachmentPoints, uid, component.AttachmentPoints);
      component.RechargeTime = current.RechargeTime;
      component.HijackLandAt = current.HijackLandAt;
      component.FireId = current.FireId;
      component.FireRange = current.FireRange;
      component.CrashWarningSound = current.CrashWarningSound;
      component.CrashSound = current.CrashSound;
      component.IncomingSound = current.IncomingSound;
      component.AnnouncedCrash = current.AnnouncedCrash;
      component.DidIncomingSound = current.DidIncomingSound;
      component.DidExplosion = current.DidExplosion;
      component.AnnounceCrashTime = current.AnnounceCrashTime;
      component.PlayIncomingSoundTime = current.PlayIncomingSoundTime;
      component.ExplodeTime = current.ExplodeTime;
      component.CancelFlightTime = current.CancelFlightTime;
    }
  }
}
