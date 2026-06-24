// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Telephone.RotaryPhoneComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Telephone;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedRMCTelephoneSystem)})]
public sealed class RotaryPhoneComponent : 
  Component,
  ISerializationGenerated<RotaryPhoneComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Category = "Almayer";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanDnd;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? DialingSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Phone/dial.ogg", new AudioParams?(AudioParams.Default.WithVolume(-3f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? DialingIdleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Phone/ring_outgoing.ogg", new AudioParams?(AudioParams.Default.WithVolume(-3f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ReceivingSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Phone/telephone_ring.ogg", new AudioParams?(AudioParams.Default.WithVolume(-3f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? GrabSound = (SoundSpecifier) new SoundCollectionSpecifier("RMCRadioTelephoneGrab");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? VoicemailSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/Phone/voicemail.ogg", new AudioParams?(AudioParams.Default.WithVolume(-3f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<RMCTelephoneComponent> PhoneId = (EntProtoId<RMCTelephoneComponent>) "RMCTelephone";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ContainerId = "rmc_rotary_phone_telephone";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Phone;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Sound;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastCall;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan CallCooldown = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DialingIdleDelay = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan VoicemailDelay = TimeSpan.FromSeconds(30L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan VoicemailTimeoutDelay = TimeSpan.FromSeconds(6L);
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? VoicemailSoundEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Idle;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool TryGetHolderName = true;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool NotifyAdmins { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RotaryPhoneComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RotaryPhoneComponent) target1;
    if (serialization.TryCustomCopy<RotaryPhoneComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Category == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Category, ref target2, hookCtx, false, context))
      target2 = this.Category;
    target.Category = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanDnd, ref target3, hookCtx, false, context))
      target3 = this.CanDnd;
    target.CanDnd = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DialingSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.DialingSound, hookCtx, context);
    target.DialingSound = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DialingIdleSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.DialingIdleSound, hookCtx, context);
    target.DialingIdleSound = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ReceivingSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.ReceivingSound, hookCtx, context);
    target.ReceivingSound = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.GrabSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.GrabSound, hookCtx, context);
    target.GrabSound = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.VoicemailSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.VoicemailSound, hookCtx, context);
    target.VoicemailSound = target8;
    EntProtoId<RMCTelephoneComponent> target9 = new EntProtoId<RMCTelephoneComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<RMCTelephoneComponent>>(this.PhoneId, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId<RMCTelephoneComponent>>(this.PhoneId, hookCtx, context);
    target.PhoneId = target9;
    string target10 = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref target10, hookCtx, false, context))
      target10 = this.ContainerId;
    target.ContainerId = target10;
    EntityUid? target11 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Phone, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntityUid?>(this.Phone, hookCtx, context);
    target.Phone = target11;
    EntityUid? target12 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Sound, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<EntityUid?>(this.Sound, hookCtx, context);
    target.Sound = target12;
    TimeSpan target13 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastCall, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan>(this.LastCall, hookCtx, context);
    target.LastCall = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CallCooldown, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.CallCooldown, hookCtx, context);
    target.CallCooldown = target14;
    TimeSpan target15 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DialingIdleDelay, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan>(this.DialingIdleDelay, hookCtx, context);
    target.DialingIdleDelay = target15;
    TimeSpan target16 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.VoicemailDelay, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<TimeSpan>(this.VoicemailDelay, hookCtx, context);
    target.VoicemailDelay = target16;
    TimeSpan target17 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.VoicemailTimeoutDelay, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<TimeSpan>(this.VoicemailTimeoutDelay, hookCtx, context);
    target.VoicemailTimeoutDelay = target17;
    EntityUid? target18 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.VoicemailSoundEntity, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<EntityUid?>(this.VoicemailSoundEntity, hookCtx, context);
    target.VoicemailSoundEntity = target18;
    bool target19 = false;
    if (!serialization.TryCustomCopy<bool>(this.Idle, ref target19, hookCtx, false, context))
      target19 = this.Idle;
    target.Idle = target19;
    bool target20 = false;
    if (!serialization.TryCustomCopy<bool>(this.TryGetHolderName, ref target20, hookCtx, false, context))
      target20 = this.TryGetHolderName;
    target.TryGetHolderName = target20;
    bool target21 = false;
    if (!serialization.TryCustomCopy<bool>(this.NotifyAdmins, ref target21, hookCtx, false, context))
      target21 = this.NotifyAdmins;
    target.NotifyAdmins = target21;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RotaryPhoneComponent target,
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
    RotaryPhoneComponent target1 = (RotaryPhoneComponent) target;
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
    RotaryPhoneComponent target1 = (RotaryPhoneComponent) target;
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
    RotaryPhoneComponent target1 = (RotaryPhoneComponent) target;
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
  virtual RotaryPhoneComponent Component.Instantiate() => new RotaryPhoneComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RotaryPhoneComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RotaryPhoneComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<RotaryPhoneComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      RotaryPhoneComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastCall += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RotaryPhoneComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    string Category;
    public bool CanDnd;
    public SoundSpecifier? DialingSound;
    public SoundSpecifier? DialingIdleSound;
    public SoundSpecifier? ReceivingSound;
    public SoundSpecifier? GrabSound;
    public SoundSpecifier? VoicemailSound;
    public EntProtoId<RMCTelephoneComponent> PhoneId;
    public string ContainerId;
    public NetEntity? Phone;
    public NetEntity? Sound;
    public TimeSpan LastCall;
    public TimeSpan CallCooldown;
    public TimeSpan DialingIdleDelay;
    public TimeSpan VoicemailDelay;
    public TimeSpan VoicemailTimeoutDelay;
    public bool Idle;
    public bool TryGetHolderName;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RotaryPhoneComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RotaryPhoneComponent, ComponentGetState>(new ComponentEventRefHandler<RotaryPhoneComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RotaryPhoneComponent, ComponentHandleState>(new ComponentEventRefHandler<RotaryPhoneComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RotaryPhoneComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RotaryPhoneComponent.RotaryPhoneComponent_AutoState()
      {
        Category = component.Category,
        CanDnd = component.CanDnd,
        DialingSound = component.DialingSound,
        DialingIdleSound = component.DialingIdleSound,
        ReceivingSound = component.ReceivingSound,
        GrabSound = component.GrabSound,
        VoicemailSound = component.VoicemailSound,
        PhoneId = component.PhoneId,
        ContainerId = component.ContainerId,
        Phone = this.GetNetEntity(component.Phone),
        Sound = this.GetNetEntity(component.Sound),
        LastCall = component.LastCall,
        CallCooldown = component.CallCooldown,
        DialingIdleDelay = component.DialingIdleDelay,
        VoicemailDelay = component.VoicemailDelay,
        VoicemailTimeoutDelay = component.VoicemailTimeoutDelay,
        Idle = component.Idle,
        TryGetHolderName = component.TryGetHolderName
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RotaryPhoneComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RotaryPhoneComponent.RotaryPhoneComponent_AutoState current))
        return;
      component.Category = current.Category;
      component.CanDnd = current.CanDnd;
      component.DialingSound = current.DialingSound;
      component.DialingIdleSound = current.DialingIdleSound;
      component.ReceivingSound = current.ReceivingSound;
      component.GrabSound = current.GrabSound;
      component.VoicemailSound = current.VoicemailSound;
      component.PhoneId = current.PhoneId;
      component.ContainerId = current.ContainerId;
      component.Phone = this.EnsureEntity<RotaryPhoneComponent>(current.Phone, uid);
      component.Sound = this.EnsureEntity<RotaryPhoneComponent>(current.Sound, uid);
      component.LastCall = current.LastCall;
      component.CallCooldown = current.CallCooldown;
      component.DialingIdleDelay = current.DialingIdleDelay;
      component.VoicemailDelay = current.VoicemailDelay;
      component.VoicemailTimeoutDelay = current.VoicemailTimeoutDelay;
      component.Idle = current.Idle;
      component.TryGetHolderName = current.TryGetHolderName;
    }
  }
}
