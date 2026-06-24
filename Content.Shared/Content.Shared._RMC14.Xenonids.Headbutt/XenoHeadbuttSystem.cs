using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Animation;
using Content.Shared._RMC14.Xenonids.Crest;
using Content.Shared._RMC14.Xenonids.Fortify;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Headbutt;

public sealed class XenoHeadbuttSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private RMCObstacleSlammingSystem _rmcObstacleSlamming;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private RMCSizeStunSystem _sizeStun;

	[Dependency]
	private ThrowingSystem _throwing;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private ThrownItemSystem _thrownItem;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private XenoAnimationsSystem _xenoAnimations;

	[Dependency]
	private XenoSystem _xeno;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	private EntityQuery<ThrownItemComponent> _thrownItemQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_physicsQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		_thrownItemQuery = ((EntitySystem)this).GetEntityQuery<ThrownItemComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoHeadbuttComponent, XenoHeadbuttActionEvent>((EntityEventRefHandler<XenoHeadbuttComponent, XenoHeadbuttActionEvent>)OnXenoHeadbuttAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoHeadbuttComponent, ThrowDoHitEvent>((EntityEventRefHandler<XenoHeadbuttComponent, ThrowDoHitEvent>)OnXenoHeadbuttHit, (Type[])null, (Type[])null);
	}

	private void OnXenoHeadbuttAction(Entity<XenoHeadbuttComponent> xeno, ref XenoHeadbuttActionEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		XenoCrestComponent crest = default(XenoCrestComponent);
		if (((EntitySystem)this).TryComp<XenoCrestComponent>(Entity<XenoHeadbuttComponent>.op_Implicit(xeno), ref crest) && crest.Lowered && !_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(xeno.Owner), Entity<TransformComponent>.op_Implicit(args.Target)))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-headbutt-too-far"), Entity<XenoHeadbuttComponent>.op_Implicit(xeno), Entity<XenoHeadbuttComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return;
		}
		XenoHeadbuttAttemptEvent attempt = default(XenoHeadbuttAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoHeadbuttAttemptEvent>(Entity<XenoHeadbuttComponent>.op_Implicit(xeno), ref attempt, false);
		if (!attempt.Cancelled && _rmcActions.TryUseAction(args))
		{
			_rmcPulling.TryStopAllPullsFromAndOn(Entity<XenoHeadbuttComponent>.op_Implicit(xeno));
			((HandledEntityEventArgs)args).Handled = true;
			MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoHeadbuttComponent>.op_Implicit(xeno), (TransformComponent)null);
			Vector2 diff = _transform.GetMapCoordinates(args.Target, (TransformComponent)null).Position - origin.Position;
			diff = Vector2Helpers.Normalized(diff) * xeno.Comp.Range;
			xeno.Comp.Charge = diff;
			((EntitySystem)this).Dirty<XenoHeadbuttComponent>(xeno, (MetaDataComponent)null);
			_rmcObstacleSlamming.MakeImmune(Entity<XenoHeadbuttComponent>.op_Implicit(xeno));
			_throwing.TryThrow(Entity<XenoHeadbuttComponent>.op_Implicit(xeno), diff);
		}
	}

	private void OnXenoHeadbuttHit(Entity<XenoHeadbuttComponent> xeno, ref ThrowDoHitEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid targetId = args.Target;
		if (!_xeno.CanAbilityAttackTarget(Entity<XenoHeadbuttComponent>.op_Implicit(xeno), targetId))
		{
			return;
		}
		PhysicsComponent physics = default(PhysicsComponent);
		ThrownItemComponent thrown = default(ThrownItemComponent);
		if (_physicsQuery.TryGetComponent(Entity<XenoHeadbuttComponent>.op_Implicit(xeno), ref physics) && _thrownItemQuery.TryGetComponent(Entity<XenoHeadbuttComponent>.op_Implicit(xeno), ref thrown))
		{
			_thrownItem.LandComponent(Entity<XenoHeadbuttComponent>.op_Implicit(xeno), thrown, physics, playSound: true);
			_thrownItem.StopThrow(Entity<XenoHeadbuttComponent>.op_Implicit(xeno), thrown);
		}
		if (_timing.IsFirstTimePredicted)
		{
			Vector2? charge = xeno.Comp.Charge;
			if (charge.HasValue)
			{
				Vector2 charge2 = charge.GetValueOrDefault();
				xeno.Comp.Charge = null;
				_xenoAnimations.PlayLungeAnimationEvent(Entity<XenoHeadbuttComponent>.op_Implicit(xeno), charge2);
			}
		}
		if (_net.IsServer)
		{
			_audio.PlayPvs(xeno.Comp.Sound, Entity<XenoHeadbuttComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
		DamageSpecifier finalDamage = xeno.Comp.Damage;
		XenoCrestComponent crest = default(XenoCrestComponent);
		if (((EntitySystem)this).TryComp<XenoCrestComponent>(Entity<XenoHeadbuttComponent>.op_Implicit(xeno), ref crest) && crest.Lowered)
		{
			finalDamage.ExclusiveAdd(xeno.Comp.CrestedDamageReduction);
		}
		if (_damageable.TryChangeDamage(targetId, _xeno.TryApplyXenoSlashDamageMultiplier(targetId, finalDamage), ignoreResistances: false, interruptsDoAfters: true, null, armorPiercing: xeno.Comp.AP, origin: Entity<XenoHeadbuttComponent>.op_Implicit(xeno), tool: Entity<XenoHeadbuttComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
		{
			Filter filter = Filter.Pvs(targetId, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
			_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { targetId }, filter);
		}
		XenoCrestComponent crest2 = default(XenoCrestComponent);
		XenoFortifyComponent fort = default(XenoFortifyComponent);
		float range = xeno.Comp.ThrowForce + (((((EntitySystem)this).TryComp<XenoCrestComponent>(Entity<XenoHeadbuttComponent>.op_Implicit(xeno), ref crest2) && crest2.Lowered) || (((EntitySystem)this).TryComp<XenoFortifyComponent>(Entity<XenoHeadbuttComponent>.op_Implicit(xeno), ref fort) && fort.Fortified)) ? xeno.Comp.CrestFortifiedThrowAdd : 0f);
		_rmcPulling.TryStopAllPullsFromAndOn(targetId);
		StopHeadbutt(Entity<XenoHeadbuttComponent>.op_Implicit(xeno));
		MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoHeadbuttComponent>.op_Implicit(xeno), (TransformComponent)null);
		_sizeStun.KnockBack(targetId, origin, range, range, 10f, ignoreSize: true);
		if (_net.IsServer)
		{
			((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.Effect), targetId.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		}
	}

	private void StopHeadbutt(EntityUid xeno)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		if (_physicsQuery.TryGetComponent(xeno, ref physics))
		{
			_physics.SetLinearVelocity(xeno, Vector2.Zero, true, true, (FixturesComponent)null, physics);
			_physics.SetBodyStatus(xeno, physics, (BodyStatus)0, true);
		}
	}
}
