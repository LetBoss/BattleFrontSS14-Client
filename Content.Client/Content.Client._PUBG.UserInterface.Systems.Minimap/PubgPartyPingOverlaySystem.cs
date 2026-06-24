using Content.Shared._RMC14.Vehicle;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public sealed class PubgPartyPingOverlaySystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private VehicleSystem _vehicles;

	private PubgPartyPingOverlay? _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		PubgPartyPingClientSystem pingSystem = base.EntityManager.System<PubgPartyPingClientSystem>();
		_overlay = new PubgPartyPingOverlay(_player, _transform, pingSystem, _vehicles);
		_overlayManager.AddOverlay((Overlay)(object)_overlay);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		if (_overlay != null)
		{
			_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
			_overlay = null;
		}
	}
}
