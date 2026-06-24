using System;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Utility.Events;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Rules;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.GameTicking;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Dropship.Utility.Systems;

public sealed class RMCFultonSystem : EntitySystem
{
	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedDropshipWeaponSystem _dropshipWeapon;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRottingSystem _rotting;

	[Dependency]
	private RMCPlanetSystem _rmcPlanet;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SharedStackSystem _stack;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private RMCPullingSystem _rmcpulling;

	[Dependency]
	private RMCUnrevivableSystem _unrevivable;

	private int _fultonCount;

	private MapId? _fultonMap;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestartCleanup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCCanBeFultonedComponent, InteractUsingEvent>((EntityEventRefHandler<RMCCanBeFultonedComponent, InteractUsingEvent>)OnCanBeFultonedInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCCanBeFultonedComponent, RMCPrepareFultonDoAfterEvent>((EntityEventRefHandler<RMCCanBeFultonedComponent, RMCPrepareFultonDoAfterEvent>)OnCanBeFultonedPrepareFulton, (Type[])null, (Type[])null);
	}

	private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
	{
		_fultonCount = 0;
		_fultonMap = null;
	}

	private void OnCanBeFultonedInteractUsing(Entity<RMCCanBeFultonedComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		EntityUid target = args.Target;
		EntityUid used = args.Used;
		if (!((EntitySystem)this).HasComp<RMCFultonComponent>(used))
		{
			return;
		}
		if (_mobState.IsAlive(target) || _mobState.IsCritical(target))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-fulton-not-dead", (ValueTuple<string, object>)("fulton", used), (ValueTuple<string, object>)("target", target)), target, user);
			return;
		}
		if ((((EntitySystem)this).HasComp<PerishableComponent>(target) && !_rotting.IsRotten(target)) || (((EntitySystem)this).HasComp<RMCRevivableComponent>(target) && !_unrevivable.IsUnrevivable(target)))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-fulton-not-unrevivable", (ValueTuple<string, object>)("fulton", used), (ValueTuple<string, object>)("target", target)), target, user);
			return;
		}
		if (!_rmcPlanet.IsOnPlanet(target.ToCoordinates()))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-fulton-not-planet", (ValueTuple<string, object>)("fulton", used)), target, user);
			return;
		}
		if (!_area.CanFulton(target.ToCoordinates()))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-fulton-underground", (ValueTuple<string, object>)("fulton", used)), target, user);
			return;
		}
		TimeSpan delay = ent.Comp.Delay * _skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(user), ent.Comp.Skill);
		RMCPrepareFultonDoAfterEvent ev = new RMCPrepareFultonDoAfterEvent();
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, ev, Entity<RMCCanBeFultonedComponent>.op_Implicit(ent), Entity<RMCCanBeFultonedComponent>.op_Implicit(ent), used)
		{
			BreakOnMove = true
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			string selfMsg = base.Loc.GetString("rmc-fulton-attach-start-self", (ValueTuple<string, object>)("fulton", used), (ValueTuple<string, object>)("target", target));
			string othersMsg = base.Loc.GetString("rmc-fulton-attach-start-others", new(string, object)[3]
			{
				("user", user),
				("fulton", used),
				("target", target)
			});
			_popup.PopupPredicted(selfMsg, othersMsg, user, user);
		}
	}

	private void OnCanBeFultonedPrepareFulton(Entity<RMCCanBeFultonedComponent> ent, ref RMCPrepareFultonDoAfterEvent args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			((HandledEntityEventArgs)args).Handled = true;
			RMCActiveFultonComponent active = ((EntitySystem)this).EnsureComp<RMCActiveFultonComponent>(target2);
			active.ReturnAt = _timing.CurTime + ent.Comp.ReturnDelay;
			active.ReturnTo = _transform.GetMoverCoordinates(Entity<RMCCanBeFultonedComponent>.op_Implicit(ent));
			((EntitySystem)this).Dirty(target2, (IComponent)(object)active, (MetaDataComponent)null);
			RMCCanBeFultonedComponent canBeFultoned = default(RMCCanBeFultonedComponent);
			if (((EntitySystem)this).TryComp<RMCCanBeFultonedComponent>(target2, ref canBeFultoned))
			{
				_audio.PlayPredicted(canBeFultoned.FultonSound, active.ReturnTo, (EntityUid?)args.User, (AudioParams?)null);
			}
			string name = ((EntitySystem)this).Name(target2, (MetaDataComponent)null);
			_dropshipWeapon.MakeTarget(target2, name, targetableByWeapons: false);
			_rmcpulling.TryStopAllPullsFromAndOn(target2);
			MapId mapId = EnsureMap();
			_transform.SetMapCoordinates(target2, new MapCoordinates((float)(_fultonCount++ * 50), 0f, mapId));
			if (args.Used.HasValue)
			{
				_stack.Use(args.Used.Value, 1);
			}
		}
	}

	private MapId EnsureMap()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!_map.MapExists(_fultonMap))
		{
			_fultonMap = null;
		}
		if (!_fultonMap.HasValue)
		{
			MapId map = default(MapId);
			_map.CreateMap(ref map, true);
			_fultonMap = map;
		}
		return _fultonMap.Value;
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<RMCActiveFultonComponent> query = ((EntitySystem)this).EntityQueryEnumerator<RMCActiveFultonComponent>();
		EntityUid uid = default(EntityUid);
		RMCActiveFultonComponent comp = default(RMCActiveFultonComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (!(time < comp.ReturnAt))
			{
				((EntitySystem)this).RemComp<DropshipTargetComponent>(uid);
				((EntitySystem)this).RemCompDeferred<RMCActiveFultonComponent>(uid);
				_transform.SetCoordinates(uid, comp.ReturnTo);
				_audio.PlayPvs(comp.ReturnSound, comp.ReturnTo, (AudioParams?)null);
			}
		}
	}
}
