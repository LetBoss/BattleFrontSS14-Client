using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Emag.Components;
using Content.Shared.Emag.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.NPC.Components;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Silicons.Bots;

public sealed class MedibotSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private EmagSystem _emag;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainer;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EmaggableMedibotComponent, GotEmaggedEvent>((ComponentEventRefHandler<EmaggableMedibotComponent, GotEmaggedEvent>)OnEmagged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MedibotComponent, UserActivateInWorldEvent>((EntityEventRefHandler<MedibotComponent, UserActivateInWorldEvent>)OnInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MedibotComponent, MedibotInjectDoAfterEvent>((ComponentEventRefHandler<MedibotComponent, MedibotInjectDoAfterEvent>)OnInject, (Type[])null, (Type[])null);
	}

	private void OnEmagged(EntityUid uid, EmaggableMedibotComponent comp, ref GotEmaggedEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		MedibotComponent medibot = default(MedibotComponent);
		if (!_emag.CompareFlag(args.Type, EmagType.Interaction) || _emag.CheckFlag(uid, EmagType.Interaction) || !((EntitySystem)this).TryComp<MedibotComponent>(uid, ref medibot))
		{
			return;
		}
		foreach (var (state, treatment) in comp.Replacements)
		{
			medibot.Treatments[state] = treatment;
		}
		args.Handled = true;
	}

	private void OnInteract(Entity<MedibotComponent> medibot, ref UserActivateInWorldEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (CheckInjectable(medibot, args.Target, manual: true))
		{
			_doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, 2f, new MedibotInjectDoAfterEvent(), args.User, args.Target)
			{
				BlockDuplicate = true,
				BreakOnMove = true
			});
		}
	}

	private void OnInject(EntityUid uid, MedibotComponent comp, ref MedibotInjectDoAfterEvent args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			EntityUid? target = args.Target;
			if (target.HasValue)
			{
				EntityUid target2 = target.GetValueOrDefault();
				TryInject(Entity<MedibotComponent>.op_Implicit(uid), target2);
			}
		}
	}

	public bool TryGetTreatment(MedibotComponent comp, MobState state, [NotNullWhen(true)] out MedibotTreatment? treatment)
	{
		return comp.Treatments.TryGetValue(state, out treatment);
	}

	public bool CheckInjectable(Entity<MedibotComponent?> medibot, EntityUid target, bool manual = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MedibotComponent>(Entity<MedibotComponent>.op_Implicit(medibot), ref medibot.Comp, false))
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<NPCRecentlyInjectedComponent>(target))
		{
			_popup.PopupClient(base.Loc.GetString("medibot-recently-injected"), Entity<MedibotComponent>.op_Implicit(medibot), Entity<MedibotComponent>.op_Implicit(medibot));
			return false;
		}
		MobStateComponent mobState = default(MobStateComponent);
		if (!((EntitySystem)this).TryComp<MobStateComponent>(target, ref mobState))
		{
			return false;
		}
		DamageableComponent damageable = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<DamageableComponent>(target, ref damageable))
		{
			return false;
		}
		if (!_solutionContainer.TryGetInjectableSolution(Entity<InjectableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(target), out Entity<SolutionComponent>? _, out Solution _))
		{
			return false;
		}
		if (mobState.CurrentState != MobState.Alive && mobState.CurrentState != MobState.Critical)
		{
			_popup.PopupClient(base.Loc.GetString("medibot-target-dead"), Entity<MedibotComponent>.op_Implicit(medibot), Entity<MedibotComponent>.op_Implicit(medibot));
			return false;
		}
		FixedPoint2 total = damageable.TotalDamage;
		if (total == 0 && !((EntitySystem)this).HasComp<EmaggedComponent>(Entity<MedibotComponent>.op_Implicit(medibot)))
		{
			_popup.PopupClient(base.Loc.GetString("medibot-target-healthy"), Entity<MedibotComponent>.op_Implicit(medibot), Entity<MedibotComponent>.op_Implicit(medibot));
			return false;
		}
		if (!TryGetTreatment(medibot.Comp, mobState.CurrentState, out MedibotTreatment treatment) || (!treatment.IsValid(total) && !manual))
		{
			return false;
		}
		return true;
	}

	public bool TryInject(Entity<MedibotComponent?> medibot, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MedibotComponent>(Entity<MedibotComponent>.op_Implicit(medibot), ref medibot.Comp, false))
		{
			return false;
		}
		if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(medibot.Owner), Entity<TransformComponent>.op_Implicit(target)))
		{
			return false;
		}
		MobStateComponent mobState = default(MobStateComponent);
		if (!((EntitySystem)this).TryComp<MobStateComponent>(target, ref mobState))
		{
			return false;
		}
		if (!TryGetTreatment(medibot.Comp, mobState.CurrentState, out MedibotTreatment treatment))
		{
			return false;
		}
		if (!_solutionContainer.TryGetInjectableSolution(Entity<InjectableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(target), out Entity<SolutionComponent>? injectable, out Solution _))
		{
			return false;
		}
		((EntitySystem)this).EnsureComp<NPCRecentlyInjectedComponent>(target);
		_solutionContainer.TryAddReagent(injectable.Value, ProtoId<ReagentPrototype>.op_Implicit(treatment.Reagent), treatment.Quantity, out var _);
		_popup.PopupEntity(base.Loc.GetString("hypospray-component-feel-prick-message"), target, target);
		_popup.PopupClient(base.Loc.GetString("medibot-target-injected"), Entity<MedibotComponent>.op_Implicit(medibot), Entity<MedibotComponent>.op_Implicit(medibot));
		_audio.PlayPredicted(medibot.Comp.InjectSound, Entity<MedibotComponent>.op_Implicit(medibot), (EntityUid?)Entity<MedibotComponent>.op_Implicit(medibot), (AudioParams?)null);
		return true;
	}
}
