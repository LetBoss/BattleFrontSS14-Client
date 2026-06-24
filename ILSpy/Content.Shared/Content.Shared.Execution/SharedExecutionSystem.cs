using System;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.ActionBlocker;
using Content.Shared.Chat;
using Content.Shared.CombatMode;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Shared.Execution;

public sealed class SharedExecutionSystem : EntitySystem
{
	private static readonly bool DisableExecution = true;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedSuicideSystem _suicide;

	[Dependency]
	private SharedCombatModeSystem _combat;

	[Dependency]
	private SharedExecutionSystem _execution;

	[Dependency]
	private SharedMeleeWeaponSystem _melee;

	[Dependency]
	private IConfigurationManager _config;

	private bool _canSuicide;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ExecutionComponent, GetVerbsEvent<UtilityVerb>>((ComponentEventHandler<ExecutionComponent, GetVerbsEvent<UtilityVerb>>)OnGetInteractionsVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ExecutionComponent, GetMeleeDamageEvent>((EntityEventRefHandler<ExecutionComponent, GetMeleeDamageEvent>)OnGetMeleeDamage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ExecutionComponent, SuicideByEnvironmentEvent>((EntityEventRefHandler<ExecutionComponent, SuicideByEnvironmentEvent>)OnSuicideByEnvironment, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ExecutionComponent, ExecutionDoAfterEvent>((EntityEventRefHandler<ExecutionComponent, ExecutionDoAfterEvent>)OnExecutionDoAfter, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _config, RMCCVars.RMCEnableSuicide, (Action<bool>)delegate(bool v)
		{
			_canSuicide = v;
		}, false);
	}

	private void OnGetInteractionsVerbs(EntityUid uid, ExecutionComponent comp, GetVerbsEvent<UtilityVerb> args)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (args.Hands == null || !args.Using.HasValue || !args.CanAccess || !args.CanInteract)
		{
			return;
		}
		EntityUid attacker = args.User;
		EntityUid weapon = args.Using.Value;
		EntityUid victim = args.Target;
		if (CanBeExecuted(victim, attacker))
		{
			UtilityVerb verb = new UtilityVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0013: Unknown result type (might be due to invalid IL or missing references)
					TryStartExecutionDoAfter(weapon, victim, attacker, comp);
				},
				Impact = LogImpact.High,
				Text = base.Loc.GetString("execution-verb-name"),
				Message = base.Loc.GetString("execution-verb-message")
			};
			args.Verbs.Add(verb);
		}
	}

	private void TryStartExecutionDoAfter(EntityUid weapon, EntityUid victim, EntityUid attacker, ExecutionComponent comp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (CanBeExecuted(victim, attacker))
		{
			if (attacker == victim)
			{
				ShowExecutionInternalPopup(LocId.op_Implicit(comp.InternalSelfExecutionMessage), attacker, victim, weapon);
				ShowExecutionExternalPopup(LocId.op_Implicit(comp.ExternalSelfExecutionMessage), attacker, victim, weapon);
			}
			else
			{
				ShowExecutionInternalPopup(LocId.op_Implicit(comp.InternalMeleeExecutionMessage), attacker, victim, weapon);
				ShowExecutionExternalPopup(LocId.op_Implicit(comp.ExternalMeleeExecutionMessage), attacker, victim, weapon);
			}
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, attacker, comp.DoAfterDuration, new ExecutionDoAfterEvent(), weapon, victim, weapon)
			{
				BreakOnMove = true,
				BreakOnDamage = true,
				NeedHand = true
			};
			_doAfter.TryStartDoAfter(doAfter);
		}
	}

	public bool CanBeExecuted(EntityUid victim, EntityUid attacker)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (DisableExecution)
		{
			return false;
		}
		if (victim == attacker && !_canSuicide)
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<XenoComponent>(victim))
		{
			return false;
		}
		if (!((EntitySystem)this).HasComp<DamageableComponent>(victim))
		{
			return false;
		}
		MobStateComponent mobState = default(MobStateComponent);
		if (!((EntitySystem)this).TryComp<MobStateComponent>(victim, ref mobState))
		{
			return false;
		}
		if (_mobState.IsDead(victim, mobState))
		{
			return false;
		}
		if (!_actionBlocker.CanAttack(attacker, victim))
		{
			return false;
		}
		if (victim != attacker && _actionBlocker.CanInteract(victim, null))
		{
			return false;
		}
		return true;
	}

	private void OnGetMeleeDamage(Entity<ExecutionComponent> entity, ref GetMeleeDamageEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		MeleeWeaponComponent melee = default(MeleeWeaponComponent);
		if (((EntitySystem)this).TryComp<MeleeWeaponComponent>(Entity<ExecutionComponent>.op_Implicit(entity), ref melee) && entity.Comp.Executing)
		{
			DamageSpecifier bonus = melee.Damage * entity.Comp.DamageMultiplier - melee.Damage;
			args.Damage += bonus;
			args.ResistanceBypass = true;
		}
	}

	private void OnSuicideByEnvironment(Entity<ExecutionComponent> entity, ref SuicideByEnvironmentEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		MeleeWeaponComponent melee = default(MeleeWeaponComponent);
		if (((EntitySystem)this).TryComp<MeleeWeaponComponent>(Entity<ExecutionComponent>.op_Implicit(entity), ref melee))
		{
			string internalMsg = LocId.op_Implicit(entity.Comp.CompleteInternalSelfExecutionMessage);
			string externalMsg = LocId.op_Implicit(entity.Comp.CompleteExternalSelfExecutionMessage);
			DamageableComponent damageableComponent = default(DamageableComponent);
			if (((EntitySystem)this).TryComp<DamageableComponent>(args.Victim, ref damageableComponent))
			{
				ShowExecutionInternalPopup(internalMsg, args.Victim, args.Victim, Entity<ExecutionComponent>.op_Implicit(entity), predict: false);
				ShowExecutionExternalPopup(externalMsg, args.Victim, args.Victim, Entity<ExecutionComponent>.op_Implicit(entity));
				_audio.PlayPredicted(melee.HitSound, args.Victim, (EntityUid?)args.Victim, (AudioParams?)null);
				_suicide.ApplyLethalDamage(Entity<DamageableComponent>.op_Implicit((args.Victim, damageableComponent)), melee.Damage);
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void ShowExecutionInternalPopup(string locString, EntityUid attacker, EntityUid victim, EntityUid weapon, bool predict = true)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (predict)
		{
			_popup.PopupClient(base.Loc.GetString(locString, new(string, object)[3]
			{
				("attacker", Identity.Entity(attacker, (IEntityManager)(object)base.EntityManager)),
				("victim", Identity.Entity(victim, (IEntityManager)(object)base.EntityManager)),
				("weapon", weapon)
			}), attacker, attacker, PopupType.MediumCaution);
		}
		else
		{
			_popup.PopupEntity(base.Loc.GetString(locString, new(string, object)[3]
			{
				("attacker", Identity.Entity(attacker, (IEntityManager)(object)base.EntityManager)),
				("victim", Identity.Entity(victim, (IEntityManager)(object)base.EntityManager)),
				("weapon", weapon)
			}), attacker, attacker, PopupType.MediumCaution);
		}
	}

	private void ShowExecutionExternalPopup(string locString, EntityUid attacker, EntityUid victim, EntityUid weapon)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupEntity(base.Loc.GetString(locString, new(string, object)[3]
		{
			("attacker", Identity.Entity(attacker, (IEntityManager)(object)base.EntityManager)),
			("victim", Identity.Entity(victim, (IEntityManager)(object)base.EntityManager)),
			("weapon", weapon)
		}), attacker, Filter.PvsExcept(attacker, 2f, (IEntityManager)null), recordReplay: true, PopupType.MediumCaution);
	}

	private void OnExecutionDoAfter(Entity<ExecutionComponent> entity, ref ExecutionDoAfterEvent args)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		MeleeWeaponComponent meleeWeaponComp = default(MeleeWeaponComponent);
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled || !args.Used.HasValue || !args.Target.HasValue || !((EntitySystem)this).TryComp<MeleeWeaponComponent>(Entity<ExecutionComponent>.op_Implicit(entity), ref meleeWeaponComp))
		{
			return;
		}
		EntityUid attacker = args.User;
		EntityUid victim = args.Target.Value;
		EntityUid weapon = args.Used.Value;
		if (_execution.CanBeExecuted(victim, attacker))
		{
			bool prev = _combat.IsInCombatMode(attacker);
			_combat.SetInCombatMode(attacker, value: true);
			entity.Comp.Executing = true;
			LocId internalMsg = entity.Comp.CompleteInternalMeleeExecutionMessage;
			LocId externalMsg = entity.Comp.CompleteExternalMeleeExecutionMessage;
			if (attacker == victim)
			{
				SuicideEvent suicideEvent = new SuicideEvent(victim);
				((EntitySystem)this).RaiseLocalEvent<SuicideEvent>(victim, suicideEvent, false);
				SuicideGhostEvent suicideGhostEvent = new SuicideGhostEvent(victim);
				((EntitySystem)this).RaiseLocalEvent<SuicideGhostEvent>(victim, suicideGhostEvent, false);
			}
			else
			{
				_melee.AttemptLightAttack(attacker, weapon, meleeWeaponComp, victim);
			}
			_combat.SetInCombatMode(attacker, prev);
			entity.Comp.Executing = false;
			((HandledEntityEventArgs)args).Handled = true;
			if (attacker != victim)
			{
				_execution.ShowExecutionInternalPopup(LocId.op_Implicit(internalMsg), attacker, victim, Entity<ExecutionComponent>.op_Implicit(entity));
				_execution.ShowExecutionExternalPopup(LocId.op_Implicit(externalMsg), attacker, victim, Entity<ExecutionComponent>.op_Implicit(entity));
			}
		}
	}
}
