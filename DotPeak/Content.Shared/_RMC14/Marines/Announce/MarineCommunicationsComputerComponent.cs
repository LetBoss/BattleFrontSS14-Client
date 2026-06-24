// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Announce.MarineCommunicationsComputerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Marines.Announce;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedMarineAnnounceSystem)})]
public sealed class MarineCommunicationsComputerComponent : 
  Component,
  ISerializationGenerated<MarineCommunicationsComputerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(30L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? LastAnnouncement;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> AnnounceSkill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillLeadership";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int AnnounceSkillLevel = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> OverwatchSkill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillOverwatch";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int OverwatchSkillLevel = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanCreateEcho = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanGiveMedals;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanInitiateEvac;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? AnnounceName;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MarineCommunicationsComputerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MarineCommunicationsComputerComponent) target1;
    if (serialization.TryCustomCopy<MarineCommunicationsComputerComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target2;
    TimeSpan? target3 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LastAnnouncement, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan?>(this.LastAnnouncement, hookCtx, context);
    target.LastAnnouncement = target3;
    EntProtoId<SkillDefinitionComponent> target4 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.AnnounceSkill, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.AnnounceSkill, hookCtx, context);
    target.AnnounceSkill = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.AnnounceSkillLevel, ref target5, hookCtx, false, context))
      target5 = this.AnnounceSkillLevel;
    target.AnnounceSkillLevel = target5;
    EntProtoId<SkillDefinitionComponent> target6 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.OverwatchSkill, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.OverwatchSkill, hookCtx, context);
    target.OverwatchSkill = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.OverwatchSkillLevel, ref target7, hookCtx, false, context))
      target7 = this.OverwatchSkillLevel;
    target.OverwatchSkillLevel = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanCreateEcho, ref target8, hookCtx, false, context))
      target8 = this.CanCreateEcho;
    target.CanCreateEcho = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanGiveMedals, ref target9, hookCtx, false, context))
      target9 = this.CanGiveMedals;
    target.CanGiveMedals = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanInitiateEvac, ref target10, hookCtx, false, context))
      target10 = this.CanInitiateEvac;
    target.CanInitiateEvac = target10;
    string target11 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.AnnounceName, ref target11, hookCtx, false, context))
      target11 = this.AnnounceName;
    target.AnnounceName = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MarineCommunicationsComputerComponent target,
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
    MarineCommunicationsComputerComponent target1 = (MarineCommunicationsComputerComponent) target;
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
    MarineCommunicationsComputerComponent target1 = (MarineCommunicationsComputerComponent) target;
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
    MarineCommunicationsComputerComponent target1 = (MarineCommunicationsComputerComponent) target;
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
  virtual MarineCommunicationsComputerComponent Component.Instantiate()
  {
    return new MarineCommunicationsComputerComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MarineCommunicationsComputerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MarineCommunicationsComputerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<MarineCommunicationsComputerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      MarineCommunicationsComputerComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.LastAnnouncement.HasValue)
        component.LastAnnouncement = new TimeSpan?(component.LastAnnouncement.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MarineCommunicationsComputerComponent_AutoState : IComponentState
  {
    public TimeSpan Cooldown;
    public TimeSpan? LastAnnouncement;
    public EntProtoId<
    #nullable enable
    SkillDefinitionComponent> AnnounceSkill;
    public int AnnounceSkillLevel;
    public EntProtoId<SkillDefinitionComponent> OverwatchSkill;
    public int OverwatchSkillLevel;
    public bool CanCreateEcho;
    public bool CanGiveMedals;
    public bool CanInitiateEvac;
    public string? AnnounceName;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MarineCommunicationsComputerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MarineCommunicationsComputerComponent, ComponentGetState>(new ComponentEventRefHandler<MarineCommunicationsComputerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MarineCommunicationsComputerComponent, ComponentHandleState>(new ComponentEventRefHandler<MarineCommunicationsComputerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MarineCommunicationsComputerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MarineCommunicationsComputerComponent.MarineCommunicationsComputerComponent_AutoState()
      {
        Cooldown = component.Cooldown,
        LastAnnouncement = component.LastAnnouncement,
        AnnounceSkill = component.AnnounceSkill,
        AnnounceSkillLevel = component.AnnounceSkillLevel,
        OverwatchSkill = component.OverwatchSkill,
        OverwatchSkillLevel = component.OverwatchSkillLevel,
        CanCreateEcho = component.CanCreateEcho,
        CanGiveMedals = component.CanGiveMedals,
        CanInitiateEvac = component.CanInitiateEvac,
        AnnounceName = component.AnnounceName
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MarineCommunicationsComputerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MarineCommunicationsComputerComponent.MarineCommunicationsComputerComponent_AutoState current))
        return;
      component.Cooldown = current.Cooldown;
      component.LastAnnouncement = current.LastAnnouncement;
      component.AnnounceSkill = current.AnnounceSkill;
      component.AnnounceSkillLevel = current.AnnounceSkillLevel;
      component.OverwatchSkill = current.OverwatchSkill;
      component.OverwatchSkillLevel = current.OverwatchSkillLevel;
      component.CanCreateEcho = current.CanCreateEcho;
      component.CanGiveMedals = current.CanGiveMedals;
      component.CanInitiateEvac = current.CanInitiateEvac;
      component.AnnounceName = current.AnnounceName;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, MarineCommunicationsComputerComponent>(uid, component, ref args1);
    }
  }
}
