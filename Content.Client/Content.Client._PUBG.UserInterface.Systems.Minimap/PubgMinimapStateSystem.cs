using System;
using System.Numerics;
using Content.Shared._PUBG;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public sealed class PubgMinimapStateSystem : EntitySystem
{
	public Vector2? ZoneCurrentCenter { get; private set; }

	public float ZoneCurrentRadius { get; private set; }

	public Vector2? ZoneNextCenter { get; private set; }

	public float ZoneNextRadius { get; private set; }

	public bool ZoneActive { get; private set; }

	public bool ZoneVisible { get; private set; }

	public MapId ZoneMapId { get; private set; } = MapId.Nullspace;

	public bool RedZoneActive { get; private set; }

	public Vector2? RedZoneCenter { get; private set; }

	public float RedZoneRadius { get; private set; }

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgZoneStateEvent>((EntitySessionEventHandler<PubgZoneStateEvent>)OnZoneStateUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<RedZoneStateEvent>((EntitySessionEventHandler<RedZoneStateEvent>)OnRedZoneStateUpdate, (Type[])null, (Type[])null);
	}

	private void OnZoneStateUpdate(PubgZoneStateEvent msg, EntitySessionEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((EntitySystem)this).GetEntity(msg.ZoneMapEntity);
		MapId zoneMapId = MapId.Nullspace;
		MapComponent val = default(MapComponent);
		if (((EntitySystem)this).TryComp<MapComponent>(entity, ref val))
		{
			zoneMapId = val.MapId;
		}
		ZoneCurrentCenter = msg.CurrentCenter;
		ZoneCurrentRadius = msg.CurrentRadius;
		ZoneNextCenter = msg.NextCenter;
		ZoneNextRadius = msg.NextRadius;
		ZoneActive = msg.Active;
		ZoneVisible = msg.Visible;
		ZoneMapId = zoneMapId;
	}

	private void OnRedZoneStateUpdate(RedZoneStateEvent msg, EntitySessionEventArgs args)
	{
		RedZoneActive = msg.ZoneActive;
		if (!msg.ZoneActive)
		{
			RedZoneCenter = null;
			RedZoneRadius = 0f;
		}
		else
		{
			RedZoneCenter = msg.ZoneCenter;
			RedZoneRadius = msg.ZoneRadius;
		}
	}
}
