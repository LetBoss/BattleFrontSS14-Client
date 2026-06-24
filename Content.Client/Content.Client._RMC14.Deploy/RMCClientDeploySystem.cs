using System;
using Content.Shared._RMC14.Deploy;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Deploy;

public sealed class RMCClientDeploySystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<RMCShowDeployAreaEvent>((EntityEventHandler<RMCShowDeployAreaEvent>)OnShowDeployArea, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<RMCHideDeployAreaEvent>((EntityEventHandler<RMCHideDeployAreaEvent>)OnHideDeployArea, (Type[])null, (Type[])null);
	}

	private void OnShowDeployArea(RMCShowDeployAreaEvent ev)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		RMCDeployAreaOverlay rMCDeployAreaOverlay = new RMCDeployAreaOverlay();
		rMCDeployAreaOverlay.Box = ev.Box;
		rMCDeployAreaOverlay.Color = ev.Color;
		_overlayManager.AddOverlay((Overlay)(object)rMCDeployAreaOverlay);
	}

	private void OnHideDeployArea(RMCHideDeployAreaEvent ev)
	{
		_overlayManager.RemoveOverlay<RMCDeployAreaOverlay>();
	}
}
