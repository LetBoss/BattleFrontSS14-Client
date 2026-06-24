using System;
using Content.Client.Resources;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Events;
using Content.Shared.Shuttles.Systems;
using Content.Shared.Shuttles.UI.MapObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Components;

namespace Content.Client.Shuttles.Systems;

public sealed class ShuttleSystem : SharedShuttleSystem
{
	[Dependency]
	private IResourceCache _resource;

	[Dependency]
	private IOverlayManager _overlays;

	private bool _enableShuttlePosition;

	private EmergencyShuttleOverlay? _overlay;

	public bool EnableShuttlePosition
	{
		get
		{
			return _enableShuttlePosition;
		}
		set
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			if (_enableShuttlePosition != value)
			{
				_enableShuttlePosition = value;
				IOverlayManager val = IoCManager.Resolve<IOverlayManager>();
				if (_enableShuttlePosition)
				{
					_overlay = new EmergencyShuttleOverlay(((EntitySystem)this).EntityManager.TransformQuery, XformSystem);
					val.AddOverlay((Overlay)(object)_overlay);
					((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new EmergencyShuttleRequestPositionMessage());
				}
				else
				{
					val.RemoveOverlay((Overlay)(object)_overlay);
					_overlay = null;
				}
			}
		}
	}

	public Texture GetTexture(Entity<ShuttleMapParallaxComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ShuttleMapParallaxComponent>(Entity<ShuttleMapParallaxComponent>.op_Implicit(entity), ref entity.Comp, false))
		{
			return _resource.GetTexture(ShuttleMapParallaxComponent.FallbackTexture);
		}
		return _resource.GetTexture(entity.Comp.TexturePath);
	}

	public MapCoordinates GetMapCoordinates(IMapObject mapObj)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (!(mapObj is ShuttleBeaconObject shuttleBeaconObject))
		{
			if (!(mapObj is ShuttleExclusionObject shuttleExclusionObject))
			{
				if (mapObj is GridMapObject gridMapObject)
				{
					TransformComponent val = ((EntitySystem)this).Transform(gridMapObject.Entity);
					if (((EntitySystem)this).HasComp<MapComponent>(gridMapObject.Entity))
					{
						return new MapCoordinates(val.LocalPosition, val.MapID);
					}
					Entity<PhysicsComponent, TransformComponent> val2 = Entity<PhysicsComponent, TransformComponent>.op_Implicit((ValueTuple<EntityUid, PhysicsComponent, TransformComponent>)(gridMapObject.Entity, null, val));
					return new MapCoordinates(Maps.GetGridPosition(val2), val.MapID);
				}
				throw new ArgumentOutOfRangeException();
			}
			return XformSystem.ToMapCoordinates(((EntitySystem)this).GetCoordinates(shuttleExclusionObject.Coordinates), true);
		}
		return XformSystem.ToMapCoordinates(((EntitySystem)this).GetCoordinates(shuttleBeaconObject.Coordinates), true);
	}

	public override void Initialize()
	{
		base.Initialize();
		InitializeEmergency();
		_overlays.AddOverlay((Overlay)(object)new FtlArrivalOverlay());
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlays.RemoveOverlay<FtlArrivalOverlay>();
	}

	private void InitializeEmergency()
	{
		((EntitySystem)this).SubscribeNetworkEvent<EmergencyShuttlePositionMessage>((EntityEventHandler<EmergencyShuttlePositionMessage>)OnShuttlePosMessage, (Type[])null, (Type[])null);
	}

	private void OnShuttlePosMessage(EmergencyShuttlePositionMessage ev)
	{
		if (_overlay != null)
		{
			_overlay.StationUid = ((EntitySystem)this).GetEntity(ev.StationUid);
			_overlay.Position = ev.Position;
		}
	}
}
