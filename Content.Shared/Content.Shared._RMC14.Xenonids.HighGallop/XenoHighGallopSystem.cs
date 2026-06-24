using System;
using System.Numerics;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared.Maps;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.HighGallop;

public sealed class XenoHighGallopSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private RMCPullingSystem _pulling;

	[Dependency]
	private TagSystem _tags;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private SharedRMCEmoteSystem _emote;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private RMCSizeStunSystem _size;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoHighGallopComponent, XenoHighGallopActionEvent>((EntityEventRefHandler<XenoHighGallopComponent, XenoHighGallopActionEvent>)OnHighGallopAction, (Type[])null, (Type[])null);
	}

	private void OnHighGallopAction(Entity<XenoHighGallopComponent> xeno, ref XenoHighGallopActionEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
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
		((HandledEntityEventArgs)args).Handled = true;
		_emote.TryEmoteWithChat(Entity<XenoHighGallopComponent>.op_Implicit(xeno), xeno.Comp.Emote);
		_audio.PlayPredicted(xeno.Comp.Sound, Entity<XenoHighGallopComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoHighGallopComponent>.op_Implicit(xeno), (AudioParams?)null);
		Angle direction = DirectionExtensions.ToAngle(Vector2Helpers.Normalized(args.Target.Position - _transform.GetMoverCoordinates(Entity<XenoHighGallopComponent>.op_Implicit(xeno)).Position)) - Angle.FromDegrees(90.0);
		EntityCoordinates xenoCoord = _transform.GetMoverCoordinates(Entity<XenoHighGallopComponent>.op_Implicit(xeno));
		Box2 val = Box2.CenteredAround(xenoCoord.Position, new Vector2(xeno.Comp.Width, xeno.Comp.Height));
		Box2 area = ((Box2)(ref val)).Translated(new Vector2(0f, xeno.Comp.Height / 2f + 0.5f));
		Box2Rotated rot = default(Box2Rotated);
		((Box2Rotated)(ref rot))._002Ector(area, direction, xenoCoord.Position);
		Box2 bounds = ((Box2Rotated)(ref rot)).CalcBoundingBox();
		if (_net.IsClient)
		{
			return;
		}
		foreach (TileRef tile in _map.GetTilesIntersecting(gridId, grid2, rot, true, (Predicate<TileRef>)null))
		{
			EntProtoId spawn = xeno.Comp.TelegraphEffect;
			val = Box2.CenteredAround(_turf.GetTileCenter(tile).Position, Vector2.One);
			if (!((Box2)(ref bounds)).Encloses(ref val))
			{
				spawn = xeno.Comp.TelegraphEffectEdge;
			}
			((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(spawn), _turf.GetTileCenter(tile), (ComponentRegistry)null);
		}
		foreach (EntityUid ent in _lookup.GetEntitiesIntersecting(((EntitySystem)this).Transform(Entity<XenoHighGallopComponent>.op_Implicit(xeno)).MapID, rot, (LookupFlags)110))
		{
			RMCSizes size;
			if (_tags.HasTag(ent, xeno.Comp.Flingable))
			{
				_pulling.TryStopAllPullsFromAndOn(ent);
				MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoHighGallopComponent>.op_Implicit(xeno), (TransformComponent)null);
				_size.KnockBack(ent, origin, xeno.Comp.FlingDistance, xeno.Comp.FlingDistance, 10f, ignoreSize: true);
			}
			else if (_xeno.CanAbilityAttackTarget(Entity<XenoHighGallopComponent>.op_Implicit(xeno), ent) && (!_size.TryGetSize(ent, out size) || (int)size < 5))
			{
				_stun.TryParalyze(ent, _xeno.TryApplyXenoDebuffMultiplier(ent, xeno.Comp.StunDuration), refresh: true);
				_slow.TrySlowdown(ent, _xeno.TryApplyXenoDebuffMultiplier(ent, xeno.Comp.SlowDuration));
			}
		}
	}
}
