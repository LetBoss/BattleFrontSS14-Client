using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client.Weather;
using Content.Shared._CIV14merka.Particles;
using Content.Shared.Light.EntitySystems;
using Content.Shared.Weather;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._CIV14merka.Particles;

public sealed class CivParticleSystem : EntitySystem
{
	[Dependency]
	private readonly IOverlayManager _overlay;

	[Dependency]
	private readonly IPrototypeManager _proto;

	[Dependency]
	private readonly IPlayerManager _player;

	[Dependency]
	private readonly IResourceCache _resource;

	[Dependency]
	private readonly IGameTiming _timing;

	[Dependency]
	private readonly IMapManager _mapMan;

	[Dependency]
	private readonly WeatherSystem _weather;

	private CivParticleOverlay? _overlayInstance;

	private readonly Dictionary<string, CivParticlePresetPrototype> _byWeather = new Dictionary<string, CivParticlePresetPrototype>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		BuildLookup();
		_proto.PrototypesReloaded += delegate
		{
			BuildLookup();
		};
		_overlayInstance = new CivParticleOverlay((IEntityManager)(object)base.EntityManager, _mapMan, base.EntityManager.System<SharedTransformSystem>(), base.EntityManager.System<SharedRoofSystem>(), _resource);
		_overlay.AddOverlay((Overlay)(object)_overlayInstance);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		if (_overlayInstance != null)
		{
			_overlay.RemoveOverlay((Overlay)(object)_overlayInstance);
		}
		_overlayInstance = null;
	}

	private void BuildLookup()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		_byWeather.Clear();
		foreach (CivParticlePresetPrototype item in _proto.EnumeratePrototypes<CivParticlePresetPrototype>())
		{
			foreach (ProtoId<WeatherPrototype> weatherType in item.WeatherTypes)
			{
				_byWeather[weatherType.Id] = item;
			}
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		if (_overlayInstance == null)
		{
			return;
		}
		_overlayInstance.Preset = null;
		_overlayInstance.Intensity = 0f;
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid? mapUid = ((EntitySystem)this).Transform(localEntity.Value).MapUid;
		WeatherComponent weatherComponent = default(WeatherComponent);
		if (!mapUid.HasValue || !((EntitySystem)this).TryComp<WeatherComponent>(mapUid.Value, ref weatherComponent) || weatherComponent.Weather.Count == 0)
		{
			return;
		}
		CivParticlePresetPrototype civParticlePresetPrototype = null;
		float num = 0f;
		foreach (var (val2, component) in weatherComponent.Weather)
		{
			if (_byWeather.TryGetValue(val2.Id, out CivParticlePresetPrototype value))
			{
				float percent = _weather.GetPercent(component, mapUid.Value);
				if (percent > num)
				{
					num = percent;
					civParticlePresetPrototype = value;
				}
			}
		}
		if (civParticlePresetPrototype != null)
		{
			_overlayInstance.Preset = civParticlePresetPrototype;
			_overlayInstance.Intensity = num;
			_overlayInstance.Wind = GetWind();
		}
	}

	private Vector2 GetWind()
	{
		float num = (float)_timing.RealTime.TotalSeconds;
		float num2 = 1.4f + MathF.Sin(num * 0.13f) * 0.9f + MathF.Sin(num * 0.41f) * 0.4f;
		float x = MathF.Sin(num * 0.05f) * 0.6f;
		return new Vector2(MathF.Cos(x), MathF.Sin(x)) * num2;
	}
}
