using System;
using Content.Shared._PUBG.Airdrop;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._PUBG.Airdrop;

public sealed class PubgAirdropOverlaySystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private IResourceCache _cache;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private SharedTransformSystem _transform;

	private PubgAirdropOverlay? _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgAirdropStateEvent>((EntitySessionEventHandler<PubgAirdropStateEvent>)OnAirdropState, (Type[])null, (Type[])null);
		_overlay = new PubgAirdropOverlay(_cache, _player, _transform);
	}

	private void OnAirdropState(PubgAirdropStateEvent ev, EntitySessionEventArgs args)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (_overlay != null)
		{
			_overlay.Active = ev.Active;
			_overlay.Target = ev.Position;
			_overlay.RemainingSeconds = ev.RemainingSeconds;
			_overlay.MapId = ev.MapId;
			if (ev.Active && !_overlayManager.HasOverlay<PubgAirdropOverlay>())
			{
				_overlayManager.AddOverlay((Overlay)(object)_overlay);
			}
			else if (!ev.Active && _overlayManager.HasOverlay<PubgAirdropOverlay>())
			{
				_overlayManager.RemoveOverlay<PubgAirdropOverlay>();
			}
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		if (_overlayManager.HasOverlay<PubgAirdropOverlay>())
		{
			_overlayManager.RemoveOverlay<PubgAirdropOverlay>();
		}
	}
}
