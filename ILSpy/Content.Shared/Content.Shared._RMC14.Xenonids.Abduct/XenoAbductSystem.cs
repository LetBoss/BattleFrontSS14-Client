using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Line;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Hook;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Abduct;

public sealed class XenoAbductSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private RMCDazedSystem _dazed;

	[Dependency]
	private SharedDoAfterSystem _doafter;

	[Dependency]
	private SharedRMCEmoteSystem _emote;

	[Dependency]
	private XenoHookSystem _hook;

	[Dependency]
	private LineSystem _line;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private MobStateSystem _mob;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private XenoPlasmaSystem _plasma;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private RMCPullingSystem _pulling;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private RMCObstacleSlammingSystem _rmcObstacleSlamming;

	[Dependency]
	private RMCSizeStunSystem _size;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private StatusEffectsSystem _status;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private XenoSystem _xeno;

	private readonly HashSet<EntityUid> _abductEnts = new HashSet<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoAbductComponent, XenoAbductActionEvent>((EntityEventRefHandler<XenoAbductComponent, XenoAbductActionEvent>)OnXenoAbduct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoAbductComponent, XenoAbductDoAfterEvent>((EntityEventRefHandler<XenoAbductComponent, XenoAbductDoAfterEvent>)OnXenoAbductDoafter, (Type[])null, (Type[])null);
	}

	private void OnXenoAbduct(Entity<XenoAbductComponent> xeno, ref XenoAbductActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_plasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.Cost))
		{
			return;
		}
		CleanUpTiles(xeno);
		NetCoordinates target = ((EntitySystem)this).GetNetCoordinates(args.Target, (MetaDataComponent)null);
		EntityCoordinates xenoCoords = _transform.GetMoverCoordinates(Entity<XenoAbductComponent>.op_Implicit(xeno));
		if ((target.Position - xenoCoords.Position).Length() > (float)xeno.Comp.Range)
		{
			Vector2 newTile = Vector2Helpers.Normalized(target.Position - xenoCoords.Position) * xeno.Comp.Range;
			((NetCoordinates)(ref target))._002Ector(((EntitySystem)this).GetNetEntity(args.Target.EntityId, (MetaDataComponent)null), xenoCoords.Position + newTile);
		}
		EntityUid? blocker;
		List<LineTile> tiles = _line.DrawLine(xenoCoords, ((EntitySystem)this).GetCoordinates(target), TimeSpan.Zero, xeno.Comp.Range, out blocker);
		if (tiles.Count == 0)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-abduct-no-room"), Entity<XenoAbductComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return;
		}
		XenoAbductDoAfterEvent duct = new XenoAbductDoAfterEvent();
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoAbductComponent>.op_Implicit(xeno), xeno.Comp.DoafterTime, duct, Entity<XenoAbductComponent>.op_Implicit(xeno))
		{
			BreakOnMove = true,
			DuplicateCondition = DuplicateConditions.SameEvent,
			BlockDuplicate = true
		};
		if (!_doafter.TryStartDoAfter(doAfter))
		{
			return;
		}
		_stun.TrySlowdown(Entity<XenoAbductComponent>.op_Implicit(xeno), xeno.Comp.DoafterTime, refresh: false, 0f, 0f);
		if (_net.IsClient)
		{
			return;
		}
		foreach (LineTile tile in tiles)
		{
			xeno.Comp.Tiles.Add(((EntitySystem)this).Spawn(EntProtoId.op_Implicit(xeno.Comp.Telegraph), tile.Coordinates, (ComponentRegistry)null, default(Angle)));
		}
		ProtoId<EmotePrototype>? emote = xeno.Comp.Emote;
		if (emote.HasValue)
		{
			ProtoId<EmotePrototype> emote2 = emote.GetValueOrDefault();
			_emote.TryEmoteWithChat(Entity<XenoAbductComponent>.op_Implicit(xeno), emote2);
		}
	}

	private void OnXenoAbductDoafter(Entity<XenoAbductComponent> xeno, ref XenoAbductDoAfterEvent args)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-abduct-cancel"), Entity<XenoAbductComponent>.op_Implicit(xeno), Entity<XenoAbductComponent>.op_Implicit(xeno), PopupType.Medium);
			CleanUpTiles(xeno);
			DoCooldown(xeno);
			_status.TryRemoveStatusEffect(Entity<XenoAbductComponent>.op_Implicit(xeno), "SlowedDown");
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		XenoHookComponent hook = default(XenoHookComponent);
		if (!_plasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.Cost) || _net.IsClient || !((EntitySystem)this).TryComp<XenoHookComponent>(Entity<XenoAbductComponent>.op_Implicit(xeno), ref hook))
		{
			return;
		}
		(EntityUid, XenoHookComponent) hookEnt = (xeno.Owner, hook);
		List<EntityUid> targets = new List<EntityUid>();
		foreach (EntityUid tile in xeno.Comp.Tiles)
		{
			_abductEnts.Clear();
			_lookup.GetEntitiesInRange(tile.ToCoordinates(), xeno.Comp.TileRadius, _abductEnts, (LookupFlags)110);
			foreach (EntityUid ent in _abductEnts)
			{
				if (!((EntitySystem)this).HasComp<StunnedComponent>(ent) && _xeno.CanAbilityAttackTarget(Entity<XenoAbductComponent>.op_Implicit(xeno), ent) && !_mob.IsCritical(ent) && (!_size.TryGetSize(ent, out var targetSize) || (int)targetSize < 5) && !targets.Contains(ent))
				{
					targets.Add(ent);
				}
			}
		}
		CleanUpTiles(xeno);
		string popupMsg = base.Loc.GetString("rmc-xeno-abduct-none");
		_audio.PlayPvs(xeno.Comp.Sound, Entity<XenoAbductComponent>.op_Implicit(xeno), (AudioParams?)null);
		TimeSpan slowTime = TimeSpan.Zero;
		TimeSpan rootTime = TimeSpan.Zero;
		TimeSpan dazeTime = TimeSpan.Zero;
		TimeSpan stunTime = TimeSpan.FromSeconds(0.4);
		if (targets.Count > 2)
		{
			popupMsg = base.Loc.GetString("rmc-xeno-abduct-more", (ValueTuple<string, object>)("targets", targets.Count));
			stunTime = xeno.Comp.StunTime;
		}
		else if (targets.Count == 2)
		{
			popupMsg = base.Loc.GetString("rmc-xeno-abduct-two");
			rootTime = xeno.Comp.RootTime;
			dazeTime = xeno.Comp.DazeTime;
		}
		else if (targets.Count == 1)
		{
			popupMsg = base.Loc.GetString("rmc-xeno-abduct-one");
			slowTime = xeno.Comp.SlowTime;
		}
		DoCooldown(xeno);
		_popup.PopupEntity(popupMsg, Entity<XenoAbductComponent>.op_Implicit(xeno), Entity<XenoAbductComponent>.op_Implicit(xeno), PopupType.Medium);
		float dis = default(float);
		for (int i = 0; i < targets.Count && i < xeno.Comp.MaxTargets; i++)
		{
			EntityUid ent2 = targets[i];
			if (_hook.TryHookTarget(Entity<XenoHookComponent>.op_Implicit(hookEnt), ent2))
			{
				_pulling.TryStopAllPullsFromAndOn(ent2);
				EntityCoordinates origin = _transform.GetMoverCoordinates(Entity<XenoAbductComponent>.op_Implicit(xeno));
				MapCoordinates mapCoords = _transform.GetMapCoordinates(Entity<XenoAbductComponent>.op_Implicit(xeno), (TransformComponent)null);
				EntityCoordinates target = _transform.GetMoverCoordinates(ent2);
				if (!((EntityCoordinates)(ref origin)).TryDistance((IEntityManager)(object)base.EntityManager, target, ref dis))
				{
					break;
				}
				_slow.TrySlowdown(ent2, slowTime, refresh: true, ignoreDurationModifier: true);
				_slow.TryRoot(ent2, _xeno.TryApplyXenoDebuffMultiplier(ent2, rootTime));
				_dazed.TryDaze(ent2, dazeTime, refresh: true, null, stutter: true);
				_stun.TryParalyze(ent2, _xeno.TryApplyXenoDebuffMultiplier(ent2, stunTime), refresh: true);
				float knockBackDistance = 0f - Math.Max(dis - 2f, 0.5f);
				_rmcObstacleSlamming.MakeImmune(ent2);
				_size.KnockBack(ent2, mapCoords, knockBackDistance, knockBackDistance, 10f);
			}
		}
	}

	private void CleanUpTiles(Entity<XenoAbductComponent> xeno)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		foreach (EntityUid til in xeno.Comp.Tiles)
		{
			((EntitySystem)this).QueueDel((EntityUid?)til);
		}
		xeno.Comp.Tiles.Clear();
		((EntitySystem)this).Dirty<XenoAbductComponent>(xeno, (MetaDataComponent)null);
	}

	private void DoCooldown(Entity<XenoAbductComponent> xeno)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoAbductActionEvent>(Entity<XenoAbductComponent>.op_Implicit(xeno)))
		{
			_actions.SetCooldown(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action))), xeno.Comp.Cooldown);
		}
	}
}
