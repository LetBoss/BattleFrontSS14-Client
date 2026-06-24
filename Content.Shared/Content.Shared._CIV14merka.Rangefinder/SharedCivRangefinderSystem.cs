using System;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Timing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._CIV14merka.Rangefinder;

public abstract class SharedCivRangefinderSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private UseDelaySystem _useDelay;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<CivRangefinderComponent, MapInitEvent>((EntityEventRefHandler<CivRangefinderComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivRangefinderComponent, AfterInteractEvent>((EntityEventRefHandler<CivRangefinderComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivRangefinderComponent, CivRangefinderDoAfterEvent>((EntityEventRefHandler<CivRangefinderComponent, CivRangefinderDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivRangefinderComponent, ExaminedEvent>((EntityEventRefHandler<CivRangefinderComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivRangefinderComponent, ComponentRemove>((EntityEventRefHandler<CivRangefinderComponent, ComponentRemove>)OnRangefinderRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivRangefinderComponent, EntityTerminatingEvent>((EntityEventRefHandler<CivRangefinderComponent, EntityTerminatingEvent>)OnRangefinderRemove, (Type[])null, (Type[])null);
	}

	private void OnRangefinderRemove<T>(Entity<CivRangefinderComponent> rangefinder, ref T args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			EntityUid? markerEntity = rangefinder.Comp.MarkerEntity;
			if (markerEntity.HasValue)
			{
				EntityUid marker = markerEntity.GetValueOrDefault();
				((EntitySystem)this).QueueDel((EntityUid?)marker);
			}
		}
	}

	private void OnMapInit(Entity<CivRangefinderComponent> rangefinder, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (rangefinder.Comp.TargetDelay > TimeSpan.Zero)
		{
			_useDelay.SetLength(Entity<UseDelayComponent>.op_Implicit(rangefinder.Owner), rangefinder.Comp.TargetDelay, rangefinder.Comp.TargetUseDelay);
		}
		((EntitySystem)this).Dirty<CivRangefinderComponent>(rangefinder, (MetaDataComponent)null);
	}

	private void OnAfterInteract(Entity<CivRangefinderComponent> rangefinder, ref AfterInteractEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coords = args.ClickLocation.SnapToGrid((IEntityManager?)(object)base.EntityManager, _mapManager);
		if (((EntityCoordinates)(ref coords)).IsValid((IEntityManager)(object)base.EntityManager))
		{
			MapCoordinates mapCoords = _transform.ToMapCoordinates(coords, true);
			if (!(_transform.GetMapId(Entity<TransformComponent>.op_Implicit(args.User)) != mapCoords.MapId))
			{
				((HandledEntityEventArgs)args).Handled = true;
				TryTarget(rangefinder, args.User, coords);
			}
		}
	}

	private void OnDoAfter(Entity<CivRangefinderComponent> rangefinder, ref CivRangefinderDoAfterEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
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
		_audio.PlayPredicted(rangefinder.Comp.AcquireSound, Entity<CivRangefinderComponent>.op_Implicit(rangefinder), (EntityUid?)args.User, (AudioParams?)null);
		if (!_net.IsClient)
		{
			MapCoordinates mapCoords = _transform.ToMapCoordinates(coords, true);
			rangefinder.Comp.LastTarget = Vector2Helpers.Floored(mapCoords.Position);
			rangefinder.Comp.LastCoords = mapCoords;
			((EntitySystem)this).Dirty<CivRangefinderComponent>(rangefinder, (MetaDataComponent)null);
			EntityUid? markerEntity = rangefinder.Comp.MarkerEntity;
			if (markerEntity.HasValue)
			{
				EntityUid old = markerEntity.GetValueOrDefault();
				((EntitySystem)this).QueueDel((EntityUid?)old);
			}
			rangefinder.Comp.MarkerEntity = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(rangefinder.Comp.MarkerSpawn), _transform.GetMoverCoordinates(coords));
			_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(rangefinder.Owner), (Enum)CivRangefinderUiKey.Key, (EntityUid?)args.User, false);
		}
	}

	private void OnExamined(Entity<CivRangefinderComponent> rangefinder, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Vector2i? lastTarget = rangefinder.Comp.LastTarget;
		if (lastTarget.HasValue)
		{
			Vector2i target = lastTarget.GetValueOrDefault();
			using (args.PushGroup("CivRangefinderComponent"))
			{
				args.PushText(base.Loc.GetString("civ-eq-rangefinder-examine-coords", (ValueTuple<string, object>)("x", target.X), (ValueTuple<string, object>)("y", target.Y)));
			}
		}
	}

	private void TryTarget(Entity<CivRangefinderComponent> rangefinder, EntityUid user, EntityCoordinates coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(Entity<CivRangefinderComponent>.op_Implicit(rangefinder), ref useDelay))
		{
			if (_useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((Entity<CivRangefinderComponent>.op_Implicit(rangefinder), useDelay)), rangefinder.Comp.TargetUseDelay))
			{
				return;
			}
			_useDelay.TryResetDelay(Entity<CivRangefinderComponent>.op_Implicit(rangefinder), checkDelayed: false, useDelay, rangefinder.Comp.TargetUseDelay);
		}
		CivRangefinderDoAfterEvent ev = new CivRangefinderDoAfterEvent(((EntitySystem)this).GetNetCoordinates(coordinates, (MetaDataComponent)null));
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, rangefinder.Comp.TargetDelay, ev, Entity<CivRangefinderComponent>.op_Implicit(rangefinder))
		{
			BreakOnMove = true,
			NeedHand = true,
			BreakOnHandChange = false,
			MovementThreshold = 0.5f
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			_audio.PlayPredicted(rangefinder.Comp.TargetSound, Entity<CivRangefinderComponent>.op_Implicit(rangefinder), (EntityUid?)user, (AudioParams?)null);
			rangefinder.Comp.DoAfter = ev.DoAfter;
		}
	}
}
