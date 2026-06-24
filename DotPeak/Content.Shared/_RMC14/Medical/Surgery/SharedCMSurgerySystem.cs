// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Surgery.SharedCMSurgerySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Medical.Surgery.Conditions;
using Content.Shared._RMC14.Medical.Surgery.Steps;
using Content.Shared._RMC14.Medical.Surgery.Steps.Parts;
using Content.Shared._RMC14.Medical.Surgery.Tools;
using Content.Shared._RMC14.Xenonids.Organs;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Body.Components;
using Content.Shared.Body.Part;
using Content.Shared.Body.Systems;
using Content.Shared.Buckle.Components;
using Content.Shared.DoAfter;
using Content.Shared.GameTicking;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Medical.Surgery;

public abstract class SharedCMSurgerySystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedBodySystem _body;
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private RotateToFaceSystem _rotateToFace;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private SharedTransformSystem _transform;
  private readonly Dictionary<EntProtoId, EntityUid> _surgeries = new Dictionary<EntProtoId, EntityUid>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestartCleanup));
    this.SubscribeLocalEvent<CMSurgeryTargetComponent, CMSurgeryDoAfterEvent>(new EntityEventRefHandler<CMSurgeryTargetComponent, CMSurgeryDoAfterEvent>(this.OnTargetDoAfter));
    this.SubscribeLocalEvent<CMSurgeryCloseIncisionConditionComponent, CMSurgeryValidEvent>(new EntityEventRefHandler<CMSurgeryCloseIncisionConditionComponent, CMSurgeryValidEvent>(this.OnCloseIncisionValid));
    this.SubscribeLocalEvent<CMSurgeryLarvaConditionComponent, CMSurgeryValidEvent>(new EntityEventRefHandler<CMSurgeryLarvaConditionComponent, CMSurgeryValidEvent>(this.OnLarvaValid));
    this.SubscribeLocalEvent<CMSurgeryPartConditionComponent, CMSurgeryValidEvent>(new EntityEventRefHandler<CMSurgeryPartConditionComponent, CMSurgeryValidEvent>(this.OnPartConditionValid));
    this.SubscribeLocalEvent<RMCSurgeryDeadConditionComponent, CMSurgeryValidEvent>(new EntityEventRefHandler<RMCSurgeryDeadConditionComponent, CMSurgeryValidEvent>(this.OnIsDead));
    this.SubscribeLocalEvent<RMCSurgeryXenoHeartConditionComponent, CMSurgeryValidEvent>(new EntityEventRefHandler<RMCSurgeryXenoHeartConditionComponent, CMSurgeryValidEvent>(this.OnXenoHeartValid));
    this.InitializeSteps();
  }

  private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev) => this._surgeries.Clear();

  private void OnTargetDoAfter(Entity<CMSurgeryTargetComponent> ent, ref CMSurgeryDoAfterEvent args)
  {
    if (!args.Cancelled && !args.Handled)
    {
      EntityUid? target = args.Target;
      if (target.HasValue)
      {
        EntityUid valueOrDefault = target.GetValueOrDefault();
        Entity<CMSurgeryComponent> surgeryEnt;
        Entity<BodyPartComponent> part;
        EntityUid step;
        if (this.IsSurgeryValid((EntityUid) ent, valueOrDefault, args.Surgery, args.Step, out surgeryEnt, out part, out step) && this.PreviousStepsComplete((EntityUid) ent, (EntityUid) part, surgeryEnt, args.Step) && this.CanPerformStep(args.User, (EntityUid) ent, part.Comp.PartType, step, false))
        {
          CMSurgeryStepEvent args1 = new CMSurgeryStepEvent(args.User, (EntityUid) ent, (EntityUid) part, this.GetTools(args.User));
          this.RaiseLocalEvent<CMSurgeryStepEvent>(step, ref args1);
          this.RefreshUI((EntityUid) ent);
          return;
        }
      }
    }
    this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) args.User)} tried to start invalid surgery.");
  }

  private void OnCloseIncisionValid(
    Entity<CMSurgeryCloseIncisionConditionComponent> ent,
    ref CMSurgeryValidEvent args)
  {
    if (this.HasComp<CMIncisionOpenComponent>(args.Part) && this.HasComp<CMBleedersClampedComponent>(args.Part) && this.HasComp<CMSkinRetractedComponent>(args.Part))
      return;
    args.Cancelled = true;
  }

  private void OnLarvaValid(
    Entity<CMSurgeryLarvaConditionComponent> ent,
    ref CMSurgeryValidEvent args)
  {
    VictimInfectedComponent comp;
    if (!this.TryComp<VictimInfectedComponent>(args.Body, out comp))
      args.Cancelled = true;
    if (comp == null || !comp.IsBursting)
      return;
    args.Cancelled = true;
  }

  private void OnPartConditionValid(
    Entity<CMSurgeryPartConditionComponent> ent,
    ref CMSurgeryValidEvent args)
  {
    BodyPartType? partType = this.CompOrNull<BodyPartComponent>(args.Part)?.PartType;
    BodyPartType part = ent.Comp.Part;
    if (partType.GetValueOrDefault() == part & partType.HasValue)
      return;
    args.Cancelled = true;
  }

  private void OnIsDead(Entity<RMCSurgeryDeadConditionComponent> ent, ref CMSurgeryValidEvent args)
  {
    if (this._mobState.IsDead(args.Body))
      return;
    args.Cancelled = true;
  }

  private void OnXenoHeartValid(
    Entity<RMCSurgeryXenoHeartConditionComponent> ent,
    ref CMSurgeryValidEvent args)
  {
    if (this.HasComp<RMCSurgeryXenoHeartComponent>(args.Body) && this._body.GetBodyOrganEntityComps<XenoHeartComponent>((Entity<BodyComponent>) args.Body).Count != 0)
      return;
    args.Cancelled = true;
  }

  protected bool IsSurgeryValid(
    EntityUid body,
    EntityUid targetPart,
    EntProtoId surgery,
    EntProtoId stepId,
    out Entity<CMSurgeryComponent> surgeryEnt,
    out Entity<BodyPartComponent> part,
    out EntityUid step)
  {
    surgeryEnt = new Entity<CMSurgeryComponent>();
    part = new Entity<BodyPartComponent>();
    step = new EntityUid();
    BodyPartComponent comp1;
    if (this.HasComp<CMSurgeryTargetComponent>(body) && this.IsLyingDown(body) && this.TryComp<BodyPartComponent>(targetPart, out comp1))
    {
      EntityUid? singleton1 = this.GetSingleton(surgery);
      if (singleton1.HasValue)
      {
        EntityUid valueOrDefault1 = singleton1.GetValueOrDefault();
        CMSurgeryComponent comp2;
        if (this.TryComp<CMSurgeryComponent>(valueOrDefault1, out comp2) && comp2.Steps.Contains(stepId))
        {
          EntityUid? singleton2 = this.GetSingleton(stepId);
          if (singleton2.HasValue)
          {
            EntityUid valueOrDefault2 = singleton2.GetValueOrDefault();
            CMSurgeryValidEvent args = new CMSurgeryValidEvent(body, targetPart);
            this.RaiseLocalEvent<CMSurgeryValidEvent>(valueOrDefault2, ref args);
            this.RaiseLocalEvent<CMSurgeryValidEvent>(valueOrDefault1, ref args);
            if (args.Cancelled)
              return false;
            surgeryEnt = (Entity<CMSurgeryComponent>) (valueOrDefault1, comp2);
            part = (Entity<BodyPartComponent>) (targetPart, comp1);
            step = valueOrDefault2;
            return true;
          }
        }
      }
    }
    return false;
  }

  public EntityUid? GetSingleton(EntProtoId surgeryOrStep)
  {
    if (!this._prototypes.HasIndex(surgeryOrStep))
      return new EntityUid?();
    EntityUid uid;
    if (!this._surgeries.TryGetValue(surgeryOrStep, out uid) || this.TerminatingOrDeleted(uid))
    {
      uid = this.Spawn((string) surgeryOrStep, MapCoordinates.Nullspace, rotation: new Angle());
      this._surgeries[surgeryOrStep] = uid;
    }
    return new EntityUid?(uid);
  }

  private List<EntityUid> GetTools(EntityUid surgeon)
  {
    return this._hands.EnumerateHeld((Entity<HandsComponent>) surgeon).ToList<EntityUid>();
  }

  public bool IsLyingDown(EntityUid entity)
  {
    if (this._standing.IsDown(entity))
      return true;
    BuckleComponent comp1;
    StrapComponent comp2;
    if (this.TryComp<BuckleComponent>(entity, out comp1) && this.TryComp<StrapComponent>(comp1.BuckledTo, out comp2))
    {
      Angle rotation = comp2.Rotation;
      Direction cardinalDir = ((Angle) ref rotation).GetCardinalDir();
      if (cardinalDir == 2 || cardinalDir == 6)
        return true;
    }
    return false;
  }

  protected virtual void RefreshUI(EntityUid body)
  {
  }

  private void InitializeSteps()
  {
    this.SubscribeLocalEvent<CMSurgeryStepComponent, CMSurgeryStepEvent>(new EntityEventRefHandler<CMSurgeryStepComponent, CMSurgeryStepEvent>(this.OnToolStep));
    this.SubscribeLocalEvent<CMSurgeryStepComponent, CMSurgeryStepCompleteCheckEvent>(new EntityEventRefHandler<CMSurgeryStepComponent, CMSurgeryStepCompleteCheckEvent>(this.OnToolCheck));
    this.SubscribeLocalEvent<CMSurgeryStepComponent, CMSurgeryCanPerformStepEvent>(new EntityEventRefHandler<CMSurgeryStepComponent, CMSurgeryCanPerformStepEvent>(this.OnToolCanPerform));
    this.SubSurgery<CMSurgeryCutLarvaRootsStepComponent>(new EntityEventRefHandler<CMSurgeryCutLarvaRootsStepComponent, CMSurgeryStepEvent>(this.OnCutLarvaRootsStep), new EntityEventRefHandler<CMSurgeryCutLarvaRootsStepComponent, CMSurgeryStepCompleteCheckEvent>(this.OnCutLarvaRootsCheck));
    this.Subs.BuiEvents<CMSurgeryTargetComponent>((object) CMSurgeryUIKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<CMSurgeryTargetComponent>) (subs => subs.Event<CMSurgeryStepChosenBuiMsg>(new EntityEventRefHandler<CMSurgeryTargetComponent, CMSurgeryStepChosenBuiMsg>(this.OnSurgeryTargetStepChosen))));
  }

  private void SubSurgery<TComp>(
    EntityEventRefHandler<TComp, CMSurgeryStepEvent> onStep,
    EntityEventRefHandler<TComp, CMSurgeryStepCompleteCheckEvent> onComplete)
    where TComp : IComponent
  {
    this.SubscribeLocalEvent<TComp, CMSurgeryStepEvent>(onStep);
    this.SubscribeLocalEvent<TComp, CMSurgeryStepCompleteCheckEvent>(onComplete);
  }

  private void OnToolStep(Entity<CMSurgeryStepComponent> ent, ref CMSurgeryStepEvent args)
  {
    if (ent.Comp.Tool != null)
    {
      foreach (EntityPrototype.ComponentRegistryEntry componentRegistryEntry in ent.Comp.Tool.Values)
      {
        EntityUid withComp;
        if (!this.AnyHaveComp(args.Tools, componentRegistryEntry.Component, out withComp))
          return;
        CMSurgeryToolComponent comp;
        if (this._net.IsServer && this.TryComp<CMSurgeryToolComponent>(withComp, out comp) && comp.EndSound != null)
          this._audio.PlayPvs(comp.EndSound, withComp);
      }
    }
    if (ent.Comp.Add != null)
    {
      foreach (EntityPrototype.ComponentRegistryEntry componentRegistryEntry in ent.Comp.Add.Values)
      {
        Type type = componentRegistryEntry.Component.GetType();
        if (!this.HasComp(args.Part, type))
          this.AddComp<IComponent>(args.Part, this._compFactory.GetComponent(type));
      }
    }
    if (ent.Comp.Remove != null)
    {
      foreach (EntityPrototype.ComponentRegistryEntry componentRegistryEntry in ent.Comp.Remove.Values)
        this.RemComp(args.Part, componentRegistryEntry.Component.GetType());
    }
    if (ent.Comp.BodyRemove == null)
      return;
    foreach (EntityPrototype.ComponentRegistryEntry componentRegistryEntry in ent.Comp.BodyRemove.Values)
      this.RemComp(args.Body, componentRegistryEntry.Component.GetType());
  }

  private void OnToolCheck(
    Entity<CMSurgeryStepComponent> ent,
    ref CMSurgeryStepCompleteCheckEvent args)
  {
    if (ent.Comp.Add != null)
    {
      foreach (EntityPrototype.ComponentRegistryEntry componentRegistryEntry in ent.Comp.Add.Values)
      {
        if (!this.HasComp(args.Part, componentRegistryEntry.Component.GetType()))
        {
          args.Cancelled = true;
          return;
        }
      }
    }
    if (ent.Comp.Remove != null)
    {
      foreach (EntityPrototype.ComponentRegistryEntry componentRegistryEntry in ent.Comp.Remove.Values)
      {
        if (this.HasComp(args.Part, componentRegistryEntry.Component.GetType()))
        {
          args.Cancelled = true;
          return;
        }
      }
    }
    if (ent.Comp.BodyRemove == null)
      return;
    foreach (EntityPrototype.ComponentRegistryEntry componentRegistryEntry in ent.Comp.BodyRemove.Values)
    {
      if (this.HasComp(args.Body, componentRegistryEntry.Component.GetType()))
      {
        args.Cancelled = true;
        break;
      }
    }
  }

  private void OnToolCanPerform(
    Entity<CMSurgeryStepComponent> ent,
    ref CMSurgeryCanPerformStepEvent args)
  {
    if (!this._skills.HasSkill((Entity<SkillsComponent>) args.User, ent.Comp.SkillType, ent.Comp.Skill))
    {
      args.Invalid = StepInvalidReason.MissingSkills;
    }
    else
    {
      BuckleComponent comp;
      if (this.HasComp<CMSurgeryOperatingTableConditionComponent>((EntityUid) ent) && (!this.TryComp<BuckleComponent>(args.Body, out comp) || !this.HasComp<CMOperatingTableComponent>(comp.BuckledTo)))
      {
        args.Invalid = StepInvalidReason.NeedsOperatingTable;
      }
      else
      {
        this.RaiseLocalEvent<CMSurgeryCanPerformStepEvent>(args.Body, ref args);
        if (args.Invalid != StepInvalidReason.None || ent.Comp.Tool == null)
          return;
        ref CMSurgeryCanPerformStepEvent local = ref args;
        if (local.ValidTools == null)
        {
          HashSet<EntityUid> entityUidSet;
          local.ValidTools = entityUidSet = new HashSet<EntityUid>();
        }
        foreach (EntityPrototype.ComponentRegistryEntry componentRegistryEntry in ent.Comp.Tool.Values)
        {
          EntityUid withComp;
          if (!this.AnyHaveComp(args.Tools, componentRegistryEntry.Component, out withComp))
          {
            args.Invalid = StepInvalidReason.MissingTool;
            if (!(componentRegistryEntry.Component is ICMSurgeryToolComponent component))
              break;
            args.Popup = $"You need {component.ToolName} to perform this step!";
            break;
          }
          args.ValidTools.Add(withComp);
        }
      }
    }
  }

  private void OnCutLarvaRootsStep(
    Entity<CMSurgeryCutLarvaRootsStepComponent> ent,
    ref CMSurgeryStepEvent args)
  {
    VictimInfectedComponent comp;
    if (!this.TryComp<VictimInfectedComponent>(args.Body, out comp) || comp.IsBursting)
      return;
    comp.RootsCut = true;
  }

  private void OnCutLarvaRootsCheck(
    Entity<CMSurgeryCutLarvaRootsStepComponent> ent,
    ref CMSurgeryStepCompleteCheckEvent args)
  {
    VictimInfectedComponent comp;
    if (!this.TryComp<VictimInfectedComponent>(args.Body, out comp) || !comp.RootsCut)
      args.Cancelled = true;
    if (comp == null || !comp.IsBursting)
      return;
    args.Cancelled = true;
  }

  private void OnSurgeryTargetStepChosen(
    Entity<CMSurgeryTargetComponent> ent,
    ref CMSurgeryStepChosenBuiMsg args)
  {
    EntityUid actor = args.Actor;
    EntityUid entity1 = this.GetEntity(args.Entity);
    if (!entity1.Valid)
      return;
    EntityUid entity2 = this.GetEntity(args.Part);
    Entity<CMSurgeryComponent> surgeryEnt;
    Entity<BodyPartComponent> part;
    EntityUid step;
    HashSet<EntityUid> validTools;
    if (!entity2.Valid || !this.IsSurgeryValid(entity1, entity2, args.Surgery, args.Step, out surgeryEnt, out part, out step) || !this.PreviousStepsComplete(entity1, (EntityUid) part, surgeryEnt, args.Step) || this.IsStepComplete(entity1, (EntityUid) part, args.Step) || !this.CanPerformStep(actor, entity1, part.Comp.PartType, step, true, out string _, out StepInvalidReason _, out validTools))
      return;
    // ISSUE: explicit non-virtual call
    if (this._net.IsServer && validTools != null && __nonvirtual (validTools.Count) > 0)
    {
      foreach (EntityUid uid in validTools)
      {
        CMSurgeryToolComponent comp;
        if (this.TryComp<CMSurgeryToolComponent>(uid, out comp) && comp.StartSound != null)
          this._audio.PlayPvs(comp.StartSound, uid);
      }
    }
    TransformComponent comp1;
    if (this.TryComp(entity1, out comp1))
      this._rotateToFace.TryFaceCoordinates(actor, this._transform.GetMapCoordinates(entity1, comp1).Position);
    CMSurgeryDoAfterEvent @event = new CMSurgeryDoAfterEvent(args.Surgery, args.Step);
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, actor, 2f, (DoAfterEvent) @event, new EntityUid?(entity1), new EntityUid?((EntityUid) part))
    {
      BreakOnMove = true,
      TargetEffect = (EntProtoId?) "RMCEffectHealBusy",
      MovementThreshold = 0.5f
    });
  }

  private (Entity<CMSurgeryComponent> Surgery, int Step)? GetNextStep(
    EntityUid body,
    EntityUid part,
    Entity<CMSurgeryComponent?> surgery,
    List<EntityUid> requirements)
  {
    if (!this.Resolve<CMSurgeryComponent>((EntityUid) surgery, ref surgery.Comp))
      return new (Entity<CMSurgeryComponent>, int)?();
    if (requirements.Contains((EntityUid) surgery))
      throw new ArgumentException($"Surgery {surgery} has a requirement loop: {string.Join<EntityUid>(", ", (IEnumerable<EntityUid>) requirements)}");
    requirements.Add((EntityUid) surgery);
    EntProtoId? requirement = surgery.Comp.Requirement;
    (Entity<CMSurgeryComponent>, int)? nextStep;
    if (requirement.HasValue)
    {
      EntityUid? singleton = this.GetSingleton(requirement.GetValueOrDefault());
      if (singleton.HasValue)
      {
        EntityUid valueOrDefault = singleton.GetValueOrDefault();
        nextStep = this.GetNextStep(body, part, (Entity<CMSurgeryComponent>) valueOrDefault, requirements);
        if (nextStep.HasValue)
          return new (Entity<CMSurgeryComponent>, int)?(nextStep.GetValueOrDefault());
      }
    }
    for (int index = 0; index < surgery.Comp.Steps.Count; ++index)
    {
      EntProtoId step = surgery.Comp.Steps[index];
      if (!this.IsStepComplete(body, part, step))
        return new (Entity<CMSurgeryComponent>, int)?(((Entity<CMSurgeryComponent>) ((EntityUid) surgery, surgery.Comp), index));
    }
    nextStep = new (Entity<CMSurgeryComponent>, int)?();
    return nextStep;
  }

  public (Entity<CMSurgeryComponent> Surgery, int Step)? GetNextStep(
    EntityUid body,
    EntityUid part,
    EntityUid surgery)
  {
    return this.GetNextStep(body, part, (Entity<CMSurgeryComponent>) surgery, new List<EntityUid>());
  }

  public bool PreviousStepsComplete(
    EntityUid body,
    EntityUid part,
    Entity<CMSurgeryComponent> surgery,
    EntProtoId step)
  {
    EntProtoId? requirement = surgery.Comp.Requirement;
    if (requirement.HasValue)
    {
      EntityUid? singleton = this.GetSingleton(requirement.GetValueOrDefault());
      if (singleton.HasValue)
      {
        EntityUid valueOrDefault = singleton.GetValueOrDefault();
        CMSurgeryComponent comp;
        if (this.TryComp<CMSurgeryComponent>(valueOrDefault, out comp) && this.PreviousStepsComplete(body, part, (Entity<CMSurgeryComponent>) (valueOrDefault, comp), step))
          goto label_4;
      }
      return false;
    }
label_4:
    foreach (EntProtoId step1 in surgery.Comp.Steps)
    {
      if (!(step1 == step))
      {
        if (!this.IsStepComplete(body, part, step1))
          return false;
      }
      else
        break;
    }
    return true;
  }

  public bool CanPerformStep(
    EntityUid user,
    EntityUid body,
    BodyPartType part,
    EntityUid step,
    bool doPopup,
    out string? popup,
    out StepInvalidReason reason,
    out HashSet<EntityUid>? validTools)
  {
    SlotFlags slotFlags;
    switch (part)
    {
      case BodyPartType.Other:
        slotFlags = SlotFlags.NONE;
        break;
      case BodyPartType.Torso:
        slotFlags = SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING;
        break;
      case BodyPartType.Head:
        slotFlags = SlotFlags.HEAD;
        break;
      case BodyPartType.Arm:
        slotFlags = SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING;
        break;
      case BodyPartType.Hand:
        slotFlags = SlotFlags.GLOVES;
        break;
      case BodyPartType.Leg:
        slotFlags = SlotFlags.OUTERCLOTHING | SlotFlags.LEGS;
        break;
      case BodyPartType.Foot:
        slotFlags = SlotFlags.FEET;
        break;
      case BodyPartType.Tail:
        slotFlags = SlotFlags.NONE;
        break;
      default:
        slotFlags = SlotFlags.NONE;
        break;
    }
    SlotFlags TargetSlots = slotFlags;
    CMSurgeryCanPerformStepEvent args = new CMSurgeryCanPerformStepEvent(user, body, this.GetTools(user), TargetSlots);
    this.RaiseLocalEvent<CMSurgeryCanPerformStepEvent>(step, ref args);
    popup = args.Popup;
    validTools = args.ValidTools;
    if (args.Invalid != StepInvalidReason.None)
    {
      if (doPopup && args.Popup != null)
        this._popup.PopupEntity(args.Popup, user, PopupType.SmallCaution);
      reason = args.Invalid;
      return false;
    }
    reason = StepInvalidReason.None;
    return true;
  }

  public bool CanPerformStep(
    EntityUid user,
    EntityUid body,
    BodyPartType part,
    EntityUid step,
    bool doPopup)
  {
    return this.CanPerformStep(user, body, part, step, doPopup, out string _, out StepInvalidReason _, out HashSet<EntityUid> _);
  }

  public bool IsStepComplete(EntityUid body, EntityUid part, EntProtoId step)
  {
    EntityUid? singleton = this.GetSingleton(step);
    if (!singleton.HasValue)
      return false;
    EntityUid valueOrDefault = singleton.GetValueOrDefault();
    CMSurgeryStepCompleteCheckEvent args = new CMSurgeryStepCompleteCheckEvent(body, part);
    this.RaiseLocalEvent<CMSurgeryStepCompleteCheckEvent>(valueOrDefault, ref args);
    return !args.Cancelled;
  }

  private bool AnyHaveComp(List<EntityUid> tools, IComponent component, out EntityUid withComp)
  {
    foreach (EntityUid tool in tools)
    {
      if (this.HasComp(tool, component.GetType()))
      {
        withComp = tool;
        return true;
      }
    }
    withComp = new EntityUid();
    return false;
  }
}
