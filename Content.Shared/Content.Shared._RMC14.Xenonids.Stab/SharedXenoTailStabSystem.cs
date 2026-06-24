using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.Barricade;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.IdentityManagement;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.GasToggle;
using Content.Shared._RMC14.Xenonids.Neurotoxin;
using Content.Shared._RMC14.Xenonids.Rotate;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Stab;

public abstract class SharedXenoTailStabSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedMeleeWeaponSystem _melee;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedDirectionalAttackBlockSystem _directionBlock;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainer;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private XenoRotateSystem _rotate;

	[Dependency]
	private RMCDazedSystem _daze;

	[Dependency]
	private RMCCameraShakeSystem _cameraShake;

	[Dependency]
	private RMCSizeStunSystem _size;

	protected Box2Rotated LastTailAttack;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoTailStabComponent, XenoTailStabEvent>((EntityEventRefHandler<XenoTailStabComponent, XenoTailStabEvent>)OnXenoTailStab, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTailStabComponent, XenoGasToggleActionEvent>((EntityEventRefHandler<XenoTailStabComponent, XenoGasToggleActionEvent>)OnXenoGasToggle, (Type[])null, (Type[])null);
	}

	private void OnXenoGasToggle(Entity<XenoTailStabComponent> stab, ref XenoGasToggleActionEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (stab.Comp.Toggle)
		{
			stab.Comp.InjectNeuro = !stab.Comp.InjectNeuro;
		}
	}

	private void OnXenoTailStab(Entity<XenoTailStabComponent> stab, ref XenoTailStabEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a05: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_083b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0840: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_088f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		//IL_089d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_081f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0824: Unknown result type (might be due to invalid IL or missing references)
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_090b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0913: Unknown result type (might be due to invalid IL or missing references)
		//IL_095e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0963: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0781: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0794: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transform = default(TransformComponent);
		if (!_actionBlocker.CanAttack(Entity<XenoTailStabComponent>.op_Implicit(stab)) || !((EntitySystem)this).TryComp(Entity<XenoTailStabComponent>.op_Implicit(stab), ref transform))
		{
			return;
		}
		MapCoordinates userCoords = _transform.GetMapCoordinates(Entity<XenoTailStabComponent>.op_Implicit(stab), transform);
		if (userCoords.MapId == MapId.Nullspace)
		{
			return;
		}
		MapCoordinates targetCoords = _transform.ToMapCoordinates(args.Target, true);
		if (userCoords.MapId != targetCoords.MapId)
		{
			return;
		}
		MeleeWeaponComponent melee = default(MeleeWeaponComponent);
		if (((EntitySystem)this).TryComp<MeleeWeaponComponent>(Entity<XenoTailStabComponent>.op_Implicit(stab), ref melee))
		{
			if (_timing.CurTime < melee.NextAttack)
			{
				return;
			}
			melee.NextAttack = _timing.CurTime + TimeSpan.FromSeconds(1L);
			((EntitySystem)this).Dirty(Entity<XenoTailStabComponent>.op_Implicit(stab), (IComponent)(object)melee, (MetaDataComponent)null);
		}
		bool damaged = false;
		DamageSpecifier damage = new DamageSpecifier(stab.Comp.TailDamage);
		RMCGetTailStabBonusDamageEvent eve = new RMCGetTailStabBonusDamageEvent(new DamageSpecifier());
		((EntitySystem)this).RaiseLocalEvent<RMCGetTailStabBonusDamageEvent>(Entity<XenoTailStabComponent>.op_Implicit(stab), ref eve, false);
		damage += eve.Damage;
		Angle val2;
		if (!args.Entity.HasValue || ((EntitySystem)this).TerminatingOrDeleted(args.Entity, (MetaDataComponent)null) || !_xeno.CanAbilityAttackTarget(Entity<XenoTailStabComponent>.op_Implicit(stab), args.Entity.Value, canAttackBarricades: true))
		{
			MeleeHitEvent missEvent = new MeleeHitEvent(new List<EntityUid>(), Entity<XenoTailStabComponent>.op_Implicit(stab), Entity<XenoTailStabComponent>.op_Implicit(stab), damage, null);
			((EntitySystem)this).RaiseLocalEvent<MeleeHitEvent>(Entity<XenoTailStabComponent>.op_Implicit(stab), missEvent, false);
			XenoTailStabActionComponent actionComp = default(XenoTailStabActionComponent);
			foreach (Entity<ActionComponent> action in _actions.GetActions(Entity<XenoTailStabComponent>.op_Implicit(stab)))
			{
				if (((EntitySystem)this).TryComp<XenoTailStabActionComponent>(Entity<ActionComponent>.op_Implicit(action), ref actionComp))
				{
					_actions.SetCooldown(action.AsNullable(), actionComp.MissCooldown);
				}
			}
		}
		else
		{
			((HandledEntityEventArgs)args).Handled = true;
			EntityUid hit = args.Entity.Value;
			MeleeHitEvent hitEvent = new MeleeHitEvent(new List<EntityUid> { hit }, Entity<XenoTailStabComponent>.op_Implicit(stab), Entity<XenoTailStabComponent>.op_Implicit(stab), damage, null);
			((EntitySystem)this).RaiseLocalEvent<MeleeHitEvent>(Entity<XenoTailStabComponent>.op_Implicit(stab), hitEvent, false);
			if (!((HandledEntityEventArgs)hitEvent).Handled)
			{
				_interaction.DoContactInteraction(Entity<XenoTailStabComponent>.op_Implicit(stab), Entity<XenoTailStabComponent>.op_Implicit(stab));
				_interaction.DoContactInteraction(Entity<XenoTailStabComponent>.op_Implicit(stab), hit);
				Vector2 targetPosition = _transform.GetMoverCoordinates(hit).Position;
				Vector2 userPosition = _transform.GetMoverCoordinates(Entity<XenoTailStabComponent>.op_Implicit(stab)).Position;
				foreach (NetEntity potentialTarget in ((EntitySystem)this).GetNetEntityList(_melee.ArcRayCast(userPosition, DirectionExtensions.ToWorldAngle(targetPosition - userPosition), Angle.op_Implicit(0f), stab.Comp.TailRange.Float(), _transform.GetMapId(Entity<TransformComponent>.op_Implicit(stab.Owner)), Entity<XenoTailStabComponent>.op_Implicit(stab)).ToList()))
				{
					EntityUid target = ((EntitySystem)this).GetEntity(potentialTarget);
					if (_directionBlock.IsAttackBlocked(Entity<XenoTailStabComponent>.op_Implicit(stab), target))
					{
						hit = target;
						break;
					}
				}
				Filter filter = Filter.Pvs(transform.Coordinates, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == stab.Owner));
				AttackedEvent attackedEv = new AttackedEvent(Entity<XenoTailStabComponent>.op_Implicit(stab), Entity<XenoTailStabComponent>.op_Implicit(stab), args.Target);
				((EntitySystem)this).RaiseLocalEvent<AttackedEvent>(hit, attackedEv, false);
				DamageSpecifier modifiedDamage = DamageSpecifier.ApplyModifierSets(damage + hitEvent.BonusDamage + attackedEv.BonusDamage, hitEvent.ModifiersList);
				FixedPoint2? fixedPoint = _damageable.TryChangeDamage(hit, _xeno.TryApplyXenoSlashDamageMultiplier(hit, modifiedDamage), ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoTailStabComponent>.op_Implicit(stab), Entity<XenoTailStabComponent>.op_Implicit(stab))?.GetTotal();
				FixedPoint2 value = FixedPoint2.Zero;
				if (fixedPoint > value)
				{
					damaged = true;
					_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { hit }, filter);
				}
				if (_net.IsServer)
				{
					string text = EntProtoId.op_Implicit(stab.Comp.HitAnimationId);
					EntityCoordinates val = hit.ToCoordinates();
					val2 = default(Angle);
					((EntitySystem)this).SpawnAttachedTo(text, val, (ComponentRegistry)null, val2);
					if (_size.TryGetSize(Entity<XenoTailStabComponent>.op_Implicit(stab), out var size))
					{
						if ((int)size >= 5)
						{
							_daze.TryDaze(hit, stab.Comp.BigDazeTime, refresh: true);
						}
						else if (size == RMCSizes.Xeno)
						{
							_daze.TryDaze(hit, stab.Comp.DazeTime, refresh: true);
						}
					}
				}
				_cameraShake.ShakeCamera(hit, 2, 1);
				if (!((EntitySystem)this).HasComp<XenoComponent>(hit))
				{
					NeurotoxinInjectorComponent neuroTox = default(NeurotoxinInjectorComponent);
					Entity<SolutionComponent>? solutionEnt;
					Solution solution;
					if (stab.Comp.InjectNeuro && ((EntitySystem)this).TryComp<NeurotoxinInjectorComponent>(Entity<XenoTailStabComponent>.op_Implicit(stab), ref neuroTox))
					{
						NeurotoxinComponent neuro = default(NeurotoxinComponent);
						if (!((EntitySystem)this).EnsureComp<NeurotoxinComponent>(hit, ref neuro))
						{
							neuro.LastMessage = _timing.CurTime;
							neuro.LastAccentTime = _timing.CurTime;
							neuro.LastStumbleTime = _timing.CurTime;
						}
						neuro.NeurotoxinAmount += neuroTox.NeuroPerSecond;
						neuro.ToxinDamage = neuroTox.ToxinDamage;
						neuro.OxygenDamage = neuroTox.OxygenDamage;
						neuro.CoughDamage = neuroTox.CoughDamage;
					}
					else if (stab.Comp.Inject != null && _solutionContainer.TryGetInjectableSolution(Entity<InjectableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(hit), out solutionEnt, out solution))
					{
						FixedPoint2 total = FixedPoint2.Zero;
						foreach (FixedPoint2 amount in stab.Comp.Inject.Values)
						{
							total += amount;
						}
						FixedPoint2 available = solutionEnt.Value.Comp.Solution.AvailableVolume;
						if (available < total)
						{
							_solutionContainer.SplitSolution(solutionEnt.Value, total - available);
						}
						foreach (KeyValuePair<ProtoId<ReagentPrototype>, FixedPoint2> item in stab.Comp.Inject)
						{
							item.Deconstruct(out var key, out value);
							ProtoId<ReagentPrototype> reagent = key;
							FixedPoint2 amount2 = value;
							_solutionContainer.TryAddReagent(solutionEnt.Value, ProtoId<ReagentPrototype>.op_Implicit(reagent), amount2);
						}
					}
				}
				IdentityEntity hitName = Identity.Name(hit, (IEntityManager)(object)base.EntityManager, Entity<XenoTailStabComponent>.op_Implicit(stab));
				string msg = base.Loc.GetString("rmc-xeno-tail-stab-self", (ValueTuple<string, object>)("target", hitName));
				if (_net.IsServer)
				{
					_popup.PopupEntity(msg, Entity<XenoTailStabComponent>.op_Implicit(stab), Entity<XenoTailStabComponent>.op_Implicit(stab));
				}
				IdentityEntity userName = Identity.Name(Entity<XenoTailStabComponent>.op_Implicit(stab), (IEntityManager)(object)base.EntityManager, hit);
				msg = base.Loc.GetString("rmc-xeno-tail-stab-target", (ValueTuple<string, object>)("user", userName));
				_popup.PopupEntity(msg, Entity<XenoTailStabComponent>.op_Implicit(stab), hit, PopupType.MediumCaution);
				Filter othersFilter = Filter.PvsExcept(Entity<XenoTailStabComponent>.op_Implicit(stab), 2f, (IEntityManager)null).RemovePlayerByAttachedEntity(hit);
				foreach (ICommonSession recipient in othersFilter.Recipients)
				{
					EntityUid? attachedEntity = recipient.AttachedEntity;
					if (attachedEntity.HasValue)
					{
						EntityUid otherEnt = attachedEntity.GetValueOrDefault();
						userName = Identity.Name(Entity<XenoTailStabComponent>.op_Implicit(stab), (IEntityManager)(object)base.EntityManager, otherEnt);
						hitName = Identity.Name(hit, (IEntityManager)(object)base.EntityManager, otherEnt);
						msg = base.Loc.GetString("rmc-xeno-tail-stab-others", (ValueTuple<string, object>)("user", userName), (ValueTuple<string, object>)("target", hitName));
						_popup.PopupEntity(msg, Entity<XenoTailStabComponent>.op_Implicit(stab), othersFilter, recordReplay: true, PopupType.SmallCaution);
					}
				}
			}
		}
		SoundSpecifier obj;
		if (_net.IsServer)
		{
			if (args.Entity.HasValue && !((EntitySystem)this).TerminatingOrDeleted(args.Entity, (MetaDataComponent)null))
			{
				val2 = _transform.GetWorldRotation(Entity<XenoTailStabComponent>.op_Implicit(stab));
				Angle angle = DirectionExtensions.ToAngle(((Angle)(ref val2)).GetDir()) - Angle.FromDegrees(180.0);
				_rotate.RotateXeno(Entity<XenoTailStabComponent>.op_Implicit(stab), ((Angle)(ref angle)).GetDir());
			}
			if (args.Entity.HasValue && damaged && !((EntitySystem)this).TerminatingOrDeleted(args.Entity, (MetaDataComponent)null))
			{
				EntityUid? attachedEntity = args.Entity;
				EntityUid val3 = Entity<XenoTailStabComponent>.op_Implicit(stab);
				if (!attachedEntity.HasValue || attachedEntity.GetValueOrDefault() != val3)
				{
					obj = stab.Comp.SoundHit;
					goto IL_0a8e;
				}
			}
			obj = stab.Comp.SoundMiss;
			goto IL_0a8e;
		}
		goto IL_0ab3;
		IL_0a8e:
		SoundSpecifier sound = obj;
		_audio.PlayPvs(sound, Entity<XenoTailStabComponent>.op_Implicit(stab), (AudioParams?)null);
		goto IL_0ab3;
		IL_0ab3:
		MeleeAttackEvent attackEv = new MeleeAttackEvent(Entity<XenoTailStabComponent>.op_Implicit(stab));
		((EntitySystem)this).RaiseLocalEvent<MeleeAttackEvent>(Entity<XenoTailStabComponent>.op_Implicit(stab), ref attackEv, false);
	}

	protected virtual void DoLunge(Entity<XenoTailStabComponent, TransformComponent> user, Vector2 localPos, EntProtoId animationId)
	{
	}
}
