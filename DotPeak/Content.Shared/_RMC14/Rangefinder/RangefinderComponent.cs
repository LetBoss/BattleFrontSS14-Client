// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Rangefinder.RangefinderComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Rangefinder;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (RangefinderSystem)})]
public sealed class RangefinderComponent : 
  Component,
  ISerializationGenerated<RangefinderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? Id;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Range = 25;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanDesignate;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RangefinderMode Mode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i? LastTarget;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public MapCoordinates? LastCoords;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string TargetUseDelay = "rangefinder_mode";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TargetDelay = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string SwitchModeUseDelay = "rangefinder_mode";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SwitchModeDelay = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Content.Shared.DoAfter.DoAfter? DoAfter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MinimumDelay = TimeSpan.FromSeconds(1.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TimePerSkillLevel = TimeSpan.FromSeconds(2.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillJtac";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId RangefinderSpawn = (EntProtoId) "RMCRangefinderTarget";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<LaserDesignatorTargetComponent> DesignatorSpawn = (EntProtoId<LaserDesignatorTargetComponent>) "RMCLaserDesignatorTarget";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? TargetSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Binoculars/nightvision.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? AcquireSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Binoculars/binoctarget.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ToggleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/click.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BreakRange = 0.5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RangefinderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RangefinderComponent) target1;
    if (serialization.TryCustomCopy<RangefinderComponent>(this, ref target, hookCtx, false, context))
      return;
    int? target2 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Id, ref target2, hookCtx, false, context))
      target2 = this.Id;
    target.Id = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Range, ref target3, hookCtx, false, context))
      target3 = this.Range;
    target.Range = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanDesignate, ref target4, hookCtx, false, context))
      target4 = this.CanDesignate;
    target.CanDesignate = target4;
    RangefinderMode target5 = RangefinderMode.Rangefinder;
    if (!serialization.TryCustomCopy<RangefinderMode>(this.Mode, ref target5, hookCtx, false, context))
      target5 = this.Mode;
    target.Mode = target5;
    Vector2i? target6 = new Vector2i?();
    if (!serialization.TryCustomCopy<Vector2i?>(this.LastTarget, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Vector2i?>(this.LastTarget, hookCtx, context);
    target.LastTarget = target6;
    MapCoordinates? target7 = new MapCoordinates?();
    if (!serialization.TryCustomCopy<MapCoordinates?>(this.LastCoords, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<MapCoordinates?>(this.LastCoords, hookCtx, context);
    target.LastCoords = target7;
    string target8 = (string) null;
    if (this.TargetUseDelay == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.TargetUseDelay, ref target8, hookCtx, false, context))
      target8 = this.TargetUseDelay;
    target.TargetUseDelay = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TargetDelay, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.TargetDelay, hookCtx, context);
    target.TargetDelay = target9;
    string target10 = (string) null;
    if (this.SwitchModeUseDelay == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SwitchModeUseDelay, ref target10, hookCtx, false, context))
      target10 = this.SwitchModeUseDelay;
    target.SwitchModeUseDelay = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SwitchModeDelay, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.SwitchModeDelay, hookCtx, context);
    target.SwitchModeDelay = target11;
    Content.Shared.DoAfter.DoAfter target12 = (Content.Shared.DoAfter.DoAfter) null;
    if (!serialization.TryCustomCopy<Content.Shared.DoAfter.DoAfter>(this.DoAfter, ref target12, hookCtx, false, context))
    {
      if (this.DoAfter == null)
        target12 = (Content.Shared.DoAfter.DoAfter) null;
      else
        serialization.CopyTo<Content.Shared.DoAfter.DoAfter>(this.DoAfter, ref target12, hookCtx, context);
    }
    target.DoAfter = target12;
    TimeSpan target13 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MinimumDelay, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.MinimumDelay, hookCtx, context);
    target.MinimumDelay = target14;
    TimeSpan target15 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimePerSkillLevel, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan>(this.TimePerSkillLevel, hookCtx, context);
    target.TimePerSkillLevel = target15;
    EntProtoId<SkillDefinitionComponent> target16 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target16;
    EntProtoId target17 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.RangefinderSpawn, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<EntProtoId>(this.RangefinderSpawn, hookCtx, context);
    target.RangefinderSpawn = target17;
    EntProtoId<LaserDesignatorTargetComponent> target18 = new EntProtoId<LaserDesignatorTargetComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<LaserDesignatorTargetComponent>>(this.DesignatorSpawn, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<EntProtoId<LaserDesignatorTargetComponent>>(this.DesignatorSpawn, hookCtx, context);
    target.DesignatorSpawn = target18;
    SoundSpecifier target19 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TargetSound, ref target19, hookCtx, true, context))
      target19 = serialization.CreateCopy<SoundSpecifier>(this.TargetSound, hookCtx, context);
    target.TargetSound = target19;
    SoundSpecifier target20 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.AcquireSound, ref target20, hookCtx, true, context))
      target20 = serialization.CreateCopy<SoundSpecifier>(this.AcquireSound, hookCtx, context);
    target.AcquireSound = target20;
    SoundSpecifier target21 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ToggleSound, ref target21, hookCtx, true, context))
      target21 = serialization.CreateCopy<SoundSpecifier>(this.ToggleSound, hookCtx, context);
    target.ToggleSound = target21;
    float target22 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BreakRange, ref target22, hookCtx, false, context))
      target22 = this.BreakRange;
    target.BreakRange = target22;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RangefinderComponent target,
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
    RangefinderComponent target1 = (RangefinderComponent) target;
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
    RangefinderComponent target1 = (RangefinderComponent) target;
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
    RangefinderComponent target1 = (RangefinderComponent) target;
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
  virtual RangefinderComponent Component.Instantiate() => new RangefinderComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RangefinderComponent_AutoState : IComponentState
  {
    public int? Id;
    public int Range;
    public bool CanDesignate;
    public RangefinderMode Mode;
    public Vector2i? LastTarget;
    public MapCoordinates? LastCoords;
    public string TargetUseDelay;
    public TimeSpan TargetDelay;
    public string SwitchModeUseDelay;
    public TimeSpan SwitchModeDelay;
    public Content.Shared.DoAfter.DoAfter? DoAfter;
    public TimeSpan Delay;
    public TimeSpan MinimumDelay;
    public TimeSpan TimePerSkillLevel;
    public EntProtoId<SkillDefinitionComponent> Skill;
    public EntProtoId RangefinderSpawn;
    public EntProtoId<LaserDesignatorTargetComponent> DesignatorSpawn;
    public SoundSpecifier? TargetSound;
    public SoundSpecifier? AcquireSound;
    public SoundSpecifier? ToggleSound;
    public float BreakRange;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RangefinderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RangefinderComponent, ComponentGetState>(new ComponentEventRefHandler<RangefinderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RangefinderComponent, ComponentHandleState>(new ComponentEventRefHandler<RangefinderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RangefinderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RangefinderComponent.RangefinderComponent_AutoState()
      {
        Id = component.Id,
        Range = component.Range,
        CanDesignate = component.CanDesignate,
        Mode = component.Mode,
        LastTarget = component.LastTarget,
        LastCoords = component.LastCoords,
        TargetUseDelay = component.TargetUseDelay,
        TargetDelay = component.TargetDelay,
        SwitchModeUseDelay = component.SwitchModeUseDelay,
        SwitchModeDelay = component.SwitchModeDelay,
        DoAfter = component.DoAfter,
        Delay = component.Delay,
        MinimumDelay = component.MinimumDelay,
        TimePerSkillLevel = component.TimePerSkillLevel,
        Skill = component.Skill,
        RangefinderSpawn = component.RangefinderSpawn,
        DesignatorSpawn = component.DesignatorSpawn,
        TargetSound = component.TargetSound,
        AcquireSound = component.AcquireSound,
        ToggleSound = component.ToggleSound,
        BreakRange = component.BreakRange
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RangefinderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RangefinderComponent.RangefinderComponent_AutoState current))
        return;
      component.Id = current.Id;
      component.Range = current.Range;
      component.CanDesignate = current.CanDesignate;
      component.Mode = current.Mode;
      component.LastTarget = current.LastTarget;
      component.LastCoords = current.LastCoords;
      component.TargetUseDelay = current.TargetUseDelay;
      component.TargetDelay = current.TargetDelay;
      component.SwitchModeUseDelay = current.SwitchModeUseDelay;
      component.SwitchModeDelay = current.SwitchModeDelay;
      component.DoAfter = current.DoAfter;
      component.Delay = current.Delay;
      component.MinimumDelay = current.MinimumDelay;
      component.TimePerSkillLevel = current.TimePerSkillLevel;
      component.Skill = current.Skill;
      component.RangefinderSpawn = current.RangefinderSpawn;
      component.DesignatorSpawn = current.DesignatorSpawn;
      component.TargetSound = current.TargetSound;
      component.AcquireSound = current.AcquireSound;
      component.ToggleSound = current.ToggleSound;
      component.BreakRange = current.BreakRange;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, RangefinderComponent>(uid, component, ref args1);
    }
  }
}
