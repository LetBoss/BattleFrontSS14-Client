using System;
using System.Collections.Generic;
using System.Linq;
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
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

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
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestartCleanup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMSurgeryTargetComponent, CMSurgeryDoAfterEvent>((EntityEventRefHandler<CMSurgeryTargetComponent, CMSurgeryDoAfterEvent>)OnTargetDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMSurgeryCloseIncisionConditionComponent, CMSurgeryValidEvent>((EntityEventRefHandler<CMSurgeryCloseIncisionConditionComponent, CMSurgeryValidEvent>)OnCloseIncisionValid, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMSurgeryLarvaConditionComponent, CMSurgeryValidEvent>((EntityEventRefHandler<CMSurgeryLarvaConditionComponent, CMSurgeryValidEvent>)OnLarvaValid, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMSurgeryPartConditionComponent, CMSurgeryValidEvent>((EntityEventRefHandler<CMSurgeryPartConditionComponent, CMSurgeryValidEvent>)OnPartConditionValid, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSurgeryDeadConditionComponent, CMSurgeryValidEvent>((EntityEventRefHandler<RMCSurgeryDeadConditionComponent, CMSurgeryValidEvent>)OnIsDead, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSurgeryXenoHeartConditionComponent, CMSurgeryValidEvent>((EntityEventRefHandler<RMCSurgeryXenoHeartConditionComponent, CMSurgeryValidEvent>)OnXenoHeartValid, (Type[])null, (Type[])null);
		InitializeSteps();
	}

	private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
	{
		_surgeries.Clear();
	}

	private void OnTargetDoAfter(Entity<CMSurgeryTargetComponent> ent, ref CMSurgeryDoAfterEvent args)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			EntityUid? target = args.Target;
			if (target.HasValue)
			{
				EntityUid target2 = target.GetValueOrDefault();
				if (IsSurgeryValid(Entity<CMSurgeryTargetComponent>.op_Implicit(ent), target2, args.Surgery, args.Step, out Entity<CMSurgeryComponent> surgery, out Entity<BodyPartComponent> part, out EntityUid step) && PreviousStepsComplete(Entity<CMSurgeryTargetComponent>.op_Implicit(ent), Entity<BodyPartComponent>.op_Implicit(part), surgery, args.Step) && CanPerformStep(args.User, Entity<CMSurgeryTargetComponent>.op_Implicit(ent), part.Comp.PartType, step, doPopup: false))
				{
					CMSurgeryStepEvent ev = new CMSurgeryStepEvent(args.User, Entity<CMSurgeryTargetComponent>.op_Implicit(ent), Entity<BodyPartComponent>.op_Implicit(part), GetTools(args.User));
					((EntitySystem)this).RaiseLocalEvent<CMSurgeryStepEvent>(step, ref ev, false);
					RefreshUI(Entity<CMSurgeryTargetComponent>.op_Implicit(ent));
					return;
				}
			}
		}
		((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User))} tried to start invalid surgery.");
	}

	private void OnCloseIncisionValid(Entity<CMSurgeryCloseIncisionConditionComponent> ent, ref CMSurgeryValidEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<CMIncisionOpenComponent>(args.Part) || !((EntitySystem)this).HasComp<CMBleedersClampedComponent>(args.Part) || !((EntitySystem)this).HasComp<CMSkinRetractedComponent>(args.Part))
		{
			args.Cancelled = true;
		}
	}

	private void OnLarvaValid(Entity<CMSurgeryLarvaConditionComponent> ent, ref CMSurgeryValidEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		VictimInfectedComponent infected = default(VictimInfectedComponent);
		if (!((EntitySystem)this).TryComp<VictimInfectedComponent>(args.Body, ref infected))
		{
			args.Cancelled = true;
		}
		if (infected != null && infected.IsBursting)
		{
			args.Cancelled = true;
		}
	}

	private void OnPartConditionValid(Entity<CMSurgeryPartConditionComponent> ent, ref CMSurgeryValidEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).CompOrNull<BodyPartComponent>(args.Part)?.PartType != ent.Comp.Part)
		{
			args.Cancelled = true;
		}
	}

	private void OnIsDead(Entity<RMCSurgeryDeadConditionComponent> ent, ref CMSurgeryValidEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!_mobState.IsDead(args.Body))
		{
			args.Cancelled = true;
		}
	}

	private void OnXenoHeartValid(Entity<RMCSurgeryXenoHeartConditionComponent> ent, ref CMSurgeryValidEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<RMCSurgeryXenoHeartComponent>(args.Body) || _body.GetBodyOrganEntityComps<XenoHeartComponent>(Entity<BodyComponent>.op_Implicit(args.Body)).Count == 0)
		{
			args.Cancelled = true;
		}
	}

	protected bool IsSurgeryValid(EntityUid body, EntityUid targetPart, EntProtoId surgery, EntProtoId stepId, out Entity<CMSurgeryComponent> surgeryEnt, out Entity<BodyPartComponent> part, out EntityUid step)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		surgeryEnt = default(Entity<CMSurgeryComponent>);
		part = default(Entity<BodyPartComponent>);
		step = default(EntityUid);
		BodyPartComponent partComp = default(BodyPartComponent);
		if (((EntitySystem)this).HasComp<CMSurgeryTargetComponent>(body) && IsLyingDown(body) && ((EntitySystem)this).TryComp<BodyPartComponent>(targetPart, ref partComp))
		{
			EntityUid? singleton = GetSingleton(surgery);
			if (singleton.HasValue)
			{
				EntityUid surgeryEntId = singleton.GetValueOrDefault();
				CMSurgeryComponent surgeryComp = default(CMSurgeryComponent);
				if (((EntitySystem)this).TryComp<CMSurgeryComponent>(surgeryEntId, ref surgeryComp) && surgeryComp.Steps.Contains(stepId))
				{
					singleton = GetSingleton(stepId);
					if (singleton.HasValue)
					{
						EntityUid stepEnt = singleton.GetValueOrDefault();
						CMSurgeryValidEvent ev = new CMSurgeryValidEvent(body, targetPart);
						((EntitySystem)this).RaiseLocalEvent<CMSurgeryValidEvent>(stepEnt, ref ev, false);
						((EntitySystem)this).RaiseLocalEvent<CMSurgeryValidEvent>(surgeryEntId, ref ev, false);
						if (ev.Cancelled)
						{
							return false;
						}
						surgeryEnt = Entity<CMSurgeryComponent>.op_Implicit((surgeryEntId, surgeryComp));
						part = Entity<BodyPartComponent>.op_Implicit((targetPart, partComp));
						step = stepEnt;
						return true;
					}
				}
			}
		}
		return false;
	}

	public EntityUid? GetSingleton(EntProtoId surgeryOrStep)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (!_prototypes.HasIndex(surgeryOrStep))
		{
			return null;
		}
		if (!_surgeries.TryGetValue(surgeryOrStep, out var ent) || ((EntitySystem)this).TerminatingOrDeleted(ent, (MetaDataComponent)null))
		{
			ent = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(surgeryOrStep), MapCoordinates.Nullspace, (ComponentRegistry)null, default(Angle));
			_surgeries[surgeryOrStep] = ent;
		}
		return ent;
	}

	private List<EntityUid> GetTools(EntityUid surgeon)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(surgeon)).ToList();
	}

	public bool IsLyingDown(EntityUid entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Invalid comparison between Unknown and I4
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		if (_standing.IsDown(entity))
		{
			return true;
		}
		BuckleComponent buckle = default(BuckleComponent);
		StrapComponent strap = default(StrapComponent);
		if (((EntitySystem)this).TryComp<BuckleComponent>(entity, ref buckle) && ((EntitySystem)this).TryComp<StrapComponent>(buckle.BuckledTo, ref strap))
		{
			Angle rotation = strap.Rotation;
			Direction cardinalDir = ((Angle)(ref rotation)).GetCardinalDir();
			if (((int)cardinalDir == 2 || (int)cardinalDir == 6) ? true : false)
			{
				return true;
			}
		}
		return false;
	}

	protected virtual void RefreshUI(EntityUid body)
	{
	}

	private void InitializeSteps()
	{
		((EntitySystem)this).SubscribeLocalEvent<CMSurgeryStepComponent, CMSurgeryStepEvent>((EntityEventRefHandler<CMSurgeryStepComponent, CMSurgeryStepEvent>)OnToolStep, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMSurgeryStepComponent, CMSurgeryStepCompleteCheckEvent>((EntityEventRefHandler<CMSurgeryStepComponent, CMSurgeryStepCompleteCheckEvent>)OnToolCheck, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMSurgeryStepComponent, CMSurgeryCanPerformStepEvent>((EntityEventRefHandler<CMSurgeryStepComponent, CMSurgeryCanPerformStepEvent>)OnToolCanPerform, (Type[])null, (Type[])null);
		SubSurgery<CMSurgeryCutLarvaRootsStepComponent>((EntityEventRefHandler<CMSurgeryCutLarvaRootsStepComponent, CMSurgeryStepEvent>)OnCutLarvaRootsStep, (EntityEventRefHandler<CMSurgeryCutLarvaRootsStepComponent, CMSurgeryStepCompleteCheckEvent>)OnCutLarvaRootsCheck);
		BoundUserInterfaceRegisterExt.BuiEvents<CMSurgeryTargetComponent>(((EntitySystem)this).Subs, (object)CMSurgeryUIKey.Key, (BuiEventSubscriber<CMSurgeryTargetComponent>)delegate(Subscriber<CMSurgeryTargetComponent> subs)
		{
			subs.Event<CMSurgeryStepChosenBuiMsg>((EntityEventRefHandler<CMSurgeryTargetComponent, CMSurgeryStepChosenBuiMsg>)OnSurgeryTargetStepChosen);
		});
	}

	private void SubSurgery<TComp>(EntityEventRefHandler<TComp, CMSurgeryStepEvent> onStep, EntityEventRefHandler<TComp, CMSurgeryStepCompleteCheckEvent> onComplete) where TComp : IComponent
	{
		((EntitySystem)this).SubscribeLocalEvent<TComp, CMSurgeryStepEvent>(onStep, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TComp, CMSurgeryStepCompleteCheckEvent>(onComplete, (Type[])null, (Type[])null);
	}

	private void OnToolStep(Entity<CMSurgeryStepComponent> ent, ref CMSurgeryStepEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Tool != null)
		{
			CMSurgeryToolComponent toolComp = default(CMSurgeryToolComponent);
			foreach (ComponentRegistryEntry reg in ((Dictionary<string, ComponentRegistryEntry>)(object)ent.Comp.Tool).Values)
			{
				if (!AnyHaveComp(args.Tools, reg.Component, out var tool))
				{
					return;
				}
				if (_net.IsServer && ((EntitySystem)this).TryComp<CMSurgeryToolComponent>(tool, ref toolComp) && toolComp.EndSound != null)
				{
					_audio.PlayPvs(toolComp.EndSound, tool, (AudioParams?)null);
				}
			}
		}
		if (ent.Comp.Add != null)
		{
			foreach (ComponentRegistryEntry value in ((Dictionary<string, ComponentRegistryEntry>)(object)ent.Comp.Add).Values)
			{
				Type compType = ((object)value.Component).GetType();
				if (!((EntitySystem)this).HasComp(args.Part, compType))
				{
					((EntitySystem)this).AddComp<IComponent>(args.Part, _compFactory.GetComponent(compType), false);
				}
			}
		}
		if (ent.Comp.Remove != null)
		{
			foreach (ComponentRegistryEntry reg2 in ((Dictionary<string, ComponentRegistryEntry>)(object)ent.Comp.Remove).Values)
			{
				((EntitySystem)this).RemComp(args.Part, ((object)reg2.Component).GetType());
			}
		}
		if (ent.Comp.BodyRemove == null)
		{
			return;
		}
		foreach (ComponentRegistryEntry reg3 in ((Dictionary<string, ComponentRegistryEntry>)(object)ent.Comp.BodyRemove).Values)
		{
			((EntitySystem)this).RemComp(args.Body, ((object)reg3.Component).GetType());
		}
	}

	private void OnToolCheck(Entity<CMSurgeryStepComponent> ent, ref CMSurgeryStepCompleteCheckEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Add != null)
		{
			foreach (ComponentRegistryEntry reg in ((Dictionary<string, ComponentRegistryEntry>)(object)ent.Comp.Add).Values)
			{
				if (!((EntitySystem)this).HasComp(args.Part, ((object)reg.Component).GetType()))
				{
					args.Cancelled = true;
					return;
				}
			}
		}
		if (ent.Comp.Remove != null)
		{
			foreach (ComponentRegistryEntry reg2 in ((Dictionary<string, ComponentRegistryEntry>)(object)ent.Comp.Remove).Values)
			{
				if (((EntitySystem)this).HasComp(args.Part, ((object)reg2.Component).GetType()))
				{
					args.Cancelled = true;
					return;
				}
			}
		}
		if (ent.Comp.BodyRemove == null)
		{
			return;
		}
		foreach (ComponentRegistryEntry reg3 in ((Dictionary<string, ComponentRegistryEntry>)(object)ent.Comp.BodyRemove).Values)
		{
			if (((EntitySystem)this).HasComp(args.Body, ((object)reg3.Component).GetType()))
			{
				args.Cancelled = true;
				break;
			}
		}
	}

	private void OnToolCanPerform(Entity<CMSurgeryStepComponent> ent, ref CMSurgeryCanPerformStepEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		if (!_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.SkillType, ent.Comp.Skill))
		{
			args.Invalid = StepInvalidReason.MissingSkills;
			return;
		}
		BuckleComponent buckle = default(BuckleComponent);
		if (((EntitySystem)this).HasComp<CMSurgeryOperatingTableConditionComponent>(Entity<CMSurgeryStepComponent>.op_Implicit(ent)) && (!((EntitySystem)this).TryComp<BuckleComponent>(args.Body, ref buckle) || !((EntitySystem)this).HasComp<CMOperatingTableComponent>(buckle.BuckledTo)))
		{
			args.Invalid = StepInvalidReason.NeedsOperatingTable;
			return;
		}
		((EntitySystem)this).RaiseLocalEvent<CMSurgeryCanPerformStepEvent>(args.Body, ref args, false);
		if (args.Invalid != StepInvalidReason.None || ent.Comp.Tool == null)
		{
			return;
		}
		if (args.ValidTools == null)
		{
			HashSet<EntityUid> hashSet = (args.ValidTools = new HashSet<EntityUid>());
		}
		foreach (ComponentRegistryEntry reg in ((Dictionary<string, ComponentRegistryEntry>)(object)ent.Comp.Tool).Values)
		{
			if (!AnyHaveComp(args.Tools, reg.Component, out var withComp))
			{
				args.Invalid = StepInvalidReason.MissingTool;
				if (reg.Component is ICMSurgeryToolComponent tool)
				{
					args.Popup = "You need " + tool.ToolName + " to perform this step!";
				}
				break;
			}
			args.ValidTools.Add(withComp);
		}
	}

	private void OnCutLarvaRootsStep(Entity<CMSurgeryCutLarvaRootsStepComponent> ent, ref CMSurgeryStepEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		VictimInfectedComponent infected = default(VictimInfectedComponent);
		if (((EntitySystem)this).TryComp<VictimInfectedComponent>(args.Body, ref infected) && !infected.IsBursting)
		{
			infected.RootsCut = true;
		}
	}

	private void OnCutLarvaRootsCheck(Entity<CMSurgeryCutLarvaRootsStepComponent> ent, ref CMSurgeryStepCompleteCheckEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		VictimInfectedComponent infected = default(VictimInfectedComponent);
		if (!((EntitySystem)this).TryComp<VictimInfectedComponent>(args.Body, ref infected) || !infected.RootsCut)
		{
			args.Cancelled = true;
		}
		if (infected != null && infected.IsBursting)
		{
			args.Cancelled = true;
		}
	}

	private void OnSurgeryTargetStepChosen(Entity<CMSurgeryTargetComponent> ent, ref CMSurgeryStepChosenBuiMsg args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = ((BaseBoundUserInterfaceEvent)args).Actor;
		EntityUid body = ((EntitySystem)this).GetEntity(((BoundUserInterfaceMessage)args).Entity);
		if (!((EntityUid)(ref body)).Valid)
		{
			return;
		}
		EntityUid targetPart = ((EntitySystem)this).GetEntity(args.Part);
		if (!((EntityUid)(ref targetPart)).Valid || !IsSurgeryValid(body, targetPart, args.Surgery, args.Step, out Entity<CMSurgeryComponent> surgery, out Entity<BodyPartComponent> part, out EntityUid step) || !PreviousStepsComplete(body, Entity<BodyPartComponent>.op_Implicit(part), surgery, args.Step) || IsStepComplete(body, Entity<BodyPartComponent>.op_Implicit(part), args.Step) || !CanPerformStep(user, body, part.Comp.PartType, step, doPopup: true, out string _, out StepInvalidReason _, out HashSet<EntityUid> validTools))
		{
			return;
		}
		if (_net.IsServer && validTools != null && validTools.Count > 0)
		{
			CMSurgeryToolComponent toolComp = default(CMSurgeryToolComponent);
			foreach (EntityUid tool in validTools)
			{
				if (((EntitySystem)this).TryComp<CMSurgeryToolComponent>(tool, ref toolComp) && toolComp.StartSound != null)
				{
					_audio.PlayPvs(toolComp.StartSound, tool, (AudioParams?)null);
				}
			}
		}
		TransformComponent xform = default(TransformComponent);
		if (((EntitySystem)this).TryComp(body, ref xform))
		{
			_rotateToFace.TryFaceCoordinates(user, _transform.GetMapCoordinates(body, xform).Position);
		}
		CMSurgeryDoAfterEvent ev = new CMSurgeryDoAfterEvent(args.Surgery, args.Step);
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, 2f, ev, body, Entity<BodyPartComponent>.op_Implicit(part))
		{
			BreakOnMove = true,
			TargetEffect = EntProtoId.op_Implicit("RMCEffectHealBusy"),
			MovementThreshold = 0.5f
		};
		_doAfter.TryStartDoAfter(doAfter);
	}

	private (Entity<CMSurgeryComponent> Surgery, int Step)? GetNextStep(EntityUid body, EntityUid part, Entity<CMSurgeryComponent?> surgery, List<EntityUid> requirements)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<CMSurgeryComponent>(Entity<CMSurgeryComponent>.op_Implicit(surgery), ref surgery.Comp, true))
		{
			return null;
		}
		if (requirements.Contains(Entity<CMSurgeryComponent>.op_Implicit(surgery)))
		{
			throw new ArgumentException($"Surgery {surgery} has a requirement loop: {string.Join(", ", requirements)}");
		}
		requirements.Add(Entity<CMSurgeryComponent>.op_Implicit(surgery));
		EntProtoId? requirement = surgery.Comp.Requirement;
		if (requirement.HasValue)
		{
			EntProtoId requirementId = requirement.GetValueOrDefault();
			EntityUid? singleton = GetSingleton(requirementId);
			if (singleton.HasValue)
			{
				EntityUid requirement2 = singleton.GetValueOrDefault();
				(Entity<CMSurgeryComponent>, int)? nextStep = GetNextStep(body, part, Entity<CMSurgeryComponent>.op_Implicit(requirement2), requirements);
				if (nextStep.HasValue)
				{
					return nextStep.GetValueOrDefault();
				}
			}
		}
		for (int i = 0; i < surgery.Comp.Steps.Count; i++)
		{
			EntProtoId surgeryStep = surgery.Comp.Steps[i];
			if (!IsStepComplete(body, part, surgeryStep))
			{
				return (Entity<CMSurgeryComponent>.op_Implicit((Entity<CMSurgeryComponent>.op_Implicit(surgery), surgery.Comp)), i);
			}
		}
		return null;
	}

	public (Entity<CMSurgeryComponent> Surgery, int Step)? GetNextStep(EntityUid body, EntityUid part, EntityUid surgery)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return GetNextStep(body, part, Entity<CMSurgeryComponent>.op_Implicit(surgery), new List<EntityUid>());
	}

	public bool PreviousStepsComplete(EntityUid body, EntityUid part, Entity<CMSurgeryComponent> surgery, EntProtoId step)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		EntProtoId? requirement = surgery.Comp.Requirement;
		if (requirement.HasValue)
		{
			EntProtoId requirement2 = requirement.GetValueOrDefault();
			EntityUid? singleton = GetSingleton(requirement2);
			if (singleton.HasValue)
			{
				EntityUid requiredEnt = singleton.GetValueOrDefault();
				CMSurgeryComponent requiredComp = default(CMSurgeryComponent);
				if (((EntitySystem)this).TryComp<CMSurgeryComponent>(requiredEnt, ref requiredComp) && PreviousStepsComplete(body, part, Entity<CMSurgeryComponent>.op_Implicit((requiredEnt, requiredComp)), step))
				{
					goto IL_005c;
				}
			}
			return false;
		}
		goto IL_005c;
		IL_005c:
		foreach (EntProtoId surgeryStep in surgery.Comp.Steps)
		{
			if (surgeryStep == step)
			{
				break;
			}
			if (!IsStepComplete(body, part, surgeryStep))
			{
				return false;
			}
		}
		return true;
	}

	public bool CanPerformStep(EntityUid user, EntityUid body, BodyPartType part, EntityUid step, bool doPopup, out string? popup, out StepInvalidReason reason, out HashSet<EntityUid>? validTools)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		CMSurgeryCanPerformStepEvent check = new CMSurgeryCanPerformStepEvent(user, body, GetTools(user), part switch
		{
			BodyPartType.Head => SlotFlags.HEAD, 
			BodyPartType.Torso => SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING, 
			BodyPartType.Arm => SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING, 
			BodyPartType.Hand => SlotFlags.GLOVES, 
			BodyPartType.Leg => SlotFlags.OUTERCLOTHING | SlotFlags.LEGS, 
			BodyPartType.Foot => SlotFlags.FEET, 
			BodyPartType.Tail => SlotFlags.NONE, 
			BodyPartType.Other => SlotFlags.NONE, 
			_ => SlotFlags.NONE, 
		});
		((EntitySystem)this).RaiseLocalEvent<CMSurgeryCanPerformStepEvent>(step, ref check, false);
		popup = check.Popup;
		validTools = check.ValidTools;
		if (check.Invalid != StepInvalidReason.None)
		{
			if (doPopup && check.Popup != null)
			{
				_popup.PopupEntity(check.Popup, user, PopupType.SmallCaution);
			}
			reason = check.Invalid;
			return false;
		}
		reason = StepInvalidReason.None;
		return true;
	}

	public bool CanPerformStep(EntityUid user, EntityUid body, BodyPartType part, EntityUid step, bool doPopup)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		string popup;
		StepInvalidReason reason;
		HashSet<EntityUid> validTools;
		return CanPerformStep(user, body, part, step, doPopup, out popup, out reason, out validTools);
	}

	public bool IsStepComplete(EntityUid body, EntityUid part, EntProtoId step)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? singleton = GetSingleton(step);
		if (singleton.HasValue)
		{
			EntityUid stepEnt = singleton.GetValueOrDefault();
			CMSurgeryStepCompleteCheckEvent ev = new CMSurgeryStepCompleteCheckEvent(body, part);
			((EntitySystem)this).RaiseLocalEvent<CMSurgeryStepCompleteCheckEvent>(stepEnt, ref ev, false);
			return !ev.Cancelled;
		}
		return false;
	}

	private bool AnyHaveComp(List<EntityUid> tools, IComponent component, out EntityUid withComp)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid tool in tools)
		{
			if (((EntitySystem)this).HasComp(tool, ((object)component).GetType()))
			{
				withComp = tool;
				return true;
			}
		}
		withComp = default(EntityUid);
		return false;
	}
}
