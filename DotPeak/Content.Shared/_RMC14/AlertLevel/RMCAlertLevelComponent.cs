// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.AlertLevel.RMCAlertLevelComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Radio;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.AlertLevel;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCAlertLevelSystem)})]
public sealed class RMCAlertLevelComponent : 
  Component,
  ISerializationGenerated<RMCAlertLevelComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCAlertLevels Level;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? BlueElevatedSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/AI/code_blue_elevated.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? BlueElevatedMessage = (LocId?) "rmc-alert-level-blue-elevated";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? BlueLoweredSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/AI/code_blue_lowered.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? BlueLoweredMessage = (LocId?) "rmc-alert-level-blue-lowered";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? GreenSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/AI/code_green.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? GreenMessage = (LocId?) "rmc-alert-level-green";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? RedElevatedSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/AI/code_red_elevated.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? RedElevatedMessage = (LocId?) "rmc-alert-level-red-elevated";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? RedLoweredSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/AI/code_red_lowered.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? RedLoweredMessage = (LocId?) "rmc-alert-level-red-lowered";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? DeltaAnnouncement = (LocId?) "rmc-announcement-delta";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? DeltaSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Misc/gamma.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<RadioChannelPrototype> RadioChannel = (ProtoId<RadioChannelPrototype>) "MarineCommon";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCAlertLevelComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCAlertLevelComponent) target1;
    if (serialization.TryCustomCopy<RMCAlertLevelComponent>(this, ref target, hookCtx, false, context))
      return;
    RMCAlertLevels target2 = RMCAlertLevels.Green;
    if (!serialization.TryCustomCopy<RMCAlertLevels>(this.Level, ref target2, hookCtx, false, context))
      target2 = this.Level;
    target.Level = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BlueElevatedSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.BlueElevatedSound, hookCtx, context);
    target.BlueElevatedSound = target3;
    LocId? target4 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.BlueElevatedMessage, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId?>(this.BlueElevatedMessage, hookCtx, context);
    target.BlueElevatedMessage = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BlueLoweredSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.BlueLoweredSound, hookCtx, context);
    target.BlueLoweredSound = target5;
    LocId? target6 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.BlueLoweredMessage, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId?>(this.BlueLoweredMessage, hookCtx, context);
    target.BlueLoweredMessage = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.GreenSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.GreenSound, hookCtx, context);
    target.GreenSound = target7;
    LocId? target8 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.GreenMessage, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<LocId?>(this.GreenMessage, hookCtx, context);
    target.GreenMessage = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RedElevatedSound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.RedElevatedSound, hookCtx, context);
    target.RedElevatedSound = target9;
    LocId? target10 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.RedElevatedMessage, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<LocId?>(this.RedElevatedMessage, hookCtx, context);
    target.RedElevatedMessage = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RedLoweredSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.RedLoweredSound, hookCtx, context);
    target.RedLoweredSound = target11;
    LocId? target12 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.RedLoweredMessage, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<LocId?>(this.RedLoweredMessage, hookCtx, context);
    target.RedLoweredMessage = target12;
    LocId? target13 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.DeltaAnnouncement, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<LocId?>(this.DeltaAnnouncement, hookCtx, context);
    target.DeltaAnnouncement = target13;
    SoundSpecifier target14 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DeltaSound, ref target14, hookCtx, true, context))
      target14 = serialization.CreateCopy<SoundSpecifier>(this.DeltaSound, hookCtx, context);
    target.DeltaSound = target14;
    ProtoId<RadioChannelPrototype> target15 = new ProtoId<RadioChannelPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<RadioChannelPrototype>>(this.RadioChannel, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<ProtoId<RadioChannelPrototype>>(this.RadioChannel, hookCtx, context);
    target.RadioChannel = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCAlertLevelComponent target,
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
    RMCAlertLevelComponent target1 = (RMCAlertLevelComponent) target;
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
    RMCAlertLevelComponent target1 = (RMCAlertLevelComponent) target;
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
    RMCAlertLevelComponent target1 = (RMCAlertLevelComponent) target;
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
  virtual RMCAlertLevelComponent Component.Instantiate() => new RMCAlertLevelComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCAlertLevelComponent_AutoState : IComponentState
  {
    public RMCAlertLevels Level;
    public SoundSpecifier? BlueElevatedSound;
    public LocId? BlueElevatedMessage;
    public SoundSpecifier? BlueLoweredSound;
    public LocId? BlueLoweredMessage;
    public SoundSpecifier? GreenSound;
    public LocId? GreenMessage;
    public SoundSpecifier? RedElevatedSound;
    public LocId? RedElevatedMessage;
    public SoundSpecifier? RedLoweredSound;
    public LocId? RedLoweredMessage;
    public LocId? DeltaAnnouncement;
    public SoundSpecifier? DeltaSound;
    public ProtoId<RadioChannelPrototype> RadioChannel;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCAlertLevelComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCAlertLevelComponent, ComponentGetState>(new ComponentEventRefHandler<RMCAlertLevelComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCAlertLevelComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCAlertLevelComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCAlertLevelComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCAlertLevelComponent.RMCAlertLevelComponent_AutoState()
      {
        Level = component.Level,
        BlueElevatedSound = component.BlueElevatedSound,
        BlueElevatedMessage = component.BlueElevatedMessage,
        BlueLoweredSound = component.BlueLoweredSound,
        BlueLoweredMessage = component.BlueLoweredMessage,
        GreenSound = component.GreenSound,
        GreenMessage = component.GreenMessage,
        RedElevatedSound = component.RedElevatedSound,
        RedElevatedMessage = component.RedElevatedMessage,
        RedLoweredSound = component.RedLoweredSound,
        RedLoweredMessage = component.RedLoweredMessage,
        DeltaAnnouncement = component.DeltaAnnouncement,
        DeltaSound = component.DeltaSound,
        RadioChannel = component.RadioChannel
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCAlertLevelComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCAlertLevelComponent.RMCAlertLevelComponent_AutoState current))
        return;
      component.Level = current.Level;
      component.BlueElevatedSound = current.BlueElevatedSound;
      component.BlueElevatedMessage = current.BlueElevatedMessage;
      component.BlueLoweredSound = current.BlueLoweredSound;
      component.BlueLoweredMessage = current.BlueLoweredMessage;
      component.GreenSound = current.GreenSound;
      component.GreenMessage = current.GreenMessage;
      component.RedElevatedSound = current.RedElevatedSound;
      component.RedElevatedMessage = current.RedElevatedMessage;
      component.RedLoweredSound = current.RedLoweredSound;
      component.RedLoweredMessage = current.RedLoweredMessage;
      component.DeltaAnnouncement = current.DeltaAnnouncement;
      component.DeltaSound = current.DeltaSound;
      component.RadioChannel = current.RadioChannel;
    }
  }
}
