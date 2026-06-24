using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Heal;
using Content.Shared._RMC14.Xenonids.Stab;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Flurry;

public sealed class XenoFlurrySystem : EntitySystem
{
	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedRMCEmoteSystem _emote;

	[Dependency]
	private SharedRMCMeleeWeaponSystem _rmcMelee;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private SharedXenoHealSystem _xenoHeal;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoFlurryComponent, XenoFlurryActionEvent>((EntityEventRefHandler<XenoFlurryComponent, XenoFlurryActionEvent>)OnXenoFlurryAction, (Type[])null, (Type[])null);
	}

	private void OnXenoFlurryAction(Entity<XenoFlurryComponent> xeno, ref XenoFlurryActionEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_rmcActions.TryUseAction(args))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid? grid = _transform.GetGrid(args.Target);
		if (!grid.HasValue)
		{
			return;
		}
		EntityUid gridId = grid.GetValueOrDefault();
		MapGridComponent grid2 = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
		{
			return;
		}
		Angle direction = DirectionExtensions.ToAngle(Vector2Helpers.Normalized(args.Target.Position - _transform.GetMoverCoordinates(Entity<XenoFlurryComponent>.op_Implicit(xeno)).Position)) - Angle.FromDegrees(90.0);
		EntityCoordinates xenoCoord = _transform.GetMoverCoordinates(Entity<XenoFlurryComponent>.op_Implicit(xeno));
		Box2 val = Box2.CenteredAround(xenoCoord.Position, new Vector2(1f, xeno.Comp.Range));
		Box2 area = ((Box2)(ref val)).Translated(new Vector2(0f, xeno.Comp.Range / 2f + 0.5f));
		Box2Rotated rot = default(Box2Rotated);
		((Box2Rotated)(ref rot))._002Ector(area, direction, xenoCoord.Position);
		List<EntityUid> mobs = new List<EntityUid>();
		if (_net.IsClient)
		{
			return;
		}
		foreach (Entity<PhysicsComponent> ent in _physics.GetCollidingEntities(((EntitySystem)this).Transform(Entity<XenoFlurryComponent>.op_Implicit(xeno)).MapID, ref rot))
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoFlurryComponent>.op_Implicit(xeno), Entity<PhysicsComponent>.op_Implicit(ent)))
			{
				mobs.Add(Entity<PhysicsComponent>.op_Implicit(ent));
			}
		}
		_emote.TryEmoteWithChat(Entity<XenoFlurryComponent>.op_Implicit(xeno), xeno.Comp.Emote, hideLog: false, null, ignoreActionBlocker: false, forceEmote: false, xeno.Comp.EmoteDelay);
		DamageSpecifier damage = new DamageSpecifier(xeno.Comp.Damage);
		RMCGetTailStabBonusDamageEvent ev = new RMCGetTailStabBonusDamageEvent(new DamageSpecifier());
		((EntitySystem)this).RaiseLocalEvent<RMCGetTailStabBonusDamageEvent>(Entity<XenoFlurryComponent>.op_Implicit(xeno), ref ev, false);
		damage += ev.Damage;
		int hits = 0;
		EntityUid? hitEnt = null;
		foreach (EntityUid victim in mobs)
		{
			if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(xeno.Owner), Entity<TransformComponent>.op_Implicit(victim), xeno.Comp.Range + 0.5f))
			{
				continue;
			}
			if (!hitEnt.HasValue)
			{
				hitEnt = victim;
			}
			hits++;
			if (_damage.TryChangeDamage(victim, _xeno.TryApplyXenoSlashDamageMultiplier(victim, damage), ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoFlurryComponent>.op_Implicit(xeno), Entity<XenoFlurryComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
			{
				Filter filter = Filter.Pvs(victim, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
				_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { victim }, filter);
			}
			((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.AttackEffect), victim.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			_audio.PlayEntity(xeno.Comp.SlashSound, Entity<XenoFlurryComponent>.op_Implicit(xeno), victim, (AudioParams?)null);
			((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.HealEffect), xeno.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			_xenoHeal.CreateHealStacks(Entity<XenoFlurryComponent>.op_Implicit(xeno), xeno.Comp.HealAmount, xeno.Comp.HealDelay, xeno.Comp.HealCharges, xeno.Comp.HealDelay);
			if (xeno.Comp.MaxTargets.HasValue && hits >= xeno.Comp.MaxTargets)
			{
				break;
			}
		}
		if (hitEnt.HasValue)
		{
			_rmcMelee.DoLunge(Entity<XenoFlurryComponent>.op_Implicit(xeno), hitEnt.Value);
		}
		((Box2Rotated)(ref rot)).CalcBoundingBox();
		foreach (TileRef tile in _map.GetTilesIntersecting(gridId, grid2, rot, true, (Predicate<TileRef>)null))
		{
			if (_interaction.InRangeUnobstructed(xeno.Owner, _turf.GetTileCenter(tile), xeno.Comp.Range + 0.5f))
			{
				((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(xeno.Comp.TelegraphEffect), _turf.GetTileCenter(tile), (ComponentRegistry)null);
			}
		}
	}
}
