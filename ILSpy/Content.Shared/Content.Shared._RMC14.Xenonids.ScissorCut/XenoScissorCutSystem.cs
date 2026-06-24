using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Empower;
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

namespace Content.Shared._RMC14.Xenonids.ScissorCut;

public sealed class XenoScissorCutSystem : EntitySystem
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
	private RMCSlowSystem _slow;

	[Dependency]
	private SharedRMCEmoteSystem _emote;

	[Dependency]
	private SharedRMCMeleeWeaponSystem _rmcMelee;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private TurfSystem _turf;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoScissorCutComponent, XenoScissorCutActionEvent>((EntityEventRefHandler<XenoScissorCutComponent, XenoScissorCutActionEvent>)OnXenoScissorCutAction, (Type[])null, (Type[])null);
	}

	private void OnXenoScissorCutAction(Entity<XenoScissorCutComponent> xeno, ref XenoScissorCutActionEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_0733: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_rmcActions.TryUseAction(args))
		{
			return;
		}
		bool slows = ((EntitySystem)this).HasComp<XenoSuperEmpoweredComponent>(Entity<XenoScissorCutComponent>.op_Implicit(xeno));
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
		Angle direction = DirectionExtensions.ToAngle(Vector2Helpers.Normalized(args.Target.Position - _transform.GetMoverCoordinates(Entity<XenoScissorCutComponent>.op_Implicit(xeno)).Position)) - Angle.FromDegrees(90.0);
		EntityCoordinates xenoCoord = _transform.GetMoverCoordinates(Entity<XenoScissorCutComponent>.op_Implicit(xeno));
		Box2 val = Box2.CenteredAround(xenoCoord.Position, new Vector2(1f, xeno.Comp.Range));
		Box2 area = ((Box2)(ref val)).Translated(new Vector2(0f, xeno.Comp.Range / 2f + 0.5f));
		Box2Rotated rot = default(Box2Rotated);
		((Box2Rotated)(ref rot))._002Ector(area, direction, xenoCoord.Position);
		List<EntityUid> destructibles = new List<EntityUid>();
		List<EntityUid> mobs = new List<EntityUid>();
		if (_net.IsClient)
		{
			return;
		}
		foreach (Entity<PhysicsComponent> ent in _physics.GetCollidingEntities(((EntitySystem)this).Transform(Entity<XenoScissorCutComponent>.op_Implicit(xeno)).MapID, ref rot))
		{
			if (((EntitySystem)this).HasComp<DamageOnXenoScissorsComponent>(Entity<PhysicsComponent>.op_Implicit(ent)) || ((EntitySystem)this).HasComp<DestroyOnXenoPierceScissorComponent>(Entity<PhysicsComponent>.op_Implicit(ent)))
			{
				destructibles.Add(Entity<PhysicsComponent>.op_Implicit(ent));
			}
			else if (_xeno.CanAbilityAttackTarget(Entity<XenoScissorCutComponent>.op_Implicit(xeno), Entity<PhysicsComponent>.op_Implicit(ent), canAttackBarricades: false, canAttackWindows: true))
			{
				mobs.Add(Entity<PhysicsComponent>.op_Implicit(ent));
			}
		}
		EntityCoordinates selfCoords = _transform.GetMoverCoordinates(Entity<XenoScissorCutComponent>.op_Implicit(xeno));
		destructibles = destructibles.OrderBy((EntityUid a) =>
		{
			float num = default(float);
			return (!((EntityCoordinates)(ref selfCoords)).TryDistance((IEntityManager)(object)base.EntityManager, a.ToCoordinates(), ref num)) ? 10f : num;
		}).ToList();
		DamageOnXenoScissorsComponent destruct = default(DamageOnXenoScissorsComponent);
		DestroyOnXenoPierceScissorComponent destoy = default(DestroyOnXenoPierceScissorComponent);
		foreach (EntityUid des in destructibles)
		{
			if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(xeno.Owner), Entity<TransformComponent>.op_Implicit(des), xeno.Comp.Range + 0.5f))
			{
				continue;
			}
			if (((EntitySystem)this).TryComp<DamageOnXenoScissorsComponent>(des, ref destruct))
			{
				if (_damage.TryChangeDamage(des, destruct.Damage, ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoScissorCutComponent>.op_Implicit(xeno), Entity<XenoScissorCutComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
				{
					Filter filter = Filter.Pvs(des, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
					_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { des }, filter);
				}
			}
			else if (((EntitySystem)this).TryComp<DestroyOnXenoPierceScissorComponent>(des, ref destoy))
			{
				((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(destoy.SpawnPrototype), des.ToCoordinates(), (ComponentRegistry)null);
				((EntitySystem)this).QueueDel((EntityUid?)des);
				_audio.PlayEntity(destoy.Sound, des, Entity<XenoScissorCutComponent>.op_Implicit(xeno), (AudioParams?)null);
			}
		}
		_emote.TryEmoteWithChat(Entity<XenoScissorCutComponent>.op_Implicit(xeno), xeno.Comp.Emote);
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
			if (_damage.TryChangeDamage(victim, xeno.Comp.Damage, ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoScissorCutComponent>.op_Implicit(xeno), Entity<XenoScissorCutComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
			{
				Filter filter2 = Filter.Pvs(victim, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
				_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { victim }, filter2);
			}
			((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.AttackEffect), victim.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			_audio.PlayEntity(xeno.Comp.SlashSound, Entity<XenoScissorCutComponent>.op_Implicit(xeno), victim, (AudioParams?)null);
			if (slows)
			{
				_slow.TrySuperSlowdown(victim, xeno.Comp.SuperSlowDuration, refresh: true, ignoreDurationModifier: true);
			}
		}
		if (hitEnt.HasValue)
		{
			_rmcMelee.DoLunge(Entity<XenoScissorCutComponent>.op_Implicit(xeno), hitEnt.Value);
		}
		Box2 bounds = ((Box2Rotated)(ref rot)).CalcBoundingBox();
		foreach (TileRef tile in _map.GetTilesIntersecting(gridId, grid2, rot, true, (Predicate<TileRef>)null))
		{
			if (_interaction.InRangeUnobstructed(xeno.Owner, _turf.GetTileCenter(tile), xeno.Comp.Range + 0.5f))
			{
				EntProtoId spawn = xeno.Comp.TelegraphEffect;
				val = Box2.CenteredAround(_turf.GetTileCenter(tile).Position, Vector2.One);
				if (!((Box2)(ref bounds)).Encloses(ref val))
				{
					spawn = xeno.Comp.TelegraphEffectEdge;
				}
				((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(spawn), _turf.GetTileCenter(tile), (ComponentRegistry)null);
			}
		}
	}
}
