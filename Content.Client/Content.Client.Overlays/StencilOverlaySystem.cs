using Content.Client.Parallax;
using Content.Client.Weather;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Overlays;

public sealed class StencilOverlaySystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private ParallaxSystem _parallax;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private WeatherSystem _weather;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_overlay.AddOverlay((Overlay)(object)new StencilOverlay(_parallax, _transform, _map, _sprite, _weather));
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlay.RemoveOverlay<StencilOverlay>();
	}
}
