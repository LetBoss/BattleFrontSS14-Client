using System;
using System.Collections.Generic;
using Content.Shared.Fluids;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Fluids;

public sealed class PuddleDebugOverlaySystem : SharedPuddleDebugOverlaySystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	public readonly Dictionary<EntityUid, PuddleOverlayDebugMessage> TileData = new Dictionary<EntityUid, PuddleOverlayDebugMessage>();

	private PuddleOverlay? _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PuddleOverlayDisableMessage>((EntityEventHandler<PuddleOverlayDisableMessage>)DisableOverlay, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PuddleOverlayDebugMessage>((EntityEventHandler<PuddleOverlayDebugMessage>)RenderDebugData, (Type[])null, (Type[])null);
	}

	private void RenderDebugData(PuddleOverlayDebugMessage message)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		TileData[((EntitySystem)this).GetEntity(message.GridUid)] = message;
		if (_overlay == null)
		{
			_overlay = new PuddleOverlay();
			_overlayManager.AddOverlay((Overlay)(object)_overlay);
		}
	}

	private void DisableOverlay(PuddleOverlayDisableMessage message)
	{
		TileData.Clear();
		if (_overlay != null)
		{
			_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
			_overlay = null;
		}
	}

	public PuddleDebugOverlayData[] GetData(EntityUid mapGridGridEntityId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return TileData[mapGridGridEntityId].OverlayData;
	}
}
