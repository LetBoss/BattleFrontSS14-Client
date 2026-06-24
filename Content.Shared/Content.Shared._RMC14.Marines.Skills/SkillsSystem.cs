using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Flash;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.Prototypes;
using Content.Shared.Throwing;
using Content.Shared.UserInterface;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Marines.Skills;

public sealed class SkillsSystem : EntitySystem
{
	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainerSystem;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private ItemToggleSystem _toggle;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private RMCReagentSystem _reagent;

	private static readonly EntProtoId<SkillDefinitionComponent> MeleeSkill = EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillMeleeWeapons");

	private EntityQuery<SkillsComponent> _skillsQuery;

	private SortedSet<(string, int)> _skillsSorted = new SortedSet<(string, int)>(Comparer<(string, int)>.Create(((string, int) a, (string, int) b) => string.Compare(a.Item1, b.Item1, StringComparison.Ordinal)));

	public ImmutableArray<EntProtoId<SkillDefinitionComponent>> Skills { get; private set; }

	public ImmutableDictionary<string, EntProtoId<SkillDefinitionComponent>> SkillNames { get; private set; } = ImmutableDictionary<string, EntProtoId<SkillDefinitionComponent>>.Empty;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_skillsQuery = ((EntitySystem)this).GetEntityQuery<SkillsComponent>();
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnPrototypesReloaded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GetMeleeDamageEvent>((EntityEventRefHandler<GetMeleeDamageEvent>)OnGetMeleeDamage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SkillsComponent, MapInitEvent>((EntityEventRefHandler<SkillsComponent, MapInitEvent>)OnSkillsMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SkillsComponent, GetVerbsEvent<ExamineVerb>>((EntityEventRefHandler<SkillsComponent, GetVerbsEvent<ExamineVerb>>)OnSkillsVerbExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MedicallyUnskilledDoAfterComponent, AttemptHyposprayUseEvent>((EntityEventRefHandler<MedicallyUnskilledDoAfterComponent, AttemptHyposprayUseEvent>)OnAttemptHyposprayUse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RequiresSkillComponent, BeforeRangedInteractEvent>((EntityEventRefHandler<RequiresSkillComponent, BeforeRangedInteractEvent>)OnRequiresSkillBeforeRangedInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RequiresSkillComponent, ActivatableUIOpenAttemptEvent>((EntityEventRefHandler<RequiresSkillComponent, ActivatableUIOpenAttemptEvent>)OnRequiresSkillActivatableUIOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RequiresSkillComponent, UseInHandEvent>((EntityEventRefHandler<RequiresSkillComponent, UseInHandEvent>)OnRequiresSkillUseInHand, new Type[2]
		{
			typeof(HypospraySystem),
			typeof(SharedFlashSystem)
		}, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MeleeRequiresSkillComponent, AttemptMeleeEvent>((EntityEventRefHandler<MeleeRequiresSkillComponent, AttemptMeleeEvent>)OnMeleeRequiresSkillAttemptMelee, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MeleeRequiresSkillComponent, ThrowItemAttemptEvent>((EntityEventRefHandler<MeleeRequiresSkillComponent, ThrowItemAttemptEvent>)OnMeleeRequiresSkillThrowAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MeleeRequiresSkillComponent, UseInHandEvent>((EntityEventRefHandler<MeleeRequiresSkillComponent, UseInHandEvent>)OnMeleeRequiresSkillUseInHand, new Type[2]
		{
			typeof(HypospraySystem),
			typeof(SharedFlashSystem)
		}, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemToggleRequiresSkillComponent, ItemToggleActivateAttemptEvent>((EntityEventRefHandler<ItemToggleRequiresSkillComponent, ItemToggleActivateAttemptEvent>)OnItemToggleRequiresSkill, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemToggleDeactivateUnskilledComponent, GotEquippedEvent>((EntityEventRefHandler<ItemToggleDeactivateUnskilledComponent, GotEquippedEvent>)OnItemToggleDeactivateUnskilled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReagentExaminationRequiresSkillComponent, ExaminedEvent>((EntityEventRefHandler<ReagentExaminationRequiresSkillComponent, ExaminedEvent>)OnExamineReagentContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ExamineRequiresSkillComponent, ExaminedEvent>((EntityEventRefHandler<ExamineRequiresSkillComponent, ExaminedEvent>)OnExamineRequiresSkill, (Type[])null, (Type[])null);
		ReloadPrototypes();
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
	{
		if (ev.WasModified<EntityPrototype>())
		{
			ReloadPrototypes();
		}
	}

	private void OnGetMeleeDamage(ref GetMeleeDamageEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.User == args.Weapon) && GetSkill(Entity<SkillsComponent>.op_Implicit(args.User), MeleeSkill) > 0)
		{
			args.Damage = ApplyMeleeSkillModifier(args.User, args.Damage);
		}
	}

	private void OnSkillsMapInit(Entity<SkillsComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		SkillPresetPrototype skillPreset = default(SkillPresetPrototype);
		if (_prototypes.TryIndex<SkillPresetPrototype>(ent.Comp.Preset, ref skillPreset))
		{
			ent.Comp.Skills = skillPreset.Skills;
			((EntitySystem)this).Dirty<SkillsComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnSkillsVerbExamine(Entity<SkillsComponent> ent, ref GetVerbsEvent<ExamineVerb> args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (!args.CanInteract || !args.CanAccess || ((EntitySystem)this).HasComp<XenoComponent>(user))
		{
			return;
		}
		_skillsSorted.Clear();
		EntityPrototype proto = default(EntityPrototype);
		foreach (var (id, value) in ent.Comp.Skills)
		{
			if (_prototypes.TryIndex(EntProtoId<SkillDefinitionComponent>.op_Implicit(id), ref proto))
			{
				_skillsSorted.Add((proto.Name, value));
			}
		}
		FormattedMessage msg = new FormattedMessage();
		if (_skillsSorted.Count == 0)
		{
			msg.AddMarkupPermissive(base.Loc.GetString("rmc-skills-examine-none", (ValueTuple<string, object>)("target", ent)));
		}
		else
		{
			foreach (var (name, level) in _skillsSorted)
			{
				if (level != 0)
				{
					msg.AddMarkupPermissive(base.Loc.GetString("rmc-skills-examine-skill", (ValueTuple<string, object>)("name", name), (ValueTuple<string, object>)("level", level)));
					msg.PushNewline();
				}
			}
		}
		_examine.AddDetailedExamineVerb(args, (Component)(object)Entity<SkillsComponent>.op_Implicit(ent), msg, base.Loc.GetString("rmc-skills-examine", (ValueTuple<string, object>)("target", ent)), "/Textures/Interface/students-cap.svg.192dpi.png", base.Loc.GetString("rmc-skills-examine", (ValueTuple<string, object>)("target", ent)));
	}

	private void OnAttemptHyposprayUse(Entity<MedicallyUnskilledDoAfterComponent> ent, ref AttemptHyposprayUseEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!HasSkill(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.Skill, ent.Comp.Min))
		{
			args.MaxDoAfter(ent.Comp.DoAfter);
		}
	}

	private void OnRequiresSkillBeforeRangedInteract(Entity<RequiresSkillComponent> ent, ref BeforeRangedInteractEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !HasAllSkills(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.Skills))
		{
			string msg = base.Loc.GetString("rmc-skills-cant-use", (ValueTuple<string, object>)("item", args.Used));
			_popup.PopupClient(msg, args.User, PopupType.SmallCaution);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnRequiresSkillActivatableUIOpenAttempt(Entity<RequiresSkillComponent> ent, ref ActivatableUIOpenAttemptEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !HasAllSkills(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.Skills))
		{
			string msg = base.Loc.GetString("rmc-skills-no-training", (ValueTuple<string, object>)("target", ent));
			_popup.PopupClient(msg, args.User, PopupType.SmallCaution);
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnRequiresSkillUseInHand(Entity<RequiresSkillComponent> ent, ref UseInHandEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!HasAllSkills(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.Skills))
		{
			string msg = base.Loc.GetString("rmc-skills-cant-use", (ValueTuple<string, object>)("item", ent));
			_popup.PopupClient(msg, args.User, args.User, PopupType.SmallCaution);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnMeleeRequiresSkillAttemptMelee(Entity<MeleeRequiresSkillComponent> ent, ref AttemptMeleeEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (!HasAllSkills(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.Skills))
		{
			string msg = base.Loc.GetString("rmc-skills-cant-use", (ValueTuple<string, object>)("item", ent));
			_popup.PopupClient(msg, args.User, args.User, PopupType.SmallCaution);
			args.Cancelled = true;
		}
	}

	private void OnMeleeRequiresSkillThrowAttempt(Entity<MeleeRequiresSkillComponent> ent, ref ThrowItemAttemptEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!HasAllSkills(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.Skills))
		{
			if (_net.IsServer)
			{
				string msg = base.Loc.GetString("rmc-skills-cant-use", (ValueTuple<string, object>)("item", ent));
				_popup.PopupEntity(msg, args.User, args.User, PopupType.SmallCaution);
			}
			args.Cancelled = true;
		}
	}

	private void OnMeleeRequiresSkillUseInHand(Entity<MeleeRequiresSkillComponent> ent, ref UseInHandEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!HasAllSkills(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.Skills))
		{
			string msg = base.Loc.GetString("rmc-skills-cant-use", (ValueTuple<string, object>)("item", ent));
			_popup.PopupClient(msg, args.User, args.User, PopupType.SmallCaution);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnItemToggleRequiresSkill(Entity<ItemToggleRequiresSkillComponent> ent, ref ItemToggleActivateAttemptEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (args.User.HasValue && !HasAllSkills(Entity<SkillsComponent>.op_Implicit(args.User.Value), ent.Comp.Skills))
		{
			args.Popup = base.Loc.GetString("rmc-skills-cant-use", (ValueTuple<string, object>)("item", ent));
			args.Cancelled = true;
		}
	}

	private void OnItemToggleDeactivateUnskilled(Entity<ItemToggleDeactivateUnskilledComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (!HasAllSkills(Entity<SkillsComponent>.op_Implicit(args.Equipee), ent.Comp.Skills) && _toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(ent.Owner)) && _toggle.TryDeactivate(Entity<ItemToggleComponent>.op_Implicit(ent.Owner), args.Equipee) && ent.Comp.Popup.HasValue)
		{
			ILocalizationManager loc = base.Loc;
			LocId? popup = ent.Comp.Popup;
			string msg = loc.GetString(popup.HasValue ? LocId.op_Implicit(popup.GetValueOrDefault()) : null, (ValueTuple<string, object>)("item", ent));
			_popup.PopupClient(msg, args.Equipee, args.Equipee, PopupType.SmallCaution);
		}
	}

	private void OnExamineReagentContainer(Entity<ReagentExaminationRequiresSkillComponent> ent, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		if (!HasAllSkills(Entity<SkillsComponent>.op_Implicit(args.Examiner), ent.Comp.Skills))
		{
			if (ent.Comp.UnskilledExamine.HasValue)
			{
				using (args.PushGroup("ReagentExaminationRequiresSkillComponent"))
				{
					ExaminedEvent obj = args;
					ILocalizationManager loc = base.Loc;
					LocId? unskilledExamine = ent.Comp.UnskilledExamine;
					obj.PushMarkup(loc.GetString(unskilledExamine.HasValue ? LocId.op_Implicit(unskilledExamine.GetValueOrDefault()) : null));
					return;
				}
			}
			return;
		}
		EntityUid entityToExamine = args.Examined;
		if (ent.Comp.ContainerId != null)
		{
			BaseContainer container = default(BaseContainer);
			EntityUid? contained = default(EntityUid?);
			if (!_container.TryGetContainer(args.Examined, ent.Comp.ContainerId, ref container, (ContainerManagerComponent)null) || !Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)container.ContainedEntities, ref contained))
			{
				if (!ent.Comp.NoContainerExamine.HasValue)
				{
					return;
				}
				using (args.PushGroup("ReagentExaminationRequiresSkillComponent"))
				{
					ExaminedEvent obj2 = args;
					ILocalizationManager loc2 = base.Loc;
					LocId? unskilledExamine = ent.Comp.NoContainerExamine;
					obj2.PushMarkup(loc2.GetString(unskilledExamine.HasValue ? LocId.op_Implicit(unskilledExamine.GetValueOrDefault()) : null, (ValueTuple<string, object>)("target", ent.Owner)));
					return;
				}
			}
			entityToExamine = contained.Value;
		}
		SolutionContainerManagerComponent solutionContainerManager = default(SolutionContainerManagerComponent);
		if (!((EntitySystem)this).TryComp<SolutionContainerManagerComponent>(entityToExamine, ref solutionContainerManager))
		{
			return;
		}
		List<ReagentQuantity> foundReagents = new List<ReagentQuantity>();
		foreach (string solutionContainerId in solutionContainerManager.Containers)
		{
			if (!_solutionContainerSystem.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entityToExamine), solutionContainerId, out Entity<SolutionComponent>? _, out Solution solution))
			{
				continue;
			}
			foreach (ReagentQuantity reagent in solution.Contents)
			{
				foundReagents.Add(reagent);
			}
		}
		if (!foundReagents.Any())
		{
			using (args.PushGroup("ReagentExaminationRequiresSkillComponent"))
			{
				args.PushMarkup(base.Loc.GetString(LocId.op_Implicit(ent.Comp.SkilledExamineNone), (ValueTuple<string, object>)("target", ent.Owner)));
				return;
			}
		}
		string reagentsText = string.Join("; ", foundReagents.Select((ReagentQuantity r) => $"{_reagent.Index(ProtoId<ReagentPrototype>.op_Implicit(r.Reagent.Prototype)).LocalizedName}({r.Quantity}u)"));
		using (args.PushGroup("ReagentExaminationRequiresSkillComponent"))
		{
			args.PushMarkup(base.Loc.GetString(LocId.op_Implicit(ent.Comp.SkilledExamineContains), (ValueTuple<string, object>)("target", ent.Owner), (ValueTuple<string, object>)("reagents", reagentsText)));
		}
	}

	private void OnExamineRequiresSkill(Entity<ExamineRequiresSkillComponent> ent, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!HasAllSkills(Entity<SkillsComponent>.op_Implicit(args.Examiner), ent.Comp.Skills))
		{
			if (ent.Comp.UnskilledExamine.HasValue)
			{
				using (args.PushGroup("ExamineRequiresSkillComponent", ent.Comp.ExaminePriority))
				{
					ExaminedEvent obj = args;
					ILocalizationManager loc = base.Loc;
					LocId? unskilledExamine = ent.Comp.UnskilledExamine;
					obj.PushMarkup(loc.GetString(unskilledExamine.HasValue ? LocId.op_Implicit(unskilledExamine.GetValueOrDefault()) : null));
					return;
				}
			}
			return;
		}
		using (args.PushGroup("ExamineRequiresSkillComponent", ent.Comp.ExaminePriority))
		{
			args.PushMarkup(base.Loc.GetString(LocId.op_Implicit(ent.Comp.SkilledExamine)));
		}
	}

	private void ReloadPrototypes()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		ImmutableArray<EntProtoId<SkillDefinitionComponent>>.Builder skillsArray = ImmutableArray.CreateBuilder<EntProtoId<SkillDefinitionComponent>>();
		ImmutableDictionary<string, EntProtoId<SkillDefinitionComponent>>.Builder skillsDict = ImmutableDictionary.CreateBuilder<string, EntProtoId<SkillDefinitionComponent>>();
		foreach (EntityPrototype prototype in _prototypes.EnumeratePrototypes<EntityPrototype>())
		{
			if (prototype.HasComponent<SkillDefinitionComponent>((IComponentFactory?)null))
			{
				string id = prototype.ID;
				string name = prototype.Name.Replace(" ", string.Empty);
				skillsArray.Add(EntProtoId<SkillDefinitionComponent>.op_Implicit(id));
				if (!skillsDict.TryAdd(name, EntProtoId<SkillDefinitionComponent>.op_Implicit(id)))
				{
					string old = skillsDict.GetValueOrDefault(name).Id;
					string msg = $"Duplicate skill name found: {name}, old: {old}, new: {id}";
					((EntitySystem)this).Log.Error(msg);
				}
			}
		}
		Skills = skillsArray.ToImmutable();
		SkillNames = skillsDict.ToImmutable();
	}

	public TimeSpan GetDelay(EntityUid user, EntityUid tool)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		MedicallyUnskilledDoAfterComponent doAfter = default(MedicallyUnskilledDoAfterComponent);
		if (!((EntitySystem)this).TryComp<MedicallyUnskilledDoAfterComponent>(tool, ref doAfter) || doAfter.Min <= 0)
		{
			return default(TimeSpan);
		}
		if (!HasSkill(Entity<SkillsComponent>.op_Implicit(user), doAfter.Skill, doAfter.Min))
		{
			return doAfter.DoAfter;
		}
		return default(TimeSpan);
	}

	public int GetSkill(Entity<SkillsComponent?> ent, EntProtoId<SkillDefinitionComponent> skill)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (skill == default(EntProtoId<SkillDefinitionComponent>))
		{
			string msg = $"Empty skill {skill} passed to {"GetSkill"}!";
			((EntitySystem)this).Log.Error(msg);
		}
		if (!_skillsQuery.Resolve(Entity<SkillsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return 0;
		}
		return ent.Comp.Skills.GetValueOrDefault(skill);
	}

	public bool HasSkills(Entity<SkillsComponent?> ent, SkillWhitelist whitelist)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return HasAllSkills(ent, whitelist.All);
	}

	public bool HasAllSkills(Entity<SkillsComponent?> ent, Dictionary<EntProtoId<SkillDefinitionComponent>, int> required)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<BypassSkillChecksComponent>(Entity<SkillsComponent>.op_Implicit(ent)))
		{
			return true;
		}
		_skillsQuery.Resolve(Entity<SkillsComponent>.op_Implicit(ent), ref ent.Comp, false);
		foreach (var (requiredSkill, requiredLevel) in required)
		{
			if (requiredLevel > 0)
			{
				if (ent.Comp == null)
				{
					return false;
				}
				if (!ent.Comp.Skills.TryGetValue(requiredSkill, out var level) || level < requiredLevel)
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool HasAllSkills(Entity<SkillsComponent?> ent, List<Skill> allRequired)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<BypassSkillChecksComponent>(Entity<SkillsComponent>.op_Implicit(ent)))
		{
			return true;
		}
		_skillsQuery.Resolve(Entity<SkillsComponent>.op_Implicit(ent), ref ent.Comp, false);
		Span<Skill> span = CollectionsMarshal.AsSpan(allRequired);
		for (int i = 0; i < span.Length; i++)
		{
			ref Skill required = ref span[i];
			if (required.Level > 0)
			{
				if (ent.Comp == null)
				{
					return false;
				}
				if (!ent.Comp.Skills.TryGetValue(required.Type, out var level) || level < required.Level)
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool HasAnySkills(Entity<SkillsComponent?> ent, Dictionary<EntProtoId<SkillDefinitionComponent>, int> anyRequired)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<BypassSkillChecksComponent>(Entity<SkillsComponent>.op_Implicit(ent)))
		{
			return true;
		}
		_skillsQuery.Resolve(Entity<SkillsComponent>.op_Implicit(ent), ref ent.Comp, false);
		foreach (var (requiredSkill, requiredLevel) in anyRequired)
		{
			if (requiredLevel > 0 && ent.Comp != null && ent.Comp.Skills.TryGetValue(requiredSkill, out var level) && level >= requiredLevel)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnySkills(Entity<SkillsComponent?> ent, List<Skill> anyRequired)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<BypassSkillChecksComponent>(Entity<SkillsComponent>.op_Implicit(ent)))
		{
			return true;
		}
		_skillsQuery.Resolve(Entity<SkillsComponent>.op_Implicit(ent), ref ent.Comp, false);
		Span<Skill> span = CollectionsMarshal.AsSpan(anyRequired);
		for (int i = 0; i < span.Length; i++)
		{
			ref Skill required = ref span[i];
			if (required.Level > 0 && ent.Comp != null && ent.Comp.Skills.TryGetValue(required.Type, out var level) && level >= required.Level)
			{
				return false;
			}
		}
		return true;
	}

	public bool HasSkill(Entity<SkillsComponent?> ent, EntProtoId<SkillDefinitionComponent> skill, int required)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<BypassSkillChecksComponent>(Entity<SkillsComponent>.op_Implicit(ent)))
		{
			return true;
		}
		if (required <= 0)
		{
			return true;
		}
		if (_skillsQuery.Resolve(Entity<SkillsComponent>.op_Implicit(ent), ref ent.Comp, false) && ent.Comp.Skills.TryGetValue(skill, out var level))
		{
			return level >= required;
		}
		return false;
	}

	public void IncrementSkill(Entity<SkillsComponent?> ent, EntProtoId<SkillDefinitionComponent> skill, int by = 1)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		ref SkillsComponent comp = ref ent.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<SkillsComponent>(Entity<SkillsComponent>.op_Implicit(ent));
		}
		SetSkill(ent, skill, ent.Comp.Skills.GetValueOrDefault(skill) + by);
	}

	public void IncrementSkills(Entity<SkillsComponent?> ent, List<EntProtoId<SkillDefinitionComponent>> skills, int by = 1)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		ref SkillsComponent comp = ref ent.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<SkillsComponent>(Entity<SkillsComponent>.op_Implicit(ent));
		}
		Span<EntProtoId<SkillDefinitionComponent>> span = CollectionsMarshal.AsSpan(skills);
		for (int i = 0; i < span.Length; i++)
		{
			ref EntProtoId<SkillDefinitionComponent> skill = ref span[i];
			SetSkill(ent, skill, ent.Comp.Skills.GetValueOrDefault(skill) + by);
		}
	}

	public void IncrementSkills(Entity<SkillsComponent?> ent, HashSet<EntProtoId<SkillDefinitionComponent>> skills, int by = 1)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		ref SkillsComponent comp = ref ent.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<SkillsComponent>(Entity<SkillsComponent>.op_Implicit(ent));
		}
		foreach (EntProtoId<SkillDefinitionComponent> skill in skills)
		{
			SetSkill(ent, skill, ent.Comp.Skills.GetValueOrDefault(skill) + by);
		}
	}

	public void RemoveAllSkills(Entity<SkillsComponent?> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (_skillsQuery.Resolve(Entity<SkillsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			ent.Comp.Skills.Clear();
			((EntitySystem)this).Dirty<SkillsComponent>(ent, (MetaDataComponent)null);
		}
	}

	public void SetSkill(Entity<SkillsComponent?> ent, EntProtoId<SkillDefinitionComponent> skill, int to)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (skill == default(EntProtoId<SkillDefinitionComponent>))
		{
			string msg = $"Empty skill {skill} passed to {"SetSkill"}!";
			((EntitySystem)this).Log.Error(msg);
		}
		else
		{
			ref SkillsComponent comp = ref ent.Comp;
			if (comp == null)
			{
				comp = ((EntitySystem)this).EnsureComp<SkillsComponent>(Entity<SkillsComponent>.op_Implicit(ent));
			}
			ent.Comp.Skills[skill] = to;
			((EntitySystem)this).Dirty<SkillsComponent>(ent, (MetaDataComponent)null);
		}
	}

	public void SetSkills(Entity<SkillsComponent?> ent, Dictionary<EntProtoId<SkillDefinitionComponent>, int> to)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		ref SkillsComponent comp = ref ent.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<SkillsComponent>(Entity<SkillsComponent>.op_Implicit(ent));
		}
		foreach (var (skill, level) in to)
		{
			ent.Comp.Skills[skill] = level;
		}
		((EntitySystem)this).Dirty<SkillsComponent>(ent, (MetaDataComponent)null);
	}

	public void SetSkills(Entity<SkillsComponent?> ent, List<Skill> to)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		ref SkillsComponent comp = ref ent.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<SkillsComponent>(Entity<SkillsComponent>.op_Implicit(ent));
		}
		Span<Skill> span = CollectionsMarshal.AsSpan(to);
		for (int i = 0; i < span.Length; i++)
		{
			ref Skill skill = ref span[i];
			ent.Comp.Skills[skill.Type] = skill.Level;
		}
		((EntitySystem)this).Dirty<SkillsComponent>(ent, (MetaDataComponent)null);
	}

	public void SetSkills(Entity<SkillsComponent?> ent, HashSet<Skill> to)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		ref SkillsComponent comp = ref ent.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<SkillsComponent>(Entity<SkillsComponent>.op_Implicit(ent));
		}
		foreach (Skill skill in to)
		{
			ent.Comp.Skills[skill.Type] = skill.Level;
		}
		((EntitySystem)this).Dirty<SkillsComponent>(ent, (MetaDataComponent)null);
	}

	public float GetSkillDelayMultiplier(Entity<SkillsComponent?> user, EntProtoId<SkillDefinitionComponent> definition, float[]? multipliers = null)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		SkillDefinitionComponent definitionComp = default(SkillDefinitionComponent);
		if (!definition.TryGet(ref definitionComp, _prototypes, _compFactory))
		{
			return 1f;
		}
		if (multipliers == null)
		{
			multipliers = definitionComp.DelayMultipliers;
		}
		if (multipliers.Length == 0)
		{
			return 1f;
		}
		int skill = GetSkill(user, definition);
		float multiplier = default(float);
		if (!Extensions.TryGetValue<float>((IList<float>)multipliers, skill, ref multiplier))
		{
			return multipliers[^1];
		}
		return multiplier;
	}

	public DamageSpecifier ApplyMeleeSkillModifier(EntityUid user, DamageSpecifier damage)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		int skill = GetSkill(Entity<SkillsComponent>.op_Implicit(user), MeleeSkill);
		return damage * (1.0 + 0.25 * (double)skill);
	}
}
