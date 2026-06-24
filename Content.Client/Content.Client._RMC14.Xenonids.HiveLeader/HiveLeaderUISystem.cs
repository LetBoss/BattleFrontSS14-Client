using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Xenonids.HiveLeader;

public sealed class HiveLeaderUISystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlay;

	public override void Initialize()
	{
		if (!_overlay.HasOverlay<HiveLeaderOverlay>())
		{
			_overlay.AddOverlay((Overlay)(object)new HiveLeaderOverlay());
		}
	}

	public override void Shutdown()
	{
		_overlay.RemoveOverlay<HiveLeaderOverlay>();
	}
}
