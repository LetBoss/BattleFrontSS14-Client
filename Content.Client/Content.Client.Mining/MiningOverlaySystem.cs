using System;
using Content.Shared.Mining.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Mining;

public sealed class MiningOverlaySystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IOverlayManager _overlayMan;

	private MiningOverlay _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MiningScannerViewerComponent, ComponentInit>((EntityEventRefHandler<MiningScannerViewerComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MiningScannerViewerComponent, ComponentShutdown>((EntityEventRefHandler<MiningScannerViewerComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MiningScannerViewerComponent, LocalPlayerAttachedEvent>((EntityEventRefHandler<MiningScannerViewerComponent, LocalPlayerAttachedEvent>)OnPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MiningScannerViewerComponent, LocalPlayerDetachedEvent>((EntityEventRefHandler<MiningScannerViewerComponent, LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		_overlay = new MiningOverlay();
	}

	private void OnPlayerAttached(Entity<MiningScannerViewerComponent> ent, ref LocalPlayerAttachedEvent args)
	{
		_overlayMan.AddOverlay((Overlay)(object)_overlay);
	}

	private void OnPlayerDetached(Entity<MiningScannerViewerComponent> ent, ref LocalPlayerDetachedEvent args)
	{
		_overlayMan.RemoveOverlay((Overlay)(object)_overlay);
	}

	private void OnInit(Entity<MiningScannerViewerComponent> ent, ref ComponentInit args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid val = Entity<MiningScannerViewerComponent>.op_Implicit(ent);
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == val)
		{
			_overlayMan.AddOverlay((Overlay)(object)_overlay);
		}
	}

	private void OnShutdown(Entity<MiningScannerViewerComponent> ent, ref ComponentShutdown args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid val = Entity<MiningScannerViewerComponent>.op_Implicit(ent);
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == val)
		{
			_overlayMan.RemoveOverlay((Overlay)(object)_overlay);
		}
	}
}
