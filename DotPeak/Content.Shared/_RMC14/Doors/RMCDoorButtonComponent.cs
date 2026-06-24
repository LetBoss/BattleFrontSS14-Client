// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Doors.RMCDoorButtonComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Doors;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (CMDoorSystem)})]
public sealed class RMCDoorButtonComponent : 
  Component,
  ISerializationGenerated<RMCDoorButtonComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? Id;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastUse;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string OffState = "doorctrl";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string OnState = "doorctrl1";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string DeniedState = "doorctrl-denied";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? MinimumRoundTimeToPress;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Used;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UseOnlyOnce;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId NoTimeMessage = (LocId) "rmc-machines-button-cannot-be-lifted-weya";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId AlreadyUsedMessage = (LocId) "rmc-machines-button-already-lifted-weya";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? MarineAnnouncement;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId MarineAnnouncementAuthor = (LocId) "rmc-announcement-author";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCDoorButtonComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCDoorButtonComponent) target1;
    if (serialization.TryCustomCopy<RMCDoorButtonComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Id, ref target2, hookCtx, false, context))
      target2 = this.Id;
    target.Id = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastUse, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.LastUse, hookCtx, context);
    target.LastUse = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target4;
    string target5 = (string) null;
    if (this.OffState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OffState, ref target5, hookCtx, false, context))
      target5 = this.OffState;
    target.OffState = target5;
    string target6 = (string) null;
    if (this.OnState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OnState, ref target6, hookCtx, false, context))
      target6 = this.OnState;
    target.OnState = target6;
    string target7 = (string) null;
    if (this.DeniedState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DeniedState, ref target7, hookCtx, false, context))
      target7 = this.DeniedState;
    target.DeniedState = target7;
    TimeSpan? target8 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.MinimumRoundTimeToPress, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan?>(this.MinimumRoundTimeToPress, hookCtx, context);
    target.MinimumRoundTimeToPress = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.Used, ref target9, hookCtx, false, context))
      target9 = this.Used;
    target.Used = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseOnlyOnce, ref target10, hookCtx, false, context))
      target10 = this.UseOnlyOnce;
    target.UseOnlyOnce = target10;
    LocId target11 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.NoTimeMessage, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<LocId>(this.NoTimeMessage, hookCtx, context);
    target.NoTimeMessage = target11;
    LocId target12 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.AlreadyUsedMessage, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<LocId>(this.AlreadyUsedMessage, hookCtx, context);
    target.AlreadyUsedMessage = target12;
    LocId? target13 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.MarineAnnouncement, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<LocId?>(this.MarineAnnouncement, hookCtx, context);
    target.MarineAnnouncement = target13;
    LocId target14 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.MarineAnnouncementAuthor, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<LocId>(this.MarineAnnouncementAuthor, hookCtx, context);
    target.MarineAnnouncementAuthor = target14;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCDoorButtonComponent target,
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
    RMCDoorButtonComponent target1 = (RMCDoorButtonComponent) target;
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
    RMCDoorButtonComponent target1 = (RMCDoorButtonComponent) target;
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
    RMCDoorButtonComponent target1 = (RMCDoorButtonComponent) target;
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
  virtual RMCDoorButtonComponent Component.Instantiate() => new RMCDoorButtonComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCDoorButtonComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCDoorButtonComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<RMCDoorButtonComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      RMCDoorButtonComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastUse += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCDoorButtonComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    string? Id;
    public TimeSpan LastUse;
    public TimeSpan Cooldown;
    public string OffState;
    public string OnState;
    public string DeniedState;
    public TimeSpan? MinimumRoundTimeToPress;
    public bool Used;
    public bool UseOnlyOnce;
    public LocId NoTimeMessage;
    public LocId AlreadyUsedMessage;
    public LocId? MarineAnnouncement;
    public LocId MarineAnnouncementAuthor;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCDoorButtonComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCDoorButtonComponent, ComponentGetState>(new ComponentEventRefHandler<RMCDoorButtonComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCDoorButtonComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCDoorButtonComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCDoorButtonComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCDoorButtonComponent.RMCDoorButtonComponent_AutoState()
      {
        Id = component.Id,
        LastUse = component.LastUse,
        Cooldown = component.Cooldown,
        OffState = component.OffState,
        OnState = component.OnState,
        DeniedState = component.DeniedState,
        MinimumRoundTimeToPress = component.MinimumRoundTimeToPress,
        Used = component.Used,
        UseOnlyOnce = component.UseOnlyOnce,
        NoTimeMessage = component.NoTimeMessage,
        AlreadyUsedMessage = component.AlreadyUsedMessage,
        MarineAnnouncement = component.MarineAnnouncement,
        MarineAnnouncementAuthor = component.MarineAnnouncementAuthor
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCDoorButtonComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCDoorButtonComponent.RMCDoorButtonComponent_AutoState current))
        return;
      component.Id = current.Id;
      component.LastUse = current.LastUse;
      component.Cooldown = current.Cooldown;
      component.OffState = current.OffState;
      component.OnState = current.OnState;
      component.DeniedState = current.DeniedState;
      component.MinimumRoundTimeToPress = current.MinimumRoundTimeToPress;
      component.Used = current.Used;
      component.UseOnlyOnce = current.UseOnlyOnce;
      component.NoTimeMessage = current.NoTimeMessage;
      component.AlreadyUsedMessage = current.AlreadyUsedMessage;
      component.MarineAnnouncement = current.MarineAnnouncement;
      component.MarineAnnouncementAuthor = current.MarineAnnouncementAuthor;
    }
  }
}
