using System;
using System.Collections.Generic;
using Content.Client.Atmos.Overlays;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.GameTicking;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Atmos.EntitySystems;

internal sealed class AtmosDebugOverlaySystem : SharedAtmosDebugOverlaySystem
{
	public readonly Dictionary<EntityUid, AtmosDebugOverlayMessage> TileData = new Dictionary<EntityUid, AtmosDebugOverlayMessage>();

	public AtmosDebugOverlayMode CfgMode;

	public float CfgBase;

	public float CfgScale = 207.85599f;

	public int CfgSpecificGas;

	public bool CfgCBM;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)Reset, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<AtmosDebugOverlayMessage>((EntityEventHandler<AtmosDebugOverlayMessage>)HandleAtmosDebugOverlayMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<AtmosDebugOverlayDisableMessage>((EntityEventHandler<AtmosDebugOverlayDisableMessage>)HandleAtmosDebugOverlayDisableMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GridRemovalEvent>((EntityEventHandler<GridRemovalEvent>)OnGridRemoved, (Type[])null, (Type[])null);
		IOverlayManager val = IoCManager.Resolve<IOverlayManager>();
		if (!val.HasOverlay<AtmosDebugOverlay>())
		{
			val.AddOverlay((Overlay)(object)new AtmosDebugOverlay(this));
		}
	}

	private void OnGridRemoved(GridRemovalEvent ev)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (TileData.ContainsKey(ev.EntityUid))
		{
			TileData.Remove(ev.EntityUid);
		}
	}

	private void HandleAtmosDebugOverlayMessage(AtmosDebugOverlayMessage message)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		TileData[((EntitySystem)this).GetEntity(message.GridId)] = message;
	}

	private void HandleAtmosDebugOverlayDisableMessage(AtmosDebugOverlayDisableMessage ev)
	{
		TileData.Clear();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		IOverlayManager val = IoCManager.Resolve<IOverlayManager>();
		if (val.HasOverlay<AtmosDebugOverlay>())
		{
			val.RemoveOverlay<AtmosDebugOverlay>();
		}
	}

	public void Reset(RoundRestartCleanupEvent ev)
	{
		TileData.Clear();
	}

	public bool HasData(EntityUid gridId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return TileData.ContainsKey(gridId);
	}
}
