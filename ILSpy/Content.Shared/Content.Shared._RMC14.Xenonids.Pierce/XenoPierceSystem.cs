using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Line;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.ScissorCut;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Pierce;

public sealed class XenoPierceSystem : EntitySystem
{
	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedRMCEmoteSystem _emote;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private VanguardShieldSystem _vanguard;

	[Dependency]
	private SharedRMCMeleeWeaponSystem _rmcMelee;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private LineSystem _line;

	[Dependency]
	private EntityLookupSystem _lookup;

	private readonly HashSet<Entity<MarineComponent>> _pierceEnts = new HashSet<Entity<MarineComponent>>();

	private readonly HashSet<EntityUid> _hitAlready = new HashSet<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoPierceComponent, XenoPierceActionEvent>((EntityEventRefHandler<XenoPierceComponent, XenoPierceActionEvent>)OnXenoPierceAction, (Type[])null, (Type[])null);
	}

	private void OnXenoPierceAction(Entity<XenoPierceComponent> xeno, ref XenoPierceActionEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_rmcActions.TryUseAction(args))
		{
			return;
		}
		EntityUid? blocker = _transform.GetGrid(args.Target);
		if (!blocker.HasValue)
		{
			return;
		}
		EntityUid gridId = blocker.GetValueOrDefault();
		if (!((EntitySystem)this).HasComp<MapGridComponent>(gridId))
		{
			return;
		}
		EntityCoordinates target = args.Target;
		EntityCoordinates xenoCoords = _transform.GetMoverCoordinates(Entity<XenoPierceComponent>.op_Implicit(xeno));
		float dis = default(float);
		if (!((EntityCoordinates)(ref args.Target)).TryDistance((IEntityManager)(object)base.EntityManager, xenoCoords, ref dis))
		{
			return;
		}
		if (dis > xeno.Comp.Range)
		{
			Vector2 newTile = Vector2Helpers.Normalized(args.Target.Position - xenoCoords.Position) * xeno.Comp.Range.Float();
			target = ((EntityCoordinates)(ref xenoCoords)).WithPosition(xenoCoords.Position + newTile);
		}
		List<LineTile> tiles = _line.DrawLine(xenoCoords, target, TimeSpan.Zero, xeno.Comp.Range.Float(), out blocker);
		if (tiles.Count == 0)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		_hitAlready.Clear();
		int hits = 0;
		EntityUid? hitEnt = null;
		DestroyOnXenoPierceScissorComponent destroy = default(DestroyOnXenoPierceScissorComponent);
		foreach (LineTile tile in tiles)
		{
			_pierceEnts.Clear();
			EntityUid entTile = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(xeno.Comp.Blocker), tile.Coordinates, (ComponentRegistry)null, default(Angle));
			_lookup.GetEntitiesInRange<MarineComponent>(entTile.ToCoordinates(), 0.5f, _pierceEnts, (LookupFlags)110);
			foreach (Entity<MarineComponent> ent in _pierceEnts)
			{
				if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(entTile), Entity<TransformComponent>.op_Implicit(ent.Owner), xeno.Comp.Range.Float()))
				{
					continue;
				}
				if (((EntitySystem)this).TryComp<DestroyOnXenoPierceScissorComponent>(Entity<MarineComponent>.op_Implicit(ent), ref destroy))
				{
					if (_net.IsServer)
					{
						((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(destroy.SpawnPrototype), ent.Owner.ToCoordinates(), (ComponentRegistry)null);
						((EntitySystem)this).QueueDel((EntityUid?)Entity<MarineComponent>.op_Implicit(ent));
					}
					_audio.PlayPredicted(destroy.Sound, Entity<MarineComponent>.op_Implicit(ent), (EntityUid?)Entity<XenoPierceComponent>.op_Implicit(xeno), (AudioParams?)null);
				}
				else
				{
					if (!_xeno.CanAbilityAttackTarget(Entity<XenoPierceComponent>.op_Implicit(xeno), Entity<MarineComponent>.op_Implicit(ent)) || !_hitAlready.Add(Entity<MarineComponent>.op_Implicit(ent)))
					{
						continue;
					}
					hits++;
					if (_damage.TryChangeDamage(Entity<MarineComponent>.op_Implicit(ent), _xeno.TryApplyXenoSlashDamageMultiplier(Entity<MarineComponent>.op_Implicit(ent), xeno.Comp.Damage), ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoPierceComponent>.op_Implicit(xeno), armorPiercing: xeno.Comp.AP, tool: Entity<XenoPierceComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
					{
						Filter filter = Filter.Pvs(Entity<MarineComponent>.op_Implicit(ent), 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
						_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { Entity<MarineComponent>.op_Implicit(ent) }, filter);
					}
					if (_net.IsServer)
					{
						((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.AttackEffect), ent.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
					}
					if (!hitEnt.HasValue)
					{
						hitEnt = Entity<MarineComponent>.op_Implicit(ent);
					}
					if (xeno.Comp.MaxTargets.HasValue && hits >= xeno.Comp.MaxTargets)
					{
						break;
					}
				}
			}
		}
		_emote.TryEmoteWithChat(Entity<XenoPierceComponent>.op_Implicit(xeno), xeno.Comp.Emote, hideLog: false, null, ignoreActionBlocker: false, forceEmote: false, xeno.Comp.EmoteCooldown);
		if (hits > 0 && hitEnt.HasValue)
		{
			_rmcMelee.DoLunge(Entity<XenoPierceComponent>.op_Implicit(xeno), hitEnt.Value);
		}
		if (_net.IsServer)
		{
			_audio.PlayPvs(xeno.Comp.Sound, Entity<XenoPierceComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
		if (hits >= xeno.Comp.RechargeTargetsRequired)
		{
			_vanguard.RegenShield(Entity<XenoPierceComponent>.op_Implicit(xeno));
		}
	}
}
