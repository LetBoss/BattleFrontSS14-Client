using System;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Rules;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Rangefinder;

public sealed class RangefinderSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedDropshipWeaponSystem _dropshipWeapon;

	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private MetaDataSystem _metaData;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private RMCPlanetSystem _rmcPlanet;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SquadSystem _squad;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private UseDelaySystem _useDelay;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RangefinderComponent, MapInitEvent>((EntityEventRefHandler<RangefinderComponent, MapInitEvent>)OnRangefinderMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RangefinderComponent, AfterInteractEvent>((EntityEventRefHandler<RangefinderComponent, AfterInteractEvent>)OnRangefinderAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RangefinderComponent, LaserDesignatorDoAfterEvent>((EntityEventRefHandler<RangefinderComponent, LaserDesignatorDoAfterEvent>)OnRangefinderDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RangefinderComponent, ExaminedEvent>((EntityEventRefHandler<RangefinderComponent, ExaminedEvent>)OnRangefinderExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RangefinderComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<RangefinderComponent, GetVerbsEvent<AlternativeVerb>>)OnRangefinderGetAlternativeVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveLaserDesignatorComponent, ComponentRemove>((EntityEventRefHandler<ActiveLaserDesignatorComponent, ComponentRemove>)OnLaserDesignatorRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveLaserDesignatorComponent, EntityTerminatingEvent>((EntityEventRefHandler<ActiveLaserDesignatorComponent, EntityTerminatingEvent>)OnLaserDesignatorRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveLaserDesignatorComponent, DroppedEvent>((EntityEventRefHandler<ActiveLaserDesignatorComponent, DroppedEvent>)OnLaserDesignatorDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveLaserDesignatorComponent, RMCDroppedEvent>((EntityEventRefHandler<ActiveLaserDesignatorComponent, RMCDroppedEvent>)OnLaserDesignatorDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveLaserDesignatorComponent, GotUnequippedHandEvent>((EntityEventRefHandler<ActiveLaserDesignatorComponent, GotUnequippedHandEvent>)OnLaserDesignatorDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveLaserDesignatorComponent, GotUnequippedEvent>((EntityEventRefHandler<ActiveLaserDesignatorComponent, GotUnequippedEvent>)OnLaserDesignatorUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LaserDesignatorTargetComponent, ComponentRemove>((EntityEventRefHandler<LaserDesignatorTargetComponent, ComponentRemove>)OnLaserDesignatorTargetRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LaserDesignatorTargetComponent, EntityTerminatingEvent>((EntityEventRefHandler<LaserDesignatorTargetComponent, EntityTerminatingEvent>)OnLaserDesignatorTargetRemove, (Type[])null, (Type[])null);
	}

	private void OnRangefinderMapInit(Entity<RangefinderComponent> rangefinder, ref MapInitEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			RangefinderComponent comp = rangefinder.Comp;
			if (comp.CanDesignate)
			{
				comp.Id = _dropshipWeapon.ComputeNextId();
			}
			else
			{
				comp.Mode = RangefinderMode.Rangefinder;
			}
			if (comp.SwitchModeDelay > TimeSpan.Zero)
			{
				_useDelay.SetLength(Entity<UseDelayComponent>.op_Implicit(rangefinder.Owner), comp.SwitchModeDelay, comp.SwitchModeUseDelay);
			}
			if (comp.TargetDelay > TimeSpan.Zero)
			{
				_useDelay.SetLength(Entity<UseDelayComponent>.op_Implicit(rangefinder.Owner), comp.TargetDelay, comp.TargetUseDelay);
			}
			((EntitySystem)this).Dirty<RangefinderComponent>(rangefinder, (MetaDataComponent)null);
			UpdateAppearance(rangefinder);
		}
	}

	private void OnRangefinderAfterInteract(Entity<RangefinderComponent> rangefinder, ref AfterInteractEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		EntityCoordinates coordinates = args.ClickLocation.SnapToGrid((IEntityManager?)(object)base.EntityManager, _mapManager);
		if (!((EntityCoordinates)(ref coordinates)).IsValid((IEntityManager)(object)base.EntityManager))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (!_examine.InRangeUnOccluded(user, coordinates, rangefinder.Comp.Range))
		{
			string msg = base.Loc.GetString("rmc-laser-designator-out-of-range");
			_popup.PopupClient(msg, coordinates, user, PopupType.SmallCaution);
			return;
		}
		TimeSpan delay = rangefinder.Comp.Delay;
		int skill = _skills.GetSkill(Entity<SkillsComponent>.op_Implicit(user), rangefinder.Comp.Skill);
		delay -= skill * rangefinder.Comp.TimePerSkillLevel;
		if (delay < rangefinder.Comp.MinimumDelay)
		{
			delay = rangefinder.Comp.MinimumDelay;
		}
		EntityUid? grid = _transform.GetGrid(coordinates);
		if (!((EntitySystem)this).HasComp<RMCPlanetComponent>(grid))
		{
			string msg = base.Loc.GetString("rmc-laser-designator-not-surface");
			_popup.PopupClient(msg, coordinates, user, PopupType.SmallCaution);
		}
		else if (rangefinder.Comp.Mode == RangefinderMode.Rangefinder)
		{
			TryTarget(rangefinder, user, delay, coordinates);
		}
		else if (((EntitySystem)this).HasComp<ActiveLaserDesignatorComponent>(Entity<RangefinderComponent>.op_Implicit(rangefinder)))
		{
			string msg = base.Loc.GetString("rmc-laser-designator-already-targeting");
			_popup.PopupClient(msg, coordinates, user, PopupType.SmallCaution);
		}
		else if (!_dropshipWeapon.CasDebug && (!_area.CanCAS(coordinates) || (rangefinder.Comp.Mode == RangefinderMode.Designator && !_area.CanLase(coordinates))))
		{
			string msg = base.Loc.GetString("rmc-laser-designator-not-cas");
			_popup.PopupClient(msg, coordinates, user, PopupType.SmallCaution);
		}
		else
		{
			TryTarget(rangefinder, args.User, delay, coordinates);
		}
	}

	private void OnRangefinderDoAfter(Entity<RangefinderComponent> rangefinder, ref LaserDesignatorDoAfterEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityCoordinates coords = ((EntitySystem)this).GetCoordinates(args.Coordinates);
		if (!((EntityCoordinates)(ref coords)).IsValid((IEntityManager)(object)base.EntityManager))
		{
			return;
		}
		if (rangefinder.Comp.Mode == RangefinderMode.Designator)
		{
			string msg = base.Loc.GetString("rmc-laser-designator-acquired");
			_popup.PopupClient(msg, coords, user, PopupType.Medium);
		}
		_audio.PlayPredicted(rangefinder.Comp.AcquireSound, Entity<RangefinderComponent>.op_Implicit(rangefinder), (EntityUid?)user, (AudioParams?)null);
		if (_net.IsClient)
		{
			return;
		}
		ActiveLaserDesignatorComponent active = ((EntitySystem)this).EnsureComp<ActiveLaserDesignatorComponent>(Entity<RangefinderComponent>.op_Implicit(rangefinder));
		active.BreakRange = rangefinder.Comp.BreakRange;
		((EntitySystem)this).QueueDel(active.Target);
		EntProtoId modeLaser = ((rangefinder.Comp.Mode == RangefinderMode.Designator) ? EntProtoId<LaserDesignatorTargetComponent>.op_Implicit(rangefinder.Comp.DesignatorSpawn) : rangefinder.Comp.RangefinderSpawn);
		coords = _transform.GetMoverCoordinates(coords);
		active.Target = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(modeLaser), coords);
		active.Origin = _transform.GetMoverCoordinates(Entity<RangefinderComponent>.op_Implicit(rangefinder));
		((EntitySystem)this).Dirty(Entity<RangefinderComponent>.op_Implicit(rangefinder), (IComponent)(object)active, (MetaDataComponent)null);
		if (rangefinder.Comp.Mode == RangefinderMode.Rangefinder)
		{
			MapCoordinates mapCoords = _transform.ToMapCoordinates(coords, true);
			Vector2i position = Vector2Helpers.Floored(mapCoords.Position);
			if (_rmcPlanet.TryGetOffset(mapCoords, out var offset))
			{
				position += offset;
			}
			rangefinder.Comp.LastTarget = position;
			rangefinder.Comp.LastCoords = mapCoords;
			((EntitySystem)this).Dirty<RangefinderComponent>(rangefinder, (MetaDataComponent)null);
			_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(rangefinder.Owner), (Enum)RangefinderUiKey.Key, (EntityUid?)args.User, false);
			return;
		}
		EntityUid targetEnt = active.Target.Value;
		LaserDesignatorTargetComponent target = ((EntitySystem)this).EnsureComp<LaserDesignatorTargetComponent>(targetEnt);
		int id = (target.Id = EnsureId(rangefinder));
		((EntitySystem)this).Dirty(targetEnt, (IComponent)(object)target, (MetaDataComponent)null);
		string name = base.Loc.GetString("rmc-laser-designator-target-name", (ValueTuple<string, object>)("id", id));
		string abbreviation = _dropshipWeapon.GetUserAbbreviation(user, id);
		if (_squad.TryGetMemberSquad(Entity<SquadMemberComponent>.op_Implicit(user), out Entity<SquadTeamComponent> squad))
		{
			name = base.Loc.GetString("rmc-laser-designator-target-name-squad", (ValueTuple<string, object>)("squad", squad), (ValueTuple<string, object>)("id", id));
		}
		_dropshipWeapon.MakeDropshipTarget(targetEnt, abbreviation);
		_metaData.SetEntityName(targetEnt, name, (MetaDataComponent)null, true);
	}

	private void OnRangefinderExamined(Entity<RangefinderComponent> rangefinder, ref ExaminedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("RangefinderComponent"))
		{
			RangefinderComponent comp = rangefinder.Comp;
			Vector2i? lastTarget = comp.LastTarget;
			if (lastTarget.HasValue)
			{
				Vector2i target = lastTarget.GetValueOrDefault();
				args.PushMarkup(base.Loc.GetString("rmc-rangefinder-examine", new(string, object)[3]
				{
					("item", rangefinder),
					("x", target.X),
					("y", target.Y)
				}));
			}
			int? id = comp.Id;
			if (id.HasValue)
			{
				int id2 = id.GetValueOrDefault();
				args.PushMarkup(base.Loc.GetString("rmc-laser-designator-examine-id", (ValueTuple<string, object>)("id", id2)));
			}
			if (comp.CanDesignate)
			{
				switch (comp.Mode)
				{
				case RangefinderMode.Rangefinder:
				{
					string msg2 = base.Loc.GetString("rmc-laser-designator-in-rangefinder-mode", (ValueTuple<string, object>)("item", rangefinder));
					args.PushMarkup(msg2);
					break;
				}
				case RangefinderMode.Designator:
				{
					string msg = base.Loc.GetString("rmc-laser-designator-in-designator-mode", (ValueTuple<string, object>)("item", rangefinder));
					args.PushMarkup(msg);
					break;
				}
				}
				args.PushMarkup(base.Loc.GetString("rmc-laser-designator-to-switch"));
			}
		}
	}

	private void OnRangefinderGetAlternativeVerbs(Entity<RangefinderComponent> rangefinder, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanInteract || !args.CanAccess)
		{
			return;
		}
		RangefinderMode nextMode = ((rangefinder.Comp.Mode == RangefinderMode.Rangefinder) ? RangefinderMode.Designator : RangefinderMode.Rangefinder);
		if (nextMode != RangefinderMode.Designator || rangefinder.Comp.CanDesignate)
		{
			args.Verbs.Add(new AlternativeVerb
			{
				Priority = 100,
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					ChangeDesignatorMode(rangefinder, nextMode);
				},
				Text = base.Loc.GetString("rmc-laser-designator-switch-mode", (ValueTuple<string, object>)("mode", nextMode))
			});
		}
	}

	private void OnLaserDesignatorRemove<T>(Entity<ActiveLaserDesignatorComponent> active, ref T args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			((EntitySystem)this).Del(active.Comp.Target);
		}
	}

	private void OnLaserDesignatorDropped<T>(Entity<ActiveLaserDesignatorComponent> active, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<ActiveLaserDesignatorComponent>(Entity<ActiveLaserDesignatorComponent>.op_Implicit(active));
	}

	private void OnLaserDesignatorUnequipped(Entity<ActiveLaserDesignatorComponent> active, ref GotUnequippedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<ActiveLaserDesignatorComponent>(Entity<ActiveLaserDesignatorComponent>.op_Implicit(active));
	}

	private void OnLaserDesignatorTargetRemove<T>(Entity<LaserDesignatorTargetComponent> target, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		ActiveLaserDesignatorComponent active = default(ActiveLaserDesignatorComponent);
		if (((EntitySystem)this).TryComp<ActiveLaserDesignatorComponent>(target.Comp.LaserDesignator, ref active))
		{
			active.Target = null;
			((EntitySystem)this).Dirty(target.Comp.LaserDesignator.Value, (IComponent)(object)active, (MetaDataComponent)null);
		}
	}

	private void ChangeDesignatorMode(Entity<RangefinderComponent> rangefinder, RangefinderMode mode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (!rangefinder.Comp.CanDesignate)
		{
			return;
		}
		string delay = rangefinder.Comp.SwitchModeUseDelay;
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(Entity<RangefinderComponent>.op_Implicit(rangefinder), ref useDelay))
		{
			if (_useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((Entity<RangefinderComponent>.op_Implicit(rangefinder), useDelay)), delay))
			{
				return;
			}
			_useDelay.TryResetDelay(Entity<RangefinderComponent>.op_Implicit(rangefinder), checkDelayed: false, useDelay, delay);
		}
		if (rangefinder.Comp.DoAfter != null && _doAfter.IsRunning(rangefinder.Comp.DoAfter.Id))
		{
			_doAfter.Cancel(rangefinder.Comp.DoAfter.Id);
		}
		rangefinder.Comp.Mode = mode;
		((EntitySystem)this).Dirty<RangefinderComponent>(rangefinder, (MetaDataComponent)null);
		UpdateAppearance(rangefinder);
	}

	private void UpdateAppearance(Entity<RangefinderComponent> rangefinder)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<RangefinderComponent>.op_Implicit(rangefinder), (Enum)RangefinderLayers.Layer, (object)rangefinder.Comp.Mode, (AppearanceComponent)null);
	}

	private int EnsureId(Entity<RangefinderComponent> rangefinder)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		RangefinderComponent comp = rangefinder.Comp;
		int valueOrDefault = comp.Id.GetValueOrDefault();
		if (!comp.Id.HasValue)
		{
			valueOrDefault = _dropshipWeapon.ComputeNextId();
			comp.Id = valueOrDefault;
		}
		return rangefinder.Comp.Id.Value;
	}

	private void TryTarget(Entity<RangefinderComponent> rangefinder, EntityUid user, TimeSpan delay, EntityCoordinates coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(Entity<RangefinderComponent>.op_Implicit(rangefinder), ref useDelay))
		{
			if (_useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((Entity<RangefinderComponent>.op_Implicit(rangefinder), useDelay)), rangefinder.Comp.TargetUseDelay))
			{
				return;
			}
			_useDelay.TryResetDelay(Entity<RangefinderComponent>.op_Implicit(rangefinder), checkDelayed: false, useDelay, rangefinder.Comp.TargetUseDelay);
		}
		LaserDesignatorDoAfterEvent ev = new LaserDesignatorDoAfterEvent(((EntitySystem)this).GetNetCoordinates(coordinates, (MetaDataComponent)null));
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, ev, Entity<RangefinderComponent>.op_Implicit(rangefinder))
		{
			BreakOnMove = true,
			NeedHand = true,
			BreakOnHandChange = false,
			MovementThreshold = 0.5f
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			string msg = base.Loc.GetString("rmc-laser-designator-start");
			_popup.PopupClient(msg, coordinates, user, PopupType.Medium);
			_audio.PlayPredicted(rangefinder.Comp.TargetSound, Entity<RangefinderComponent>.op_Implicit(rangefinder), (EntityUid?)user, (AudioParams?)null);
			rangefinder.Comp.DoAfter = ev.DoAfter;
		}
	}
}
