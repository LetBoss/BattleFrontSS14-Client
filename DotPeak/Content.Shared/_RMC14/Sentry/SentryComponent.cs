// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sentry.SentryComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Sentry.Laptop;
using Content.Shared.Tag;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Sentry;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SentrySystem), typeof (SharedSentryLaptopSystem)})]
public sealed class SentryComponent : 
  Component,
  ISerializationGenerated<SentryComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SentryMode Mode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DeployDelay = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UndeployDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MagazineDelay = TimeSpan.FromSeconds(7L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int DefenseCheckRange = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ScrewdriverSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/screwdriver.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillEngineer";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SkillLevel = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? MagazineSwapSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/unload.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? DeployFixture = "sentry";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle MaxDeviation = Angle.FromDegrees(75.0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<BallisticAmmoProviderComponent>? StartingMagazine = (EntProtoId<BallisticAmmoProviderComponent>?) "RMCMagazineSentry";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ContainerSlotId = "gun_magazine";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId[]? Upgrades = new EntProtoId[4]
  {
    (EntProtoId) "RMCSentrySniper",
    (EntProtoId) "RMCSentryShotgun",
    (EntProtoId) "RMCSentryMini",
    (EntProtoId) "RMCSentryOmni"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<TagPrototype>? MagazineTag = (ProtoId<TagPrototype>?) "RMCMagazineSentry";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> DelaySkill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillConstruction";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Camera;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float LowAmmoThreshold = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CriticalHealthThreshold = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LastLowAmmoAlert;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LastHealthAlert;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LastTargetAlert;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AlertCooldown = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsLocked;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SentryComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SentryComponent) target1;
    if (serialization.TryCustomCopy<SentryComponent>(this, ref target, hookCtx, false, context))
      return;
    SentryMode target2 = SentryMode.Item;
    if (!serialization.TryCustomCopy<SentryMode>(this.Mode, ref target2, hookCtx, false, context))
      target2 = this.Mode;
    target.Mode = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DeployDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.DeployDelay, hookCtx, context);
    target.DeployDelay = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UndeployDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.UndeployDelay, hookCtx, context);
    target.UndeployDelay = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MagazineDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.MagazineDelay, hookCtx, context);
    target.MagazineDelay = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.DefenseCheckRange, ref target6, hookCtx, false, context))
      target6 = this.DefenseCheckRange;
    target.DefenseCheckRange = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ScrewdriverSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.ScrewdriverSound, hookCtx, context);
    target.ScrewdriverSound = target7;
    EntProtoId<SkillDefinitionComponent> target8 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.SkillLevel, ref target9, hookCtx, false, context))
      target9 = this.SkillLevel;
    target.SkillLevel = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.MagazineSwapSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.MagazineSwapSound, hookCtx, context);
    target.MagazineSwapSound = target10;
    string target11 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.DeployFixture, ref target11, hookCtx, false, context))
      target11 = this.DeployFixture;
    target.DeployFixture = target11;
    Angle target12 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.MaxDeviation, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<Angle>(this.MaxDeviation, hookCtx, context);
    target.MaxDeviation = target12;
    EntProtoId<BallisticAmmoProviderComponent>? target13 = new EntProtoId<BallisticAmmoProviderComponent>?();
    if (!serialization.TryCustomCopy<EntProtoId<BallisticAmmoProviderComponent>?>(this.StartingMagazine, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntProtoId<BallisticAmmoProviderComponent>?>(this.StartingMagazine, hookCtx, context);
    target.StartingMagazine = target13;
    string target14 = (string) null;
    if (this.ContainerSlotId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerSlotId, ref target14, hookCtx, false, context))
      target14 = this.ContainerSlotId;
    target.ContainerSlotId = target14;
    EntProtoId[] target15 = (EntProtoId[]) null;
    if (!serialization.TryCustomCopy<EntProtoId[]>(this.Upgrades, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<EntProtoId[]>(this.Upgrades, hookCtx, context);
    target.Upgrades = target15;
    ProtoId<TagPrototype>? target16 = new ProtoId<TagPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<TagPrototype>?>(this.MagazineTag, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<ProtoId<TagPrototype>?>(this.MagazineTag, hookCtx, context);
    target.MagazineTag = target16;
    EntProtoId<SkillDefinitionComponent> target17 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.DelaySkill, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.DelaySkill, hookCtx, context);
    target.DelaySkill = target17;
    EntityUid? target18 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Camera, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<EntityUid?>(this.Camera, hookCtx, context);
    target.Camera = target18;
    float target19 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LowAmmoThreshold, ref target19, hookCtx, false, context))
      target19 = this.LowAmmoThreshold;
    target.LowAmmoThreshold = target19;
    float target20 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CriticalHealthThreshold, ref target20, hookCtx, false, context))
      target20 = this.CriticalHealthThreshold;
    target.CriticalHealthThreshold = target20;
    TimeSpan target21 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastLowAmmoAlert, ref target21, hookCtx, false, context))
      target21 = serialization.CreateCopy<TimeSpan>(this.LastLowAmmoAlert, hookCtx, context);
    target.LastLowAmmoAlert = target21;
    TimeSpan target22 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastHealthAlert, ref target22, hookCtx, false, context))
      target22 = serialization.CreateCopy<TimeSpan>(this.LastHealthAlert, hookCtx, context);
    target.LastHealthAlert = target22;
    TimeSpan target23 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastTargetAlert, ref target23, hookCtx, false, context))
      target23 = serialization.CreateCopy<TimeSpan>(this.LastTargetAlert, hookCtx, context);
    target.LastTargetAlert = target23;
    TimeSpan target24 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AlertCooldown, ref target24, hookCtx, false, context))
      target24 = serialization.CreateCopy<TimeSpan>(this.AlertCooldown, hookCtx, context);
    target.AlertCooldown = target24;
    bool target25 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsLocked, ref target25, hookCtx, false, context))
      target25 = this.IsLocked;
    target.IsLocked = target25;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SentryComponent target,
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
    SentryComponent target1 = (SentryComponent) target;
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
    SentryComponent target1 = (SentryComponent) target;
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
    SentryComponent target1 = (SentryComponent) target;
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
  virtual SentryComponent Component.Instantiate() => new SentryComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SentryComponent_AutoState : IComponentState
  {
    public SentryMode Mode;
    public TimeSpan DeployDelay;
    public TimeSpan UndeployDelay;
    public TimeSpan MagazineDelay;
    public int DefenseCheckRange;
    public SoundSpecifier? ScrewdriverSound;
    public EntProtoId<SkillDefinitionComponent> Skill;
    public int SkillLevel;
    public SoundSpecifier? MagazineSwapSound;
    public string? DeployFixture;
    public Angle MaxDeviation;
    public EntProtoId<BallisticAmmoProviderComponent>? StartingMagazine;
    public string ContainerSlotId;
    public EntProtoId[]? Upgrades;
    public ProtoId<TagPrototype>? MagazineTag;
    public EntProtoId<SkillDefinitionComponent> DelaySkill;
    public NetEntity? Camera;
    public float LowAmmoThreshold;
    public float CriticalHealthThreshold;
    public TimeSpan LastLowAmmoAlert;
    public TimeSpan LastHealthAlert;
    public TimeSpan LastTargetAlert;
    public TimeSpan AlertCooldown;
    public bool IsLocked;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SentryComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SentryComponent, ComponentGetState>(new ComponentEventRefHandler<SentryComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SentryComponent, ComponentHandleState>(new ComponentEventRefHandler<SentryComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, SentryComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new SentryComponent.SentryComponent_AutoState()
      {
        Mode = component.Mode,
        DeployDelay = component.DeployDelay,
        UndeployDelay = component.UndeployDelay,
        MagazineDelay = component.MagazineDelay,
        DefenseCheckRange = component.DefenseCheckRange,
        ScrewdriverSound = component.ScrewdriverSound,
        Skill = component.Skill,
        SkillLevel = component.SkillLevel,
        MagazineSwapSound = component.MagazineSwapSound,
        DeployFixture = component.DeployFixture,
        MaxDeviation = component.MaxDeviation,
        StartingMagazine = component.StartingMagazine,
        ContainerSlotId = component.ContainerSlotId,
        Upgrades = component.Upgrades,
        MagazineTag = component.MagazineTag,
        DelaySkill = component.DelaySkill,
        Camera = this.GetNetEntity(component.Camera),
        LowAmmoThreshold = component.LowAmmoThreshold,
        CriticalHealthThreshold = component.CriticalHealthThreshold,
        LastLowAmmoAlert = component.LastLowAmmoAlert,
        LastHealthAlert = component.LastHealthAlert,
        LastTargetAlert = component.LastTargetAlert,
        AlertCooldown = component.AlertCooldown,
        IsLocked = component.IsLocked
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SentryComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SentryComponent.SentryComponent_AutoState current))
        return;
      component.Mode = current.Mode;
      component.DeployDelay = current.DeployDelay;
      component.UndeployDelay = current.UndeployDelay;
      component.MagazineDelay = current.MagazineDelay;
      component.DefenseCheckRange = current.DefenseCheckRange;
      component.ScrewdriverSound = current.ScrewdriverSound;
      component.Skill = current.Skill;
      component.SkillLevel = current.SkillLevel;
      component.MagazineSwapSound = current.MagazineSwapSound;
      component.DeployFixture = current.DeployFixture;
      component.MaxDeviation = current.MaxDeviation;
      component.StartingMagazine = current.StartingMagazine;
      component.ContainerSlotId = current.ContainerSlotId;
      component.Upgrades = current.Upgrades;
      component.MagazineTag = current.MagazineTag;
      component.DelaySkill = current.DelaySkill;
      component.Camera = this.EnsureEntity<SentryComponent>(current.Camera, uid);
      component.LowAmmoThreshold = current.LowAmmoThreshold;
      component.CriticalHealthThreshold = current.CriticalHealthThreshold;
      component.LastLowAmmoAlert = current.LastLowAmmoAlert;
      component.LastHealthAlert = current.LastHealthAlert;
      component.LastTargetAlert = current.LastTargetAlert;
      component.AlertCooldown = current.AlertCooldown;
      component.IsLocked = current.IsLocked;
    }
  }
}
