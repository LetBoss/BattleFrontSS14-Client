// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Mortar.MortarComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Mortar;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedMortarSystem)})]
public sealed class MortarComponent : 
  Component,
  ISerializationGenerated<MortarComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ContainerId = "rmc_mortar_container";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SkillWhitelist Skill = new SkillWhitelist()
  {
    All = {
      [(EntProtoId<SkillDefinitionComponent>) "RMCSkillEngineer"] = 1
    }
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DeployDelay = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TargetDelay = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DialDelay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Deployed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i Target;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i Offset;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i Dial;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FireDelay = TimeSpan.FromSeconds(0L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int TilesPerOffset = 20;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxTarget = 1000;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxDial = 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MinimumRange = 15;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaximumRange = 65;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string FixtureId = "mortar";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AnimationTime = TimeSpan.FromSeconds(0.3);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string AnimationLayer = "mortar";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string AnimationState = "mortar_m402_fire";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string DeployedState = "mortar_m402";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? DeploySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/gun_mortar_unpack.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ReloadSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/gun_mortar_reload.ogg", new AudioParams?(AudioParams.Default.WithVariation(new float?(0.4f))));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? FireSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/gun_mortar_fire.ogg", new AudioParams?(AudioParams.Default.AddVolume(4f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? Cooldown;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastFiredAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Drop = (EntProtoId) "RMCMortarKit";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int[] FireRandomOffset = new int[4]{ -1, 0, 0, 1 };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LaserLinkDelay = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool LaserTargetingMode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? LinkedLaserDesignator;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates? LaserTargetCoordinates;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsLinking;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? LaserTargetSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Binoculars/binoctarget.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ToggleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/click.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? LaserTargetWarningSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Misc/cryo_warning.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LaserTargetDelay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsTargeting;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool NeedAnnouncement;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MortarComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MortarComponent) target1;
    if (serialization.TryCustomCopy<MortarComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref target2, hookCtx, false, context))
      target2 = this.ContainerId;
    target.ContainerId = target2;
    SkillWhitelist target3 = (SkillWhitelist) null;
    if (this.Skill == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SkillWhitelist>(this.Skill, ref target3, hookCtx, false, context))
    {
      if (this.Skill == null)
        target3 = (SkillWhitelist) null;
      else
        serialization.CopyTo<SkillWhitelist>(this.Skill, ref target3, hookCtx, context, true);
    }
    target.Skill = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DeployDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.DeployDelay, hookCtx, context);
    target.DeployDelay = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TargetDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.TargetDelay, hookCtx, context);
    target.TargetDelay = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DialDelay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.DialDelay, hookCtx, context);
    target.DialDelay = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Deployed, ref target7, hookCtx, false, context))
      target7 = this.Deployed;
    target.Deployed = target7;
    Vector2i target8 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.Target, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<Vector2i>(this.Target, hookCtx, context);
    target.Target = target8;
    Vector2i target9 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.Offset, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<Vector2i>(this.Offset, hookCtx, context);
    target.Offset = target9;
    Vector2i target10 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.Dial, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<Vector2i>(this.Dial, hookCtx, context);
    target.Dial = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FireDelay, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.FireDelay, hookCtx, context);
    target.FireDelay = target11;
    int target12 = 0;
    if (!serialization.TryCustomCopy<int>(this.TilesPerOffset, ref target12, hookCtx, false, context))
      target12 = this.TilesPerOffset;
    target.TilesPerOffset = target12;
    int target13 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxTarget, ref target13, hookCtx, false, context))
      target13 = this.MaxTarget;
    target.MaxTarget = target13;
    int target14 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxDial, ref target14, hookCtx, false, context))
      target14 = this.MaxDial;
    target.MaxDial = target14;
    int target15 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinimumRange, ref target15, hookCtx, false, context))
      target15 = this.MinimumRange;
    target.MinimumRange = target15;
    int target16 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaximumRange, ref target16, hookCtx, false, context))
      target16 = this.MaximumRange;
    target.MaximumRange = target16;
    string target17 = (string) null;
    if (this.FixtureId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FixtureId, ref target17, hookCtx, false, context))
      target17 = this.FixtureId;
    target.FixtureId = target17;
    TimeSpan target18 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AnimationTime, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<TimeSpan>(this.AnimationTime, hookCtx, context);
    target.AnimationTime = target18;
    string target19 = (string) null;
    if (this.AnimationLayer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AnimationLayer, ref target19, hookCtx, false, context))
      target19 = this.AnimationLayer;
    target.AnimationLayer = target19;
    string target20 = (string) null;
    if (this.AnimationState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AnimationState, ref target20, hookCtx, false, context))
      target20 = this.AnimationState;
    target.AnimationState = target20;
    string target21 = (string) null;
    if (this.DeployedState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DeployedState, ref target21, hookCtx, false, context))
      target21 = this.DeployedState;
    target.DeployedState = target21;
    SoundSpecifier target22 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DeploySound, ref target22, hookCtx, true, context))
      target22 = serialization.CreateCopy<SoundSpecifier>(this.DeploySound, hookCtx, context);
    target.DeploySound = target22;
    SoundSpecifier target23 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ReloadSound, ref target23, hookCtx, true, context))
      target23 = serialization.CreateCopy<SoundSpecifier>(this.ReloadSound, hookCtx, context);
    target.ReloadSound = target23;
    SoundSpecifier target24 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FireSound, ref target24, hookCtx, true, context))
      target24 = serialization.CreateCopy<SoundSpecifier>(this.FireSound, hookCtx, context);
    target.FireSound = target24;
    TimeSpan? target25 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.Cooldown, ref target25, hookCtx, false, context))
      target25 = serialization.CreateCopy<TimeSpan?>(this.Cooldown, hookCtx, context);
    target.Cooldown = target25;
    TimeSpan target26 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastFiredAt, ref target26, hookCtx, false, context))
      target26 = serialization.CreateCopy<TimeSpan>(this.LastFiredAt, hookCtx, context);
    target.LastFiredAt = target26;
    EntProtoId target27 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Drop, ref target27, hookCtx, false, context))
      target27 = serialization.CreateCopy<EntProtoId>(this.Drop, hookCtx, context);
    target.Drop = target27;
    int[] target28 = (int[]) null;
    if (this.FireRandomOffset == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<int[]>(this.FireRandomOffset, ref target28, hookCtx, true, context))
      target28 = serialization.CreateCopy<int[]>(this.FireRandomOffset, hookCtx, context);
    target.FireRandomOffset = target28;
    TimeSpan target29 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LaserLinkDelay, ref target29, hookCtx, false, context))
      target29 = serialization.CreateCopy<TimeSpan>(this.LaserLinkDelay, hookCtx, context);
    target.LaserLinkDelay = target29;
    bool target30 = false;
    if (!serialization.TryCustomCopy<bool>(this.LaserTargetingMode, ref target30, hookCtx, false, context))
      target30 = this.LaserTargetingMode;
    target.LaserTargetingMode = target30;
    EntityUid? target31 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.LinkedLaserDesignator, ref target31, hookCtx, false, context))
      target31 = serialization.CreateCopy<EntityUid?>(this.LinkedLaserDesignator, hookCtx, context);
    target.LinkedLaserDesignator = target31;
    EntityCoordinates? target32 = new EntityCoordinates?();
    if (!serialization.TryCustomCopy<EntityCoordinates?>(this.LaserTargetCoordinates, ref target32, hookCtx, false, context))
      target32 = serialization.CreateCopy<EntityCoordinates?>(this.LaserTargetCoordinates, hookCtx, context);
    target.LaserTargetCoordinates = target32;
    bool target33 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsLinking, ref target33, hookCtx, false, context))
      target33 = this.IsLinking;
    target.IsLinking = target33;
    SoundSpecifier target34 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LaserTargetSound, ref target34, hookCtx, true, context))
      target34 = serialization.CreateCopy<SoundSpecifier>(this.LaserTargetSound, hookCtx, context);
    target.LaserTargetSound = target34;
    SoundSpecifier target35 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ToggleSound, ref target35, hookCtx, true, context))
      target35 = serialization.CreateCopy<SoundSpecifier>(this.ToggleSound, hookCtx, context);
    target.ToggleSound = target35;
    SoundSpecifier target36 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LaserTargetWarningSound, ref target36, hookCtx, true, context))
      target36 = serialization.CreateCopy<SoundSpecifier>(this.LaserTargetWarningSound, hookCtx, context);
    target.LaserTargetWarningSound = target36;
    TimeSpan target37 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LaserTargetDelay, ref target37, hookCtx, false, context))
      target37 = serialization.CreateCopy<TimeSpan>(this.LaserTargetDelay, hookCtx, context);
    target.LaserTargetDelay = target37;
    bool target38 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsTargeting, ref target38, hookCtx, false, context))
      target38 = this.IsTargeting;
    target.IsTargeting = target38;
    bool target39 = false;
    if (!serialization.TryCustomCopy<bool>(this.NeedAnnouncement, ref target39, hookCtx, false, context))
      target39 = this.NeedAnnouncement;
    target.NeedAnnouncement = target39;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MortarComponent target,
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
    MortarComponent target1 = (MortarComponent) target;
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
    MortarComponent target1 = (MortarComponent) target;
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
    MortarComponent target1 = (MortarComponent) target;
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
  virtual MortarComponent Component.Instantiate() => new MortarComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MortarComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MortarComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<MortarComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      MortarComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastFiredAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MortarComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    string ContainerId;
    public SkillWhitelist Skill;
    public TimeSpan DeployDelay;
    public TimeSpan TargetDelay;
    public TimeSpan DialDelay;
    public bool Deployed;
    public Vector2i Target;
    public Vector2i Offset;
    public Vector2i Dial;
    public TimeSpan FireDelay;
    public int TilesPerOffset;
    public int MaxTarget;
    public int MaxDial;
    public int MinimumRange;
    public int MaximumRange;
    public string FixtureId;
    public TimeSpan AnimationTime;
    public string AnimationLayer;
    public string AnimationState;
    public string DeployedState;
    public SoundSpecifier? DeploySound;
    public SoundSpecifier? ReloadSound;
    public SoundSpecifier? FireSound;
    public TimeSpan? Cooldown;
    public TimeSpan LastFiredAt;
    public EntProtoId Drop;
    public int[] FireRandomOffset;
    public TimeSpan LaserLinkDelay;
    public bool LaserTargetingMode;
    public NetEntity? LinkedLaserDesignator;
    public NetCoordinates? LaserTargetCoordinates;
    public bool IsLinking;
    public SoundSpecifier? LaserTargetSound;
    public SoundSpecifier? ToggleSound;
    public SoundSpecifier? LaserTargetWarningSound;
    public TimeSpan LaserTargetDelay;
    public bool IsTargeting;
    public bool NeedAnnouncement;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MortarComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MortarComponent, ComponentGetState>(new ComponentEventRefHandler<MortarComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MortarComponent, ComponentHandleState>(new ComponentEventRefHandler<MortarComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, MortarComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new MortarComponent.MortarComponent_AutoState()
      {
        ContainerId = component.ContainerId,
        Skill = component.Skill,
        DeployDelay = component.DeployDelay,
        TargetDelay = component.TargetDelay,
        DialDelay = component.DialDelay,
        Deployed = component.Deployed,
        Target = component.Target,
        Offset = component.Offset,
        Dial = component.Dial,
        FireDelay = component.FireDelay,
        TilesPerOffset = component.TilesPerOffset,
        MaxTarget = component.MaxTarget,
        MaxDial = component.MaxDial,
        MinimumRange = component.MinimumRange,
        MaximumRange = component.MaximumRange,
        FixtureId = component.FixtureId,
        AnimationTime = component.AnimationTime,
        AnimationLayer = component.AnimationLayer,
        AnimationState = component.AnimationState,
        DeployedState = component.DeployedState,
        DeploySound = component.DeploySound,
        ReloadSound = component.ReloadSound,
        FireSound = component.FireSound,
        Cooldown = component.Cooldown,
        LastFiredAt = component.LastFiredAt,
        Drop = component.Drop,
        FireRandomOffset = component.FireRandomOffset,
        LaserLinkDelay = component.LaserLinkDelay,
        LaserTargetingMode = component.LaserTargetingMode,
        LinkedLaserDesignator = this.GetNetEntity(component.LinkedLaserDesignator),
        LaserTargetCoordinates = this.GetNetCoordinates(component.LaserTargetCoordinates),
        IsLinking = component.IsLinking,
        LaserTargetSound = component.LaserTargetSound,
        ToggleSound = component.ToggleSound,
        LaserTargetWarningSound = component.LaserTargetWarningSound,
        LaserTargetDelay = component.LaserTargetDelay,
        IsTargeting = component.IsTargeting,
        NeedAnnouncement = component.NeedAnnouncement
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MortarComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MortarComponent.MortarComponent_AutoState current))
        return;
      component.ContainerId = current.ContainerId;
      component.Skill = current.Skill;
      component.DeployDelay = current.DeployDelay;
      component.TargetDelay = current.TargetDelay;
      component.DialDelay = current.DialDelay;
      component.Deployed = current.Deployed;
      component.Target = current.Target;
      component.Offset = current.Offset;
      component.Dial = current.Dial;
      component.FireDelay = current.FireDelay;
      component.TilesPerOffset = current.TilesPerOffset;
      component.MaxTarget = current.MaxTarget;
      component.MaxDial = current.MaxDial;
      component.MinimumRange = current.MinimumRange;
      component.MaximumRange = current.MaximumRange;
      component.FixtureId = current.FixtureId;
      component.AnimationTime = current.AnimationTime;
      component.AnimationLayer = current.AnimationLayer;
      component.AnimationState = current.AnimationState;
      component.DeployedState = current.DeployedState;
      component.DeploySound = current.DeploySound;
      component.ReloadSound = current.ReloadSound;
      component.FireSound = current.FireSound;
      component.Cooldown = current.Cooldown;
      component.LastFiredAt = current.LastFiredAt;
      component.Drop = current.Drop;
      component.FireRandomOffset = current.FireRandomOffset;
      component.LaserLinkDelay = current.LaserLinkDelay;
      component.LaserTargetingMode = current.LaserTargetingMode;
      component.LinkedLaserDesignator = this.EnsureEntity<MortarComponent>(current.LinkedLaserDesignator, uid);
      component.LaserTargetCoordinates = this.EnsureCoordinates<MortarComponent>(current.LaserTargetCoordinates, uid);
      component.IsLinking = current.IsLinking;
      component.LaserTargetSound = current.LaserTargetSound;
      component.ToggleSound = current.ToggleSound;
      component.LaserTargetWarningSound = current.LaserTargetWarningSound;
      component.LaserTargetDelay = current.LaserTargetDelay;
      component.IsTargeting = current.IsTargeting;
      component.NeedAnnouncement = current.NeedAnnouncement;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, MortarComponent>(uid, component, ref args1);
    }
  }
}
